using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.UnitTest.Common.Utilities;

public class QueryableBuilderBaseTests
{
	private IQueryable<TestEntity> GetTestEntities()
	{
		return new List<TestEntity>
		{
			new() { UpdatedAt = new DateTime(2023, 1, 1) },
			new() { UpdatedAt = new DateTime(2023, 2, 1) },
			new() { UpdatedAt = new DateTime(2023, 3, 1) }
		}.AsQueryable();
	}

	[Test]
	public void ApplyFilter_ShouldReturnAllEntities_WhenNoFilterIsApplied()
	{
		// Arrange
		var builder = new TestQueryableBuilder();
		var query = new QueryRequest();
		var entities = GetTestEntities();

		// Act
		var result = builder.ApplyFilter(entities, query);

		// Assert
		Assert.That(result.Count(), Is.EqualTo(3));
	}

	[Test]
	public void ApplySorting_ShouldSortEntitiesByUpdatedAt()
	{
		// Arrange
		var builder = new TestQueryableBuilder();
		var query = new QueryRequest { SortBy = "updated", SortOrder = "desc" };
		var entities = GetTestEntities();

		// Act
		var result = builder.ApplySorting(entities, query);

		// Assert
		Assert.That(result.First().UpdatedAt, Is.EqualTo(new DateTime(2023, 3, 1)));
	}

	[Test]
	public void ApplyPaging_ShouldPageEntities()
	{
		// Arrange
		var builder = new TestQueryableBuilder();
		var query = new QueryRequest { PageNumber = 2, PageSize = 1 };
		var entities = GetTestEntities();

		// Act
		var result = builder.ApplyPaging(entities, query);

		// Assert
		Assert.That(result.Count(), Is.EqualTo(1));
		Assert.That(result.First().UpdatedAt, Is.EqualTo(new DateTime(2023, 2, 1)));
	}

	[Test]
	public void SetSortingMode_ShouldSortEntitiesByDefault()
	{
		// Arrange
		var builder = new TestQueryableBuilder();
		var query = new QueryRequest { SortBy = "nonexistent", SortOrder = "asc" };
		var entities = GetTestEntities();

		// Act
		var result = builder.ApplySorting(entities, query);

		// Assert
		Assert.That(result.First().UpdatedAt, Is.EqualTo(new DateTime(2023, 1, 1)));
	}

	private class TestEntity : IEntity
	{
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string? Concurrency { get; set; }
	}

	private class TestQueryableBuilder : QueryableBuilderBase<TestEntity>
	{
		// No need to override methods if testing base implementation
	}
}

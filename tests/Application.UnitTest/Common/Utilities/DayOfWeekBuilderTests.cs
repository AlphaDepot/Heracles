using Application.Common.Utilities;

namespace Application.UnitTest.Common.Utilities;

public class DayOfWeekBuilderTests
{
	[Test]
	public void GetDayOfWeek_ShouldReturnCorrectDayOfWeekForAllValidDays()
	{
		// Arrange
		var days = new Dictionary<string, DayOfWeek>
		{
			{ "Sunday", DayOfWeek.Sunday },
			{ "Monday", DayOfWeek.Monday },
			{ "Tuesday", DayOfWeek.Tuesday },
			{ "Wednesday", DayOfWeek.Wednesday },
			{ "Thursday", DayOfWeek.Thursday },
			{ "Friday", DayOfWeek.Friday },
			{ "Saturday", DayOfWeek.Saturday }
		};

		foreach (var day in days)
		{
			// Act
			var result = DayOfWeekBuilder.GetDayOfWeek(day.Key);

			// Assert
			Assert.That(result, Is.EqualTo(day.Value));
		}
	}

	[Test]
	public void GetDayOfWeek_ShouldReturnNullForNullOrEmptyString()
	{
		// Act
		var resultForNull = DayOfWeekBuilder.GetDayOfWeek(null!);
		var resultForEmpty = DayOfWeekBuilder.GetDayOfWeek(string.Empty);

		// Assert
		Assert.That(resultForNull, Is.Null);
		Assert.That(resultForEmpty, Is.Null);
	}
}

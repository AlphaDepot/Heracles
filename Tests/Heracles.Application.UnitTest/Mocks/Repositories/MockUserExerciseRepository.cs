using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.UserExercises.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  The MockUserExerciseRepository class
/// </summary>
public  class MockUserExerciseRepository : MockBaseRepository<UserExercise, IUserExerciseRepository>
{
    /// <summary>
    ///  Create a new instance of the MockUserExerciseRepository
    /// </summary>
    /// <param name="entities"> The list of UserExercise entities</param>
    public MockUserExerciseRepository(List<UserExercise> entities) : base(entities)
    {
        GetUserExerciseByExerciseTypeIdAsyncMock();
    }
    /// <summary>
    ///  Get a new instance of the MockUserExerciseRepository
    /// </summary>
    /// <returns> A new instance of the MockUserExerciseRepository</returns>
    public new static Mock<IUserExerciseRepository> Get()
    {
        return new MockUserExerciseRepository(UserExerciseFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Set up the GetUserExerciseByExerciseTypeIdAsyncMock method
    ///  This method is used to mock the GetUserExerciseByExerciseTypeIdAsync method of the UserExerciseRepository
    /// </summary>
    private void GetUserExerciseByExerciseTypeIdAsyncMock()
    {
        // Setup UserExercise specific mock methods here
        MockRepo.Setup(r => r.GetUserExerciseByExerciseTypeIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => Entities.FirstOrDefault(q => q.ExerciseTypeId == id));
    }
    
    
}
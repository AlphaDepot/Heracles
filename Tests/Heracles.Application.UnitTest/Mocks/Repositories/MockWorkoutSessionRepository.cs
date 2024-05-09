using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockWorkoutSessionRepository
{

    public static Mock<IWorkoutSessionRepository> Get()
    {
        var workoutSessions = WorkoutSessionFixture.Get();
        
        var mockRepo = new Mock<IWorkoutSessionRepository>();
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<WorkoutSession>>()))
            .ReturnsAsync((QuariableDto<WorkoutSession> queryableDto) =>
            {
                var queryable = workoutSessions.AsQueryable();
                if (queryableDto.Filter != null)
                {
                    queryable = queryable.Where(queryableDto.Filter);
                }
                if (queryableDto.Sorter != null)
                {
                    queryable = queryableDto.Sorter(queryable);
                }
                var result = queryable.Skip((queryableDto.PageNumber - 1) * queryableDto.PageSize)
                    .Take(queryableDto.PageSize).ToList();
                
                return new QueryResponse<WorkoutSession>()
                    {
                        Data =  result,
                        TotalPages = result.Count(),
                        PageSize = queryableDto.PageSize,
                        PageNumber = queryableDto.PageNumber
                    };
            });
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!
            .ReturnsAsync((int id) => workoutSessions.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<WorkoutSession>()))
            .ReturnsAsync((WorkoutSession workoutSession) =>
            {
                workoutSession.Id = workoutSessions.Count + 1;
                workoutSessions.Add(workoutSession);
                return workoutSession.Id;
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => workoutSessions.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<WorkoutSession>()))
            .ReturnsAsync((WorkoutSession workoutSession) =>
            {
                var index = workoutSessions.FindIndex(q => q.Id == workoutSession.Id);
                workoutSessions[index] = workoutSession;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = workoutSessions.FindIndex(q => q.Id == id);
                workoutSessions.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.IsUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => !workoutSessions.Any(q => q.Name == name));
        
        return mockRepo;
        
    }
    
    
}
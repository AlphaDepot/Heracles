using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class UserExerciseHistoryRepository : GenericRepository<UserExerciseHistory>, IUserExerciseHistoryRepository
{
    public UserExerciseHistoryRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }
    
}
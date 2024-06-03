using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }
    
    public Task<User?> GetUserByUserIdAsync(string userId)
    {
        return DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }
    
    public async Task<bool> UserIdExistsAsync(string userId)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) != null;
    }
    
    public async Task<bool> UserIdWithIdExistsAsync(string userId, int id)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Id != id) == null;
    }

   
    public async Task<bool> IsAdminUser(string userId)
    {
        return !await DbContext.Users.AnyAsync(u 
            => u.UserId == userId  
               && u.Roles != null   
               && u.Roles.Contains("Admin")); 
    }

  
    public async Task<int> DeleteUserAsync(int id, string userId)
    {

        bool MatchesUser(User user) => user.Id == id && user.UserId == userId;

        var user = DbContext.Users.FirstOrDefault(MatchesUser);
        
        if (user == null) return 0;
        
        DbContext.Users.Remove(user);
            
        DeleteUserRelatedData(userId);
       
        return await DbContext.SaveChangesAsync();

    }
    
    
    private void DeleteUserRelatedData(string userId)
    {
        DeleteUserRelatedExercises(userId);
        DeleteUserRelatedExerciseHistories(userId);
        DeleteUserRelatedWorkouts(userId);
    }
    
    private void DeleteUserRelatedExercises(string userId)
    {
        var userExercises = DbContext.UserExercises.Where(ue => ue.UserId == userId).ToList();
        DbContext.UserExercises.RemoveRange(userExercises);
    }
    
    private void DeleteUserRelatedExerciseHistories(string userId)
    {
        var userExerciseHistories = DbContext.UserExerciseHistories.Where(ueh => ueh.UserId == userId).ToList();
        DbContext.UserExerciseHistories.RemoveRange(userExerciseHistories);
    }
    
    private void DeleteUserRelatedWorkouts(string userId)
    {
        var userWorkouts = DbContext.WorkoutSessions.Where(uw => uw.UserId == userId).ToList();
        DbContext.WorkoutSessions.RemoveRange(userWorkouts);
    }
    
    
}
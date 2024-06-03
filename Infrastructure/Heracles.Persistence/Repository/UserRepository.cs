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

    /// <summary>
    /// Retrieves a user by their user ID.
    /// </summary>
    /// <param name="userId">The user ID to retrieve the user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation with a nullable <see cref="User"/> object that matches the user ID.</returns>
    public Task<User?> GetUserByUserIdAsync(string userId)
    {
        return DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Checks if a user with the given user ID exists.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that returns true if a user with the given user ID exists; otherwise, false.</returns>
    public async Task<bool> UserIdExistsAsync(string userId)
    {
        // Check that user exists with the userId property or return false
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        
        return user != null;
    }

    /// <summary>
    /// Checks if a user with the specified user ID and ID exists in the repository asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <param name="id">The ID to exclude from the check.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that returns true if a user with the specified user ID and ID exists in the repository, otherwise false.</returns>
    public async Task<bool> UserIdWithIdExistsAsync(string userId, int id)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Id != id) == null;
    }

    /// <summary>
    /// Checks if a user is an admin based on their user ID.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation with a <see cref="bool"/> value indicating whether the user is an admin or not.</returns>
    public async Task<bool> UserIsAdmin(string userId)
    {
        return !await DbContext.Users.AnyAsync(u 
            => u.UserId == userId  // Check that the user exists
               && u.Roles != null   // Check that the user has roles
               && u.Roles.Contains("Admin")); // Check that the user has the Admin role
    }

    /// <summary>
    /// Deletes a user from the database and any related data based on the provided user ID and user ID string.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <param name="userId">The user ID string to delete the user and related data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation with the number of entities affected.</returns>
    public async Task<int> DeleteUserAsync(int id, string userId)
    {
        //delete id from the database and any other related data where the userid exist
        var user = await  DbContext.Users.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
        
        if (user == null) return 0;
        
        DbContext.Users.Remove(user);
            
        // Find every entity that has a property of UserId == userId and delete them
        var userExercises = await DbContext.UserExercises.Where(ue => ue.UserId == userId).ToListAsync();
        DbContext.UserExercises.RemoveRange(userExercises);
       
        var userExerciseHistories = await DbContext.UserExerciseHistories.Where(ueh => ueh.UserId == userId).ToListAsync();
        DbContext.UserExerciseHistories.RemoveRange(userExerciseHistories);
       
        var userWorkouts = await DbContext.WorkoutSessions.Where(uw => uw.UserId == userId).ToListAsync();
        DbContext.WorkoutSessions.RemoveRange(userWorkouts);
       
        return await DbContext.SaveChangesAsync();

    }
}
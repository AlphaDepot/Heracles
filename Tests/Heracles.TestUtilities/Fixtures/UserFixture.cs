using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Users.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

public abstract class UserFixture
{
    public static List<User> Get() => UserExerciseSeedData.Users();
    
    public static List<User> Query(QueryRequest? query, string userId, bool isAdmin)
    {
        return Fixtures.QueryWithUser(Get(), query, userId, isAdmin);
        
    }
    
}
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.SeedData;

public class DataInitializer
{
    /// <summary>
    /// Initializes the database with seed data for the Heracles application.
    /// </summary>
    /// <param name="context">The HeraclesDbContext to initialize the data.</param>
    public static void Initialize(HeraclesDbContext context)
    {
        // check dbcontext is not null
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        
        // Create or update the database
        context.Database.Migrate();
        
        /* !!! ORDER OF SEED DATA IS IMPORTANT!!! */
        
        // First load Equipment SeedData Data
        EquipmentDataLoader.Initialize(context);
        
        // Second load Exercise SeedData Data
        ExerciseDataLoader.Initialize(context);
        
        // Third load UserExercise SeedData Data
        UserExerciseDataLoader.Initialize(context);
       
        
    }
    
    

   
   
    
    
}
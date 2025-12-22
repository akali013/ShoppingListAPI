using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using ShoppingListAPI.Models;
using System;

namespace ShoppingListAPI.Helpers
{

    public class DatabaseMigrator
    {
        // Waits for the SQL Server db and creates the ShoppingList database
        public static async Task MigrateDatabaseAsync(IServiceProvider services, ILogger logger)
        {
            const int maxRetries = 10;
            const int delaySeconds = 5;

            for (int retry = 1; retry <= maxRetries; retry++)
            {
                try
                {
                    using var scope = services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ShoppingListAPIContext>();

                    logger.LogInformation("Attempting database migration (attempt {Retry}/{MaxRetries})", retry, maxRetries);

                    await db.Database.MigrateAsync();

                    logger.LogInformation("Database migration successful");
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex,
                        "Database not ready yet (attempt {Retry}/{MaxRetries}). Retrying in {Delay}s...",
                        retry, maxRetries, delaySeconds);

                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            throw new Exception("Could not connect to the database after multiple retries");
        }

        // Initialize the db with item data
        public static async Task SeedItemsAsync(ShoppingListAPIContext context)
        {
            // Only seed an empty db
            if (context.Items.Any())
            {
                return;
            }

            var path = Path.Combine(AppContext.BaseDirectory, "Data", "ItemData.csv");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Seed file not found: {path}");
            }

            var lines = await File.ReadAllLinesAsync(path);
            var items = new List<Item>();

            foreach (var line in lines.Skip(1))
            {
                var columns = line.Split(",");

                items.Add(new Item
                {
                    Id = new Guid(columns[0]),
                    Name = columns[1]
                });
            }

            context.Items.AddRange(items);
            await context.SaveChangesAsync();
        }
    }
}

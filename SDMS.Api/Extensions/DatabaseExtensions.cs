using Microsoft.EntityFrameworkCore;

namespace SDMS.Database;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using DocumentDbContext dbContext = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();


        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch
        {
            throw;
        }
    }
}
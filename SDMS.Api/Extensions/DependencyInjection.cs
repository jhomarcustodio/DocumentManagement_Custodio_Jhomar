using Microsoft.EntityFrameworkCore;
using SDMS.Database;
using SDMS.Services;

namespace SDMS.Api.Extensions;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DocumentDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("Database")
            );
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<FileHelperService>();

        return builder;
    }
}
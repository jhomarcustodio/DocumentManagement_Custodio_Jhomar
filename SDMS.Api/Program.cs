using SDMS.Api.Extensions;
using SDMS.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddControllers()
    .AddDatabase()
    .AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.ApplyMigrationsAsync();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

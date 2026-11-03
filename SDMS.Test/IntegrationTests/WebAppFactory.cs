using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace SDMS.Test.IntegrationTests;

public sealed class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres")
        .WithDatabase("sdms")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Database", _postgresContainer.GetConnectionString());
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        var a = _postgresContainer.GetConnectionString();
    }

    public async new Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
    }
}
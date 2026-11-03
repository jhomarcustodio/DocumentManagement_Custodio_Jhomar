
using Microsoft.AspNetCore.Mvc.Testing;
namespace SDMS.Test.IntegrationTests;

public abstract class IntergrationTestFixture(WebAppFactory factory): IClassFixture<WebAppFactory>
{
    public HttpClient CreateClient() => factory.CreateClient();
}
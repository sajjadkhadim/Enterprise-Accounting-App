using System.Net;
using Accounting.IntegrationTests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.IntegrationTests;

public class ApiStartupTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ApiStartupTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // [Fact]
    // public void Uses_test_database_connection_string()
    // {
    //     var configuration = _factory.Services
    //         .GetRequiredService<IConfiguration>();

    //     var connectionString =
    //         configuration.GetConnectionString("DefaultConnection");

    //     Assert.False(string.IsNullOrWhiteSpace(connectionString));

    //     Assert.Contains(
    //         "Database=enterprise_accounting_test",
    //         connectionString,
    //         StringComparison.OrdinalIgnoreCase);
    // }

    // [Fact]
    // public async Task Health_endpoint_returns_success()
    // {
    //     using var client = _factory.CreateClient();

    //     var response = await client.GetAsync("/api/health");

    //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    // }

    // [Fact]
    // public void Loads_jwt_settings_from_test_configuration()
    // {
    //     var configuration = _factory.Services
    //         .GetRequiredService<IConfiguration>();

    //     Assert.Equal(
    //         "EnterpriseAccounting.Tests",
    //         configuration["Jwt:Issuer"]);

    //     Assert.Equal(
    //         "EnterpriseAccounting.Tests",
    //         configuration["Jwt:Audience"]);

    //     Assert.False(
    //         string.IsNullOrWhiteSpace(
    //             configuration["Jwt:SecretKey"]));
    // }
}
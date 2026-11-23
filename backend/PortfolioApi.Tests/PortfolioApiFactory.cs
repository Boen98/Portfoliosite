using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PortfolioApi.Tests;

public sealed class PortfolioApiFactory : WebApplicationFactory<Program>
{
    public HttpClient MaakClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    public async Task<HttpClient> MaakIngelogdeClientAsync(string rol)
    {
        var client = MaakClient();

        try
        {
            var account = new
            {
                email = $"test-{Guid.NewGuid():N}@example.test",
                password = $"T!est1{Guid.NewGuid():N}"
            };

            using var registratie = await client.PostAsJsonAsync("/api/auth/register", account);
            Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

            if (rol != "Gast")
            {
                // Ken de rol toe in de testdatabase, voordat de gebruiker inlogt.
                await using var scope = Services.CreateAsyncScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var gebruiker = await userManager.FindByEmailAsync(account.email);
                Assert.NotNull(gebruiker);
                var resultaat = await userManager.AddToRoleAsync(gebruiker, rol);
                Assert.True(resultaat.Succeeded, $"De rol {rol} kon niet worden toegekend.");
            }

            using var login = await client.PostAsJsonAsync("/api/auth/login", account);
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
            return client;
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var connection = Environment.GetEnvironmentVariable("PORTFOLIO_TEST_CONNECTION");

        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("Stel PORTFOLIO_TEST_CONNECTION in.");
        }

        var sql = new SqlConnectionStringBuilder(connection);

        if (!string.Equals(sql.InitialCatalog, "BoenPortfolioDb_Test",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Gebruik BoenPortfolioDb_Test voor de tests.");
        }

        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PortfolioDatabase"] = sql.ConnectionString
            });
        });

        return base.CreateHost(builder);
    }
}

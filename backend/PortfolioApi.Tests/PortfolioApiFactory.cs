using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace PortfolioApi.Tests;

public sealed class PortfolioApiFactory : WebApplicationFactory<Program>
{
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
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PortfolioApi.Dtos;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioApi.Tests;

public sealed class ProjectReadTests : IClassFixture<PortfolioApiFactory>
{
    private readonly PortfolioApiFactory _factory;

    public ProjectReadTests(PortfolioApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostProject_ZonderLogin_Geeft401EnSlaatNietsOp()
    {
        // Arrange: een client zonder login en een unieke projecttitel.
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = false
        });

        var titel = $"Autorisatietest {Guid.NewGuid():N}";
        var invoer = new
        {
            titel,
            beschrijving = "Test voor een aanvraag zonder login.",
            datum = DateTime.Today,
            categorie = "Webapplicatie",
            gitHubUrl = "https://github.com/Boen98/Portfoliosite"
        };

        // Act: probeer een project toe te voegen.
        using var response = await client.PostAsJsonAsync("/api/projects", invoer);

        // Assert: de aanvraag is geweigerd en het project is niet opgeslagen.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var projecten = await client.GetFromJsonAsync<ProjectResponseDto[]>("/api/projects");
        Assert.NotNull(projecten);
        Assert.DoesNotContain(projecten, project => project.Titel == titel);
    }

    [Fact]
    public async Task PostProject_AlsGast_Geeft403EnSlaatNietsOp()
    {
        // Arrange: registreer een gast en log in met dezelfde client.
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = true
        });

        var gast = new
        {
            email = $"gast-{Guid.NewGuid():N}@example.test",
            password = $"T!est1{Guid.NewGuid():N}"
        };

        using var registratie = await client.PostAsJsonAsync("/api/auth/register", gast);
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

        using var login = await client.PostAsJsonAsync("/api/auth/login", gast);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.True(login.Headers.Contains("Set-Cookie"));

        var titel = $"Autorisatietest {Guid.NewGuid():N}";
        var invoer = new
        {
            titel,
            beschrijving = "Test voor een aanvraag van een ingelogde gast.",
            datum = DateTime.Today,
            categorie = "Webapplicatie",
            gitHubUrl = "https://github.com/Boen98/Portfoliosite"
        };

        // Act: probeer als ingelogde gast een project toe te voegen.
        using var response = await client.PostAsJsonAsync("/api/projects", invoer);

        // Assert: de gast heeft geen schrijfrechten en het project is niet opgeslagen.
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var projecten = await client.GetFromJsonAsync<ProjectResponseDto[]>("/api/projects");
        Assert.NotNull(projecten);
        Assert.DoesNotContain(projecten, project => project.Titel == titel);
    }

    [Fact]
    public async Task GetProjects_ZonderLogin_Geeft200EnJsonLijst()
    {
        // Arrange: een client zonder ingelogde gebruiker.
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = false
        });

        // Act: vraag de projecten op.
        using var response = await client.GetAsync("/api/projects");

        // Assert: controleer de status en de vorm van het antwoord.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
    }

   [Theory]
   [InlineData("Editor")]
   [InlineData("Admin")]
public async Task PostProject_MetSchrijfrechten_Geeft201EnSlaatProjectOp(string rol)
{
    // Arrange: maak een testaccount aan, ken een rol toe en log in.
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false,
        HandleCookies = true
    });

    var account = new
    {
        email = $"schrijver-{Guid.NewGuid():N}@example.test",
        password = $"T!est1{Guid.NewGuid():N}"
    };

    using var registratie = await client.PostAsJsonAsync("/api/auth/register", account);
    Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

    using (var scope = _factory.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var gebruiker = await userManager.FindByEmailAsync(account.email);
        Assert.NotNull(gebruiker);

        var resultaat = await userManager.AddToRoleAsync(gebruiker, rol);
        Assert.True(resultaat.Succeeded, $"De rol {rol} kon niet worden toegekend.");
    }

    using var login = await client.PostAsJsonAsync("/api/auth/login", account);
    Assert.Equal(HttpStatusCode.OK, login.StatusCode);

    var titel = $"Testproject {rol} {Guid.NewGuid():N}";
    var invoer = new
    {
        titel,
        beschrijving = $"Dit project wordt toegevoegd door een {rol}.",
        datum = DateTime.Today,
        categorie = "Webapplicatie",
        gitHubUrl = "https://github.com/Boen98/Portfoliosite"
    };

    // Act: voeg een project toe.
    using var response = await client.PostAsJsonAsync("/api/projects", invoer);

    // Assert: controleer het resultaat en lees het opgeslagen project terug.
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var gemaakt = await response.Content.ReadFromJsonAsync<ProjectResponseDto>();
    Assert.NotNull(gemaakt);
    Assert.True(gemaakt.Id > 0);
    Assert.Equal(titel, gemaakt.Titel);

    using var ophalen = await client.GetAsync($"/api/projects/{gemaakt.Id}");
    Assert.Equal(HttpStatusCode.OK, ophalen.StatusCode);

    var opgeslagen = await ophalen.Content.ReadFromJsonAsync<ProjectResponseDto>();
    Assert.NotNull(opgeslagen);
    Assert.Equal(gemaakt.Id, opgeslagen.Id);
    Assert.Equal(titel, opgeslagen.Titel);
}

}


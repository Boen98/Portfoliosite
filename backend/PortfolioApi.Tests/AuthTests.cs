using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PortfolioApi.Dtos;

namespace PortfolioApi.Tests;

[Collection("Portfolio database")]
public sealed class AuthTests
{
    private readonly PortfolioApiFactory _factory;

    public AuthTests(PortfolioApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_MetGeldigeInvoer_MaaktGebruikerMetAlleenGastrol()
    {
        // Arrange: maak een uniek e-mailadres en wachtwoord voor deze test.
        using var client = _factory.MaakClient();
        var invoer = MaakRegistratie();

        // Act: registreer via het publieke endpoint.
        using var registratie = await client.PostAsJsonAsync("/api/auth/register", invoer);

        // Assert: lees de opgeslagen gebruiker en de toegekende rollen terug.
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);
        await using var scope = _factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var gebruiker = await userManager.FindByEmailAsync(invoer.Email!);
        Assert.NotNull(gebruiker);
        var rol = Assert.Single(await userManager.GetRolesAsync(gebruiker));
        Assert.Equal("Gast", rol);
    }

    [Fact]
    public async Task Login_MetGeldigeGegevens_Geeft200EnAanmeldCookie()
    {
        // Arrange: registreer een account via HTTP.
        using var client = _factory.MaakClient();
        var invoer = MaakRegistratie();
        using var registratie = await client.PostAsJsonAsync("/api/auth/register", invoer);
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

        // Act: log in met dezelfde gegevens.
        using var login = await client.PostAsJsonAsync("/api/auth/login", invoer);

        // Assert: de succesvolle login geeft de Identity-aanmeldcookie terug.
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.True(HeeftAanmeldCookie(login));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Register_MetOngeldigeInvoer_Geeft400EnMaaktGeenGebruiker(
        bool geldigEmailadres, bool wachtwoordIngevuld)
    {
        // Arrange: gebruik een ongeldig e-mailadres of laat het wachtwoord leeg.
        using var client = _factory.MaakClient();
        var invoer = MaakRegistratie();
        if (!geldigEmailadres)
        {
            invoer.Email = $"ongeldig-{Guid.NewGuid():N}";
        }
        if (!wachtwoordIngevuld)
        {
            invoer.Password = "";
        }

        // Act: verstuur de ongeldige registratie.
        using var registratie = await client.PostAsJsonAsync("/api/auth/register", invoer);

        // Assert: de aanvraag is afgewezen en er is geen account opgeslagen.
        Assert.Equal(HttpStatusCode.BadRequest, registratie.StatusCode);
        Assert.False(HeeftAanmeldCookie(registratie));
        await using var scope = _factory.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        Assert.Null(await userManager.FindByEmailAsync(invoer.Email!));
    }

    [Fact]
    public async Task Login_ZonderWachtwoord_Geeft400ZonderAanmeldCookie()
    {
        // Arrange: laat het wachtwoord weg.
        using var client = _factory.MaakClient();
        var invoer = new LoginDto { Email = MaakRegistratie().Email };

        // Act: verstuur de onvolledige aanvraag.
        using var login = await client.PostAsJsonAsync("/api/auth/login", invoer);

        // Assert: er is geen sessie aangemaakt.
        Assert.Equal(HttpStatusCode.BadRequest, login.StatusCode);
        Assert.False(HeeftAanmeldCookie(login));
    }

    [Fact]
    public async Task Login_MetVerkeerdWachtwoord_Geeft401ZonderAanmeldCookie()
    {
        // Arrange: registreer een account en maak een ander wachtwoord voor de login.
        using var client = _factory.MaakClient();
        var account = MaakRegistratie();
        using var registratie = await client.PostAsJsonAsync("/api/auth/register", account);
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);
        var invoer = new LoginDto { Email = account.Email, Password = MaakWachtwoord() };

        // Act: log in met het verkeerde wachtwoord.
        using var login = await client.PostAsJsonAsync("/api/auth/login", invoer);

        // Assert: de login is geweigerd en geeft geen aanmeldcookie terug.
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        Assert.False(HeeftAanmeldCookie(login));
    }

    private bool HeeftAanmeldCookie(HttpResponseMessage response)
    {
        var cookieOpties = _factory.Services
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(IdentityConstants.ApplicationScheme);
        var cookieNaam = cookieOpties.Cookie.Name;
        return response.Headers.TryGetValues("Set-Cookie", out var cookies)
            && cookies.Any(cookie => cookie.StartsWith($"{cookieNaam}=", StringComparison.Ordinal));
    }

    private static RegistratieDto MaakRegistratie() => new()
    {
        Email = $"auth-{Guid.NewGuid():N}@example.test",
        Password = MaakWachtwoord()
    };

    private static string MaakWachtwoord() => $"T!est1{Guid.NewGuid():N}";
}

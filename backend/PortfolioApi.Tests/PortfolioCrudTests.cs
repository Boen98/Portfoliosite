using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PortfolioApi.Tests;

[Collection("Portfolio database")]
public sealed class PortfolioCrudTests
{
    private const string Projecten = "/api/projects";
    private const string Blogposts = "/api/blogposts";
    private readonly PortfolioApiFactory _factory;

    public PortfolioCrudTests(PortfolioApiFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData(Projecten, "Editor")]
    [InlineData(Projecten, "Admin")]
    [InlineData(Blogposts, "Editor")]
    [InlineData(Blogposts, "Admin")]
    public async Task Crud_MetSchrijfrechten_MaaktWijzigtEnVerwijdertItem(string route, string rol)
    {
        // Arrange
        using var schrijver = await _factory.MaakIngelogdeClientAsync(rol);
        using var lezer = _factory.MaakClient();
        var invoer = MaakInvoer(route);
        var wijziging = MaakInvoer(route, bijgewerkt: true);

        // Act en Assert: POST levert een id en Location op. Beide GET-routes zijn openbaar.
        using var post = await schrijver.PostAsJsonAsync(route, invoer);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var aangemaakt = await post.Content.ReadFromJsonAsync<JsonElement>();
        var id = aangemaakt.GetProperty("id").GetInt32();
        Assert.True(id > 0);
        var itemRoute = $"{route}/{id}";
        Assert.Equal(itemRoute, post.Headers.Location?.OriginalString);
        ControleerVelden(aangemaakt, invoer);

        var opgehaald = await HaalItemOpAsync(lezer, itemRoute);
        Assert.Equal(id, opgehaald.GetProperty("id").GetInt32());
        ControleerVelden(opgehaald, invoer);

        var lijst = await lezer.GetFromJsonAsync<JsonElement[]>(route);
        Assert.NotNull(lijst);
        var opgenomenItem = Assert.Single(lijst, item => item.GetProperty("id").GetInt32() == id);
        ControleerVelden(opgenomenItem, invoer);

        // Act en Assert: na PUT zijn de gewijzigde waarden opgeslagen onder hetzelfde id.
        using var put = await schrijver.PutAsJsonAsync(itemRoute, wijziging);
        Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);
        var bijgewerkt = await HaalItemOpAsync(lezer, itemRoute);
        Assert.Equal(id, bijgewerkt.GetProperty("id").GetInt32());
        ControleerVelden(bijgewerkt, wijziging);

        // Act en Assert: na DELETE is het id niet meer opvraagbaar of beschrijfbaar.
        using var delete = await schrijver.DeleteAsync(itemRoute);
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        using var getNaDelete = await lezer.GetAsync(itemRoute);
        Assert.Equal(HttpStatusCode.NotFound, getNaDelete.StatusCode);
        using var putNaDelete = await schrijver.PutAsJsonAsync(itemRoute, wijziging);
        Assert.Equal(HttpStatusCode.NotFound, putNaDelete.StatusCode);
        using var tweedeDelete = await schrijver.DeleteAsync(itemRoute);
        Assert.Equal(HttpStatusCode.NotFound, tweedeDelete.StatusCode);
    }

    [Theory]
    [InlineData(Projecten, "PUT")]
    [InlineData(Projecten, "DELETE")]
    [InlineData(Blogposts, "POST")]
    [InlineData(Blogposts, "PUT")]
    [InlineData(Blogposts, "DELETE")]
    public async Task Schrijfverzoek_ZonderSchrijfrechten_Geeft401Of403EnWijzigtNiets(
        string route, string methode)
    {
        // Arrange: PUT en DELETE richten zich op een bestaand item.
        using var anoniem = _factory.MaakClient();
        using var gast = await _factory.MaakIngelogdeClientAsync("Gast");
        var oorspronkelijk = MaakInvoer(route);
        var poging = MaakInvoer(route, bijgewerkt: true);
        var doelRoute = route;

        if (methode != "POST")
        {
            using var editor = await _factory.MaakIngelogdeClientAsync("Editor");
            doelRoute = await MaakItemAanAsync(editor, route, oorspronkelijk);
        }

        // Act
        using var zonderLogin = await VerstuurSchrijfverzoekAsync(anoniem, methode, doelRoute, poging);
        using var alsGast = await VerstuurSchrijfverzoekAsync(gast, methode, doelRoute, poging);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, zonderLogin.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, alsGast.StatusCode);

        if (methode == "POST")
        {
            await ControleerTitelNietOpgeslagenAsync(anoniem, route, (string)poging["titel"]);
        }
        else
        {
            var item = await HaalItemOpAsync(anoniem, doelRoute);
            ControleerVelden(item, oorspronkelijk);
        }
    }

    [Theory]
    [InlineData(Projecten, "POST")]
    [InlineData(Projecten, "PUT")]
    [InlineData(Blogposts, "POST")]
    [InlineData(Blogposts, "PUT")]
    public async Task Schrijfverzoek_MetOngeldigeDatum_Geeft400EnWijzigtNiets(
        string route, string methode)
    {
        // Arrange: de Editor mag schrijven, maar de JSON-waarde is geen datum.
        using var editor = await _factory.MaakIngelogdeClientAsync("Editor");
        using var lezer = _factory.MaakClient();
        var oorspronkelijk = MaakInvoer(route);
        var invoer = MaakInvoer(route, bijgewerkt: true);
        invoer[route == Projecten ? "datum" : "publicatieDatum"] = "geen datum";
        var doelRoute = methode == "POST"
            ? route
            : await MaakItemAanAsync(editor, route, oorspronkelijk);

        // Act
        using var response = await VerstuurSchrijfverzoekAsync(editor, methode, doelRoute, invoer);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        if (methode == "POST")
        {
            await ControleerTitelNietOpgeslagenAsync(lezer, route, (string)invoer["titel"]);
        }
        else
        {
            var item = await HaalItemOpAsync(lezer, doelRoute);
            ControleerVelden(item, oorspronkelijk);
        }
    }

    private static Dictionary<string, object> MaakInvoer(string route, bool bijgewerkt = false)
    {
        var label = bijgewerkt ? "Bijgewerkt" : "Oorspronkelijk";
        var datum = bijgewerkt ? new DateTime(2026, 11, 2) : new DateTime(2026, 11, 1);
        var titel = $"{label} {Guid.NewGuid():N}";

        if (route == Projecten)
        {
            return new Dictionary<string, object>
            {
                ["titel"] = titel,
                ["beschrijving"] = $"{label} project",
                ["datum"] = datum,
                ["categorie"] = bijgewerkt ? "Webapplicatie" : "Oefening",
                ["gitHubUrl"] = $"https://github.com/example/portfolio-{label.ToLowerInvariant()}"
            };
        }

        return new Dictionary<string, object>
        {
            ["titel"] = titel,
            ["inhoud"] = $"{label} blogbericht",
            ["publicatieDatum"] = datum
        };
    }

    private static void ControleerVelden(JsonElement item, Dictionary<string, object> invoer)
    {
        foreach (var veld in JsonSerializer.SerializeToElement(invoer).EnumerateObject())
        {
            Assert.True(item.TryGetProperty(veld.Name, out var waarde), $"Veld ontbreekt: {veld.Name}.");
            Assert.True(JsonElement.DeepEquals(veld.Value, waarde), $"Onjuiste waarde voor {veld.Name}.");
        }
    }

    private static async Task<string> MaakItemAanAsync(
        HttpClient client, string route, Dictionary<string, object> invoer)
    {
        using var response = await client.PostAsJsonAsync(route, invoer);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var item = await response.Content.ReadFromJsonAsync<JsonElement>();
        return $"{route}/{item.GetProperty("id").GetInt32()}";
    }

    private static async Task<JsonElement> HaalItemOpAsync(HttpClient client, string route)
    {
        using var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static async Task ControleerTitelNietOpgeslagenAsync(HttpClient client, string route, string titel)
    {
        var lijst = await client.GetFromJsonAsync<JsonElement[]>(route);
        Assert.NotNull(lijst);
        Assert.DoesNotContain(lijst, item => item.GetProperty("titel").GetString() == titel);
    }

    private static Task<HttpResponseMessage> VerstuurSchrijfverzoekAsync(
        HttpClient client, string methode, string route, Dictionary<string, object> invoer)
    {
        return methode switch
        {
            "POST" => client.PostAsJsonAsync(route, invoer),
            "PUT" => client.PutAsJsonAsync(route, invoer),
            "DELETE" => client.DeleteAsync(route),
            _ => throw new ArgumentOutOfRangeException(nameof(methode))
        };
    }
}

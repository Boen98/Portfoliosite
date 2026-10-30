Portfolio van Boen

## Technieken

- .NET 10 en ASP.NET Core Minimal API
- Entity Framework Core
- SQL Server LocalDB
- Swagger UI via Swashbuckle.AspNetCore

## Codestructuur

De backend staat in `backend/PortfolioApi`.

- `Program.cs`: configuratie, dependency injection en registratie van endpoints.
- `Endpoints`: aparte routegroepen voor projecten en blogposts; verwerkt requests en bepaalt HTTP-responses.
- `Services`: voert de gegevensbewerkingen uit en zet gegevens om tussen DTO's en entiteiten.
- `Dtos`: invoer voor POST en PUT en uitvoer voor GET en POST.
- `Entities`: de modellen Project en Blogpost.
- `Data`: PortfolioDbContext voor de datatoegang via EF Core.
- `Migrations`: wijzigingen van de databasestructuur.

De endpoints ontvangen en retourneren DTO's. De DbContext en services worden via dependency injection aangeboden.

## Benodigdheden

- Windows met SQL Server Express LocalDB, instantie `MSSQLLocalDB`.
- .NET 10 SDK.
- dotnet-ef. Als deze tool nog niet is geïnstalleerd:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

## Database configureren

De verbinding staat onder `ConnectionStrings` in `backend/PortfolioApi/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "PortfolioDatabase": "Server=(localdb)\\MSSQLLocalDB;Database=BoenPortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Dit onderdeel staat binnen het bestaande JSON-hoofdobject. De database heet `BoenPortfolioDb`.

Ik gebruik EF Core migrations om de database en tabellen aan te maken. De bestaande migration wordt toegepast met het onderstaande commando.

## Backend starten

Voer deze commando's uit vanuit de hoofdmap van de repository:

```powershell
cd backend/PortfolioApi
dotnet restore
dotnet ef database update -- --environment Development
dotnet run
```

Open daarna [Swagger UI](http://localhost:5168/swagger). Als de terminal een andere poort toont, gebruik dan die poort. Swagger is beschikbaar in de Development-omgeving.

## Endpoints en testen

Voor zowel `/api/projects` als `/api/blogposts` zijn er vijf endpoints:

- GET op het basisadres: alle items ophalen.
- GET met `/{id}`: één item ophalen.
- POST op het basisadres: een item aanmaken.
- PUT met `/{id}`: een item wijzigen.
- DELETE met `/{id}`: een item verwijderen.

Test de endpoints via **Try it out** en **Execute** in Swagger. GET retourneert 200, POST retourneert 201 en een geslaagde PUT of DELETE retourneert 204. Een geldig maar niet-bestaand Id geeft bij GET op Id, PUT en DELETE een 404.

De gegevens worden opgeslagen in SQL Server en blijven na het stoppen en opnieuw starten van de API bestaan.

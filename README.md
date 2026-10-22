# Portfolio van Boen

De bestaande HTML-, CSS- en JavaScript-bestanden vormen de frontend van mijn
portfolio. Voor WPFW opdracht 3 staat de start van de backend in
`backend/Portfolio.Api`.

## Huidige status

- Eén ASP.NET Core Web API-project met doelframework **.NET 10** (`net10.0`).
- Voorlopige API-stijl: **controllers**, met mappen per laag.
- Swagger UI via **Swashbuckle.AspNetCore** in de ontwikkelomgeving.
- De mappen voor controllers, services, data, entiteiten en DTO's zijn nog leeg.
- EF Core, de databaseverbinding en de tien CRUD-endpoints zijn nog niet geïmplementeerd.

De startopzet is bedoeld om eerst het project en Swagger werkend te krijgen.
De bestaande frontend gebruikt deze backend nog niet.

## Lokaal starten

Installeer de .NET 10 SDK. Voer vanaf de hoofdmap uit:

```powershell
dotnet run --project backend/Portfolio.Api
```

Of voer in `backend/Portfolio.Api` gewoon `dotnet run` uit.
De eerste keer worden de NuGet-pakketten hersteld; daarvoor is internet nodig.
Het standaardprofiel `http` gebruikt `Development` en poort 5080.

Open **http://localhost:5080/swagger**. Zolang er geen controllers met acties
zijn, is de lijst met endpoints leeg. Dat is in deze startfase verwacht.
De URL `/` heeft nog geen pagina en kan 404 teruggeven.
Stop de applicatie met `Ctrl+C`.

## Geplande structuur

```text
backend/Portfolio.Api/
  Controllers/  HTTP-routes en statuscodes
  Services/     Applicatielogica en omzetting tussen entiteiten en DTO's
  Data/         Eigen DbContext voor EF Core
  Models/       Project en Blogpost als database-entiteiten
  DTOs/         Invoer voor POST/PUT en uitvoer voor GET
  Program.cs    Registratie van services en HTTP-middleware
```

De keuze voor controllers en deze lagen is een startvoorstel; de student
controleert en onderbouwt de uiteindelijke keuze zelf.

## SQL Server en database aanmaken in de volgende fase

De opdracht vereist SQL Server. Op de ontwikkelcomputer is een draaiende
`SQLEXPRESS`-service aangetroffen. De verbinding en rechten zijn nog niet getest.

Voeg in de volgende fase de EF Core SQL Server- en Design-pakketten van versie
10.x toe. Configureer vervolgens een connectionstring onder
`ConnectionStrings:PortfolioDatabase` in `appsettings.Development.json`, bijvoorbeeld:

```json
{
  "ConnectionStrings": {
    "PortfolioDatabase": "Server=.\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Dit voorbeeld gebruikt Windows-authenticatie. Pas de servernaam en authenticatie
aan je eigen SQL Server aan. Registreer daarna de eigen DbContext en services
via dependency injection in `Program.cs`.

De voorgestelde aanpak is **migraties**. Zodra entiteiten, DbContext en de EF CLI
van versie 10.x zijn ingesteld, voer je in `backend/Portfolio.Api` uit:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Deze commando's werken pas na die implementatie. Er is in de startopzet nog geen
database aangemaakt. Gebruik geen InMemory-database. Werk dit onderdeel bij
zodra de database daadwerkelijk is geïmplementeerd en getest.

## Planning en proces

- [中文起步指南](docs/opdracht-3-start.md)
- [Proceslog om zelf aan te vullen](docs/opdracht-3-proces.md)

Minimaal zes echte commits verspreid over minimaal drie weken zijn vereist.
Bewaar later voor beide typen de Swagger-bewijzen van POST, GET alle, GET op id,
PUT, GET na herstart, DELETE en GET met 404. Lever de uiteindelijke PDF in
Brightspace en de procesverantwoording in Boulder in.

## Bronnen

- [Microsoft Learn — Controller-based Web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0)
  (gebruik voor deze opdracht SQL Server in plaats van de InMemory-database uit de tutorial).
- [Microsoft Learn — Swashbuckle configureren](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0)
  (de opdracht schrijft Swashbuckle ook bij .NET 10 expliciet voor).
- [Microsoft Learn — SQL Server-provider voor EF Core](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)
- [Microsoft Learn — EF Core-migraties](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

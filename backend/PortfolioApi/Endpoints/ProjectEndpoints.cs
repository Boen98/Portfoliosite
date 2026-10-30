using Microsoft.AspNetCore.Http.HttpResults;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var projectRoutes = app.MapGroup("/api/projects")
            .WithTags("Projecten");

        // Alle projecten ophalen.
        projectRoutes.MapGet("/", async (ProjectService service) =>
        {
            var projecten = await service.HaalAlleOpAsync();

            return TypedResults.Ok(projecten);
        });

        // Eén project ophalen op basis van het id.
        projectRoutes.MapGet("/{id:int}",
            async Task<Results<Ok<ProjectResponseDto>, NotFound>>
            (int id, ProjectService service) =>
        {
            var project = await service.HaalOpIdAsync(id);

            if (project is not null)
            {
                return TypedResults.Ok(project);
            }
            else
            {
                return TypedResults.NotFound();
            }
        });

        // Een nieuw project aanmaken.
        projectRoutes.MapPost("/",
            async (ProjectCreateDto invoer, ProjectService service) =>
        {
            var project = await service.MaakAanAsync(invoer);

            return TypedResults.Created(
                $"/api/projects/{project.Id}", project);
        });

        // Een bestaand project wijzigen.
        projectRoutes.MapPut("/{id:int}",
            async Task<Results<NoContent, NotFound>>
            (int id, ProjectUpdateDto invoer, ProjectService service) =>
        {
            var bijgewerkt = await service.WerkBijAsync(id, invoer);

            if (bijgewerkt)
            {
                return TypedResults.NoContent();
            }
            else
            {
                return TypedResults.NotFound();
            }
        });

        // Een bestaand project verwijderen.
        projectRoutes.MapDelete("/{id:int}",
            async Task<Results<NoContent, NotFound>>
            (int id, ProjectService service) =>
        {
            var verwijderd = await service.VerwijderAsync(id);

            if (verwijderd)
            {
                return TypedResults.NoContent();
            }
            else
            {
                return TypedResults.NotFound();
            }
        });
    }
}
using Microsoft.AspNetCore.Http.HttpResults;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Endpoints;
public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this WebApplication app)
    {
        var blogpostRoutes = app.MapGroup("/api/blogposts")
            .WithTags("Blogposts");

        // Alle blogposts ophalen.
        blogpostRoutes.MapGet("/", async (BlogpostService service) =>
        {
            var blogposts = await service.HaalAlleOpAsync();

            return TypedResults.Ok(blogposts);
        });

        blogpostRoutes.MapGet("/{id:int}",
            async Task<Results<Ok<BlogpostResponseDto>, NotFound>>
            (int id, BlogpostService service) =>
        {
            var blogpost = await service.HaalOpIdAsync(id);

            if (blogpost is not null)
            {
                return TypedResults.Ok(blogpost);
            }
            else
            {
                return TypedResults.NotFound();
            }
        });

        // Een nieuwe blogpost aanmaken.
        blogpostRoutes.MapPost("/",
            async (BlogpostCreateDto invoer, BlogpostService service) =>
        {
            var blogpost = await service.MaakAanAsync(invoer);

            return TypedResults.Created(
                $"/api/blogposts/{blogpost.Id}", blogpost);
        });

        blogpostRoutes.MapPut("/{id:int}",
            async Task<Results<NoContent, NotFound>>
            (int id, BlogpostUpdateDto invoer, BlogpostService service) =>
        {
            var bijgewerkt = await service.WerkBijAsync(id, invoer, service);

            if (bijgewerkt)
            {
                return TypedResults.NoContent();
            }
            else
            {
                return TypedResults.NotFound();
            }
        });

        blogpostRoutes.MapDelete("/{id:int}",
            async Task<Results<NoContent, NotFound>>
            (int id, BlogpostService service) =>
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
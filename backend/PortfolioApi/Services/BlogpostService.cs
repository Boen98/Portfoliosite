using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos;
using PortfolioApi.Entities;

namespace PortfolioApi.Services;

public class BlogpostService
{
    private readonly PortfolioDbContext _dbContext;

    public BlogpostService(PortfolioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BlogpostResponseDto>> HaalAlleOpAsync()
    {
        return await _dbContext.Blogposts
            .Select(blogpost => new BlogpostResponseDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                PublicatieDatum = blogpost.PublicatieDatum
            })
            .ToListAsync();
    }
public async Task<BlogpostResponseDto> MaakAanAsync(
    BlogpostCreateDto invoer)
{
    var blogpost = new Blogpost
    {
        Titel = invoer.Titel,
        Inhoud = invoer.Inhoud,
        PublicatieDatum = invoer.PublicatieDatum
    };

    _dbContext.Blogposts.Add(blogpost);
    await _dbContext.SaveChangesAsync();

    return new BlogpostResponseDto
    {
        Id = blogpost.Id,
        Titel = blogpost.Titel,
        Inhoud = blogpost.Inhoud,
        PublicatieDatum = blogpost.PublicatieDatum
    };
}
public async Task<BlogpostResponseDto?> HaalOpIdAsync(int id)
{
    return await _dbContext.Blogposts
        .Where(blogpost => blogpost.Id == id)
        .Select(blogpost => new BlogpostResponseDto
        {
            Id = blogpost.Id,
            Titel = blogpost.Titel,
            Inhoud = blogpost.Inhoud,
            PublicatieDatum = blogpost.PublicatieDatum
        })
        .FirstOrDefaultAsync();
}


public async Task<bool> WerkBijAsync(
    int id, BlogpostUpdateDto invoer, BlogpostService service)
{
    var blogpost = await _dbContext.Blogposts.FindAsync(id);

    if (blogpost is null)
    {
        return false;
    }

    blogpost.Titel = invoer.Titel;
    blogpost.Inhoud = invoer.Inhoud;
    blogpost.PublicatieDatum = invoer.PublicatieDatum;

    await _dbContext.SaveChangesAsync();

    return true;
}

public async Task<bool> VerwijderAsync(int id)
{
    var blogpost = await _dbContext.Blogposts.FindAsync(id);

    if (blogpost is null)
    {
        return false;
    }

    _dbContext.Blogposts.Remove(blogpost);
    await _dbContext.SaveChangesAsync();

    return true;

}

}
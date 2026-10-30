using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public class ProjectService
{
    private readonly PortfolioDbContext _dbContext;

    public ProjectService(PortfolioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProjectResponseDto> MaakAanAsync(ProjectCreateDto invoer)
    {
        var project = new Entities.Project
        {
            Titel = invoer.Titel,
            Beschrijving = invoer.Beschrijving,
            Datum = invoer.Datum,
            Categorie = invoer.Categorie,
            GitHubUrl = invoer.GitHubUrl
        };

        _dbContext.Projecten.Add(project);
        await _dbContext.SaveChangesAsync();

        return new ProjectResponseDto
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Datum = project.Datum,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl
        };
    }

    public async Task<List<ProjectResponseDto>> HaalAlleOpAsync()
    {
        // Select zet de databasegegevens om naar de velden van de uitvoer-DTO.
        return await _dbContext.Projecten
            .Select(project => new ProjectResponseDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Datum = project.Datum,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl
            })
            .ToListAsync();
    }

    public async Task<ProjectResponseDto?> HaalOpIdAsync(int id)
{
    return await _dbContext.Projecten
        .Where(project => project.Id == id)
        .Select(project => new ProjectResponseDto
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Datum = project.Datum,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl
        })
        .FirstOrDefaultAsync();
    }
    public async Task<bool> WerkBijAsync(int id, ProjectUpdateDto invoer)
{
    var project = await _dbContext.Projecten.FindAsync(id);

    if (project is null)
    {
        return false;
    }

    project.Titel = invoer.Titel;
    project.Beschrijving = invoer.Beschrijving;
    project.Datum = invoer.Datum;
    project.Categorie = invoer.Categorie;
    project.GitHubUrl = invoer.GitHubUrl;

    await _dbContext.SaveChangesAsync();

    return true;
}

public async Task<bool> VerwijderAsync(int id)
{
    var project = await _dbContext.Projecten.FindAsync(id);

    if (project is null)
    {
        return false;
    }

    _dbContext.Projecten.Remove(project);
    await _dbContext.SaveChangesAsync();

    return true;
}

}
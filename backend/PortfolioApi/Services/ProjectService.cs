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
}
namespace PortfolioApi.Dtos;

public class ProjectResponseDto
{
    public int Id { get; set; }

    public string Titel { get; set; } = "";

    public string Beschrijving { get; set; } = "";

    public DateTime Datum { get; set; }

    public string Categorie { get; set; } = "";

    public string GitHubUrl { get; set; } = "";
}
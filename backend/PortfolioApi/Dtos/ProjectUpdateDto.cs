namespace PortfolioApi.Dtos;
public class ProjectUpdateDto
{
    public string Titel { get; set; } = "";

    public string Beschrijving { get; set; } = "";

    public DateTime Datum { get; set; }

    public string Categorie { get; set; } = "";

    public string GitHubUrl { get; set; } = "";
}
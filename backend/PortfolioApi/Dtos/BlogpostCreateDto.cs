namespace PortfolioApi.Dtos;
public class BlogpostCreateDto
{
    public string Titel { get; set; } = null!;
    public string Inhoud { get; set; } = null!;
    public DateTime PublicatieDatum { get; set; } 
}
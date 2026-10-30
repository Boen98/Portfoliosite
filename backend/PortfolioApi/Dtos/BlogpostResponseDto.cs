namespace PortfolioApi.Dtos;
public class BlogpostResponseDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = null!;
    public string Inhoud { get; set; } = null!;
    public DateTime PublicatieDatum { get; set; } 
}
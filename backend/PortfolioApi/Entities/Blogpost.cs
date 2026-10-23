public class Blogpost
{
    public int Id { get; set; }

    public string Titel { get; set; } = "";

    public string Inhoud { get; set; } = "";

    public DateTime PublicatieDatum { get; set; }
    
}dotnet ef migrations add InitialCreate -- --environment Development
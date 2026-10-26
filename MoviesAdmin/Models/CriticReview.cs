namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        int Id { get; set; }
        string Title { get; set; } = string.Empty;
        string Description { get; set; } = string.Empty;
        int Rating { get; set; }
        Boolean IsPublished { get; set; }
        string CreatedBy { get; set; } = string.Empty;
        DateOnly CreatedDate { get; set; }
    }
}

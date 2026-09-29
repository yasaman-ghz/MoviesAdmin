namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty; // e.g., PG, PG-13, R
        public string ImageFilename { get; set; } = string.Empty; // Path to the image used for the movie poster
        public int Runtime { get; set; } // Duration in minutes
        public DateOnly ReleaseDate { get; set; } // Only the date of the release
    }
}

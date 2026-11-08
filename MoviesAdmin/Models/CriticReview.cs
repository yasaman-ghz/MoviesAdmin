using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        public int Id { get; set; } // primary key
        public int MovieId { get; set; } // foreign key to Movie

        [ValidateNever]
        public Movie Movie { get; set; } = null!;// navigation property to Movie
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; }
        public  Boolean IsPublished { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateOnly CreatedDate { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        public int Id { get; set; } // primary key
        public int MovieId { get; set; } // foreign key to Movie

        [ValidateNever]
        public Movie Movie { get; set; } = null!;// navigation property to Movie
        public string Title { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Maximum 100 characters accepted.")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Please enter a Rating from 0 to 5.")]
        public int Rating { get; set; }
        public  Boolean IsPublished { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateOnly CreatedDate { get; set; }
    } 
}

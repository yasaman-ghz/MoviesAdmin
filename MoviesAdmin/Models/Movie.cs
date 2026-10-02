using System.ComponentModel.DataAnnotations;


namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; }                                     //Data Annotation:
                                                                        //Reference: https://learn.microsoft.com/en-us/previous-versions/aspnet/ee256141(v=vs.100)?redirectedfrom=MSDN
        [Required, StringLength(50)]                                    //Reference: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-6
        public string Title { get; set; } = string.Empty;               //Regular Expressions:
                                                                        //Reference: https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions
        [Required, StringLength(50)]                                    //Reference: https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions
        [RegularExpression(@"^[^,\s]+$", ErrorMessage = "Please use one word and no spaces or commas.")]
        public string Genre { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Synopsis { get; set; } = string.Empty;

        [Required]                                                      
        [RegularExpression(@"^(?i)(G|PG|PG-13|R|NC-17)$", ErrorMessage = "Please enter a valid movie rating (G, PG, PG-13, R, NC-17).")]
        public string Rating { get; set; } = string.Empty; // e.g., PG, PG-13, R

        [Required, Display(Name = "Cover image", Description = "Image URL"), StringLength(300)]
        public string ImageFilename { get; set; } = string.Empty;

        [Required, Display(Name = "Runtime (minutes)")]
        [Range(1, 300, ErrorMessage = "Please enter a runtime between 1 and 300 minutes.")]
        public int Runtime { get; set; } // Duration in minutes

        [Required]
        [Display(Name = "Release Date")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly ReleaseDate { get; set; } // Only the date of the release
    }
}

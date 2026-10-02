using System.ComponentModel.DataAnnotations;


namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; }                                     //Data Annotation References:
                                                                        // https://learn.microsoft.com/en-us/previous-versions/aspnet/ee256141(v=vs.100)?redirectedfrom=MSDN
        [Required, StringLength(50)]                                    // https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-6
                                                                        // https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute.prompt?view=net-10.0
        public string Title { get; set; } = string.Empty;               //Regular Expressions References:
                                                                        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions
        [Required, StringLength(50)]                                    // https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions
        [RegularExpression(@"^[^,\s]+$", ErrorMessage = "Please only one genre and no spaces or commas.")]
        public string Genre { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Synopsis { get; set; } = string.Empty;

        [Required, Display(Prompt ="e.g. PG")]                                                      
        [RegularExpression(@"^(?i)(G|PG|PG-13|R|NC-17)$", ErrorMessage = "Please enter a valid movie rating (G, PG, PG-13, R, NC-17).")]
        public string Rating { get; set; } = string.Empty; // e.g., PG, PG-13, R

        [Required, Display(Name = "Cover image", Prompt = "e.g. movie-name.jpg"), StringLength(60)]
        public string ImageFilename { get; set; } = string.Empty; 
         
        [Required, Display(Name = "Runtime (minutes)")]
        [Range(1, 300, ErrorMessage = "Please enter a runtime between 1 and 300 minutes.")]
        public int Runtime { get; set; } // Duration in minutes

        [Required, Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]   //Reference: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/complex-data-model?view=aspnetcore-10.0
        public DateOnly ReleaseDate { get; set; } // Only the date of the release            //Reference: https://stackoverflow.com/questions/7372038/is-there-any-way-to-change-input-type-date-format
    }
}

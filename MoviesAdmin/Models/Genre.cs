using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MoviesAdmin.Models
{
    public class Genre
    {
        public int Id { get; set; } //primary key
        [ValidateNever]
        public List<Movie> Movies { get; set; } = new List<Movie>(); //navigation property

        [Required]
        public string Title { get; set; } = string.Empty;
    }
}

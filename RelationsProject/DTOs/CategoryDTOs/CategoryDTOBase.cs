using System.ComponentModel.DataAnnotations;

namespace RelationsProject.DTOs.CategoryDTOs
{
    public abstract record CategoryDTOBase
    {
        [Required]
        [MinLength(2, ErrorMessage = "Min. lenght must be 2")]
        [MaxLength(250, ErrorMessage = "Max. lenght must be 250")]
        public string CategoryName { get; init; }
    }
}
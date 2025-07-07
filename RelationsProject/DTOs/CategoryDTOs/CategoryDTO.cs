using RelationsProject.Entities;
using System.Text.Json.Serialization;

namespace RelationsProject.DTOs.CategoryDTOs
{
    public record CategoryDTO : CategoryDTOBase
    {
        public int CategoryId { get; init; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; }
    }
}
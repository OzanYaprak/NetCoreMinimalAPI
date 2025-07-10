using IdentityProject.Entities;
using System.Text.Json.Serialization;

namespace IdentityProject.DTOs.CategoryDTOs
{
    public record CategoryDTO : CategoryDTOBase
    {
        public int CategoryId { get; init; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; }
    }
}
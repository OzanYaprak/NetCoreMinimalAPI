using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityProject.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; }
    }
}
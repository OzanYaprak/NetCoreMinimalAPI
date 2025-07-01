using System.ComponentModel.DataAnnotations;

namespace RelationsProject.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public decimal Price { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }

        #region Constructor

        public Book()
        {
            URL = "/images/default.jpg";
        }

        #endregion Constructor

        #region Navigation Properties

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        #endregion Navigation Properties
    }
}
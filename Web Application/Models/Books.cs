using System.ComponentModel.DataAnnotations;

namespace Web_Application.Models
{
    public class Books
    {
        [Key]
        public int BookId { get; set; }
        public required string ISBN { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Genre { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfPublishing { get; set; }
    }
}

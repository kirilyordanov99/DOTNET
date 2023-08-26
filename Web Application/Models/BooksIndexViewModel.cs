namespace Web_Application.Models
{
    public class BooksIndexViewModel
    {
        public required List<Books> Books { get; set; }
        public required string SearchQuery { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}

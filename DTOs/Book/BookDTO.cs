namespace BooksManagementSrv.DTOs.Book
{
    public class BookDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required AuthorDTO Author { get; set; }
        public required Genre Genre { get; set; }
        public required string Description { get; set; }
        public double Price { get; set; }
    }
}
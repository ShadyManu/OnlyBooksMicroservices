namespace BooksManagementSrv.DTOs.Book
{
    public class CreateBookDTO
    {
        public required string Title { get; set; }
        public required Genre Genre { get; set; }
        public required string Description { get; set; }
        public required double Price { get; set; }
    }
}
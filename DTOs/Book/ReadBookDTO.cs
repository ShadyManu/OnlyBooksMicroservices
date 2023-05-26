

namespace BooksManagementSrv.DTOs.Book
{
    public class ReadBookDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required ReadAuthorDTO Author { get; set; }
        public required string Genre { get; set; }
        public required string Description { get; set; }
        public required double Price { get; set; }
    }
}
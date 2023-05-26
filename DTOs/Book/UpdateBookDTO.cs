namespace BooksManagementSrv.DTOs.Book
{
    public class UpdateBookDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public Genre? Genre { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
    }
}
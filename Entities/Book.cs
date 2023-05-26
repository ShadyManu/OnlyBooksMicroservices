namespace BooksManagementSrv.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int AuthorId { get; set; }
        public required Author Author { get; set; }
        public required Genre Genre { get; set; }
        public required string Description { get; set; }
        public required double Price { get; set; }
    }
}
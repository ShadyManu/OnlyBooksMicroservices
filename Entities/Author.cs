namespace AuthSrv.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public string? Address { get; set; }
        public Genre? FavouriteGenre { get; set; }
        //public List<Book>? MyBooks { get; set; }
        
    }
}
namespace AuthSrv.DTOs.UserDetails
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public string? Address { get; set; }
        public Genre? FavouriteGenre { get; set; }
    }
}
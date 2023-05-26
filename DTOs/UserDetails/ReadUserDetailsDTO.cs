namespace AuthSrv.DTOs.UserDetails
{
    public class ReadUserDetailsDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public string? Address { get; set; }
        public string? FavouriteGenre { get; set; }
    }
}
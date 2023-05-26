namespace AuthSrv.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; } 
        public byte[]? PasswordSalt { get; set; }
        public Usertype Role { get; set; }
        public UserDetails? UserDetails { get; set; }
        public int UserDetailsId { get; set; }
    }
}
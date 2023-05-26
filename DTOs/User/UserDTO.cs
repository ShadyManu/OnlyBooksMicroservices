namespace AuthSrv.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; } 
        public byte[]? PasswordSalt { get; set; }
        public Usertype Role { get; set; } 
        public UserDetailsDTO? UserDetails { get; set; }
    }
}
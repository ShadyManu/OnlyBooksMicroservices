namespace AuthSrv.DTOs.Auth
{
    public class RegisterDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Usertype Role { get; set; }
        public CreateUserDetailsDTO? UserDetails { get; set; }
    }
}
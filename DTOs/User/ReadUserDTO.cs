namespace AuthSrv.DTOs.User
{
    public class ReadUserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public ReadUserDetailsDTO? UserDetails { get; set; }
    }
}
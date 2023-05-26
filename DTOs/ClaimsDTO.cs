namespace AuthSrv.DTOs
{
    public class ClaimsDTO
    {
        public required int IdLogged { get; set; }
        public required string EmailLogged { get; set; }
        public required Usertype UsertypeLogged { get; set; }
    }
}
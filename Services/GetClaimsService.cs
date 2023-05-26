namespace AuthSrv.Services
{
    public class GetClaimsService : IGetClaimsService
    {
        private readonly IHttpContextAccessor _http;

        public GetClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            _http = httpContextAccessor;
        }

        public ClaimsDTO GetMyClaims()
        {
            int idLogged = int.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string emailLogged = _http.HttpContext!.User.FindFirst(ClaimTypes.Name)!.Value.ToString();
            var usertypeClaim = _http.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;
            Enum.TryParse<Usertype>(usertypeClaim, out Usertype userType);
            ClaimsDTO claims = new ClaimsDTO(){
                IdLogged = idLogged,
                EmailLogged = emailLogged,
                UsertypeLogged = userType
            };
            return claims;
        }

    }
}
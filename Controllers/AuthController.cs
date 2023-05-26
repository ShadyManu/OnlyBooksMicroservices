namespace AuthSrv.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGetClaimsService _getClaims;

        public AuthController(IAuthService authService, IGetClaimsService getClaimsService)
        {
            _authService = authService;
            _getClaims = getClaimsService;
        }

        // Registrazione, eseguendo i vari controlli di validità nel Service
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<ReadUserDTO>>> Register(RegisterDTO register){
            ServiceResponse<ReadUserDTO> response = await _authService.Register(register);
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        }


        // Login, eseguendo i vari controlli e restituendo nella ServiceResponse il BearerToken (JWT)
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(LoginDTO login){
            ServiceResponse<string> response = await _authService.Login(login);
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        }

        
        // Metodo di prova per leggere i dati del mio profilo, prendendo l'ID dal Token (nel Service)
        [HttpGet("GetMyUser")]
        public async Task<ActionResult<ServiceResponse<ReadUserDTO>>> GetMyUser(){
            ServiceResponse<ReadUserDTO> response = await _authService.GetMyUser();
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        }
    

        // Prende l'ID della persona loggata
        [HttpGet("GetMyId")]
        public int GetMyId(){
            return _getClaims.GetMyClaims().IdLogged;
        }
    }
}
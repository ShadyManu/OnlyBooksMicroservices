namespace AuthSrv.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<ReadUserDTO>> Register(RegisterDTO register);
        Task<ServiceResponse<string>> Login(LoginDTO login);
        Task<ServiceResponse<ReadUserDTO>> GetMyUser();
        Task<bool> UserExists(string email);
    }
}
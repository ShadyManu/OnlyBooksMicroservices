namespace AuthSrv.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IGetClaimsService _getClaims;

        // Costruttore
        public AuthService(DataContext context, IConfiguration configuration, IMapper mapper, IGetClaimsService getClaims)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _getClaims = getClaims;
        }
        
        // Login
        public async Task<ServiceResponse<string>> Login(LoginDTO login)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            // Controlla se la mail è valida
            response = IsValidEmail<string>(login.Email!);
            if(response.StatusCode == HttpStatusCode.BadRequest) return response;

            try{
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(login.Email.ToLower()));
                if (user is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                }
                else if (!VerifyPasswordHash(login.Password, user.PasswordHash!, user.PasswordSalt!))
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.Message = "Wrong password.";
                }
                else response.Data = "Bearer " + CreateToken(user);
            } catch (OperationCanceledException ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }


        // Register
        public async Task<ServiceResponse<ReadUserDTO>> Register(RegisterDTO register)
        {
            ServiceResponse<ReadUserDTO> response = new ServiceResponse<ReadUserDTO>();

            // Controlla se l'email è valida oppure no
            response = IsValidEmail<ReadUserDTO>(register.Email); 
            if(response.StatusCode == HttpStatusCode.BadRequest) return response;

            // Controlla se la password c'è oppure se è null
            if(register.Password == null || register.Password == ""){
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Invalid password.";
                return response;
            }

            // Controlla se il genere preferito rientra nelle casistiche dell'enum
            if(register.UserDetails?.FavouriteGenre != null){
                if((int) register.UserDetails.FavouriteGenre >= Enum.GetValues(typeof(Genre)).Length){
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Non esiste questo genere di libri.";
                    return response;
                }
            }
            
            try{
                // Controlla se l'Email esiste
                if (await UserExists(register.Email))
                {
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.Message = "Email already in use.";
                    return response;
                }
            
                // Controlla se lo Username esiste già
                if (await _context.UsersDetails.FirstOrDefaultAsync(d => d.Username == register.UserDetails!.Username) is not null)
                {
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.Message = $"This username already exists: {register.UserDetails?.Username} .";
                    return response;
                }

                CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordSalt);

                UserDetailsDTO allDetailsDTO = _mapper.Map<UserDetailsDTO>(register.UserDetails);
                UserDTO userDTO = new UserDTO()
                {
                    Email = register.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = register.Role,
                    UserDetails = allDetailsDTO
                };
                
                var insertUser = _context.Users.Add(_mapper.Map<User>(userDTO)).Entity;
                await _context.SaveChangesAsync();

                ReadUserDTO readUser = _mapper.Map<ReadUserDTO>(insertUser);
                response.Data = readUser;
                response.StatusCode = HttpStatusCode.Created;
                response.Message = "User successfully registered.";
            
            } catch(ArgumentNullException){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "You can't put null values.";
            } 
            catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }


        // Read del proprio User
        public async Task<ServiceResponse<ReadUserDTO>> GetMyUser(){
            ServiceResponse<ReadUserDTO> response = new ServiceResponse<ReadUserDTO>();
            int myId = _getClaims.GetMyClaims().IdLogged;
            try{
                response.Data = _mapper.Map<ReadUserDTO>(await _context.Users
                .Include(u => u.UserDetails)
                .FirstOrDefaultAsync(u => u.Id == myId));
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex + "\n" + ex.Message;
            }
            return response;
        }


        // Metodo per controllare se l'email è valida oppure no, creando una response dinamica con i Generics
        private ServiceResponse<T> IsValidEmail<T>(string Email){
            ServiceResponse<T> isValid = new ServiceResponse<T>();
            try{  
                var mailAddress = new MailAddress(Email);
            } catch(FormatException){
                isValid.StatusCode = HttpStatusCode.BadRequest;
                isValid.Message = "The Email you have provided is not a valid Email.";
            } catch(ArgumentNullException){
                isValid.StatusCode = HttpStatusCode.BadRequest;
                isValid.Message = "Email can't be null.";
            } catch(ArgumentException){
                isValid.StatusCode = HttpStatusCode.BadRequest;
                isValid.Message = "Invalid Email.";
            }
            return isValid;
        }

        
        // --------------------------------------------------------------------------------------------//
        //                              METODI SOLAMENTE PER LA JWT                                    //
        // --------------------------------------------------------------------------------------------//


        // Controlla se lo UserName esiste già nel database oppure no
        public async Task<bool> UserExists(string email)
        {
            try{
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower())) return true;
                else return false;
            } catch(Exception){
                return false;
            }
        }


        // Crea la password criptata in HMACSHA512, passando in ingresso una password lineare
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        // Controlla se la password lineare passata coincide con la password criptata
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


        // Crea il Token dello User, setta i Claims al suo interno, decide la durata della validità del token (Expires)
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
                throw new Exception("AppSettings Token is null.");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    
    }
}
using AuthSrv.Database;

var builder = WebApplication.CreateBuilder(args);

// Aggiunta dei controllers e dei service
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IGetClaimsService, GetClaimsService>();

// Aggiunta della configurazione delle mappature dell'Automapper
builder.Services.AddAutoMapper(new[]{
    typeof(ProfileMapper).Assembly
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Stringa per connessione al Database
//var connectionStringDb = builder.Configuration["Database:ConnectionString"];

/*builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(connectionStringDb, providerOptions =>
        {
            providerOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(1), null);
            providerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });*/

builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:MyDatabase"]));

// Aggiunta dell'autenticazione e autorizzazione in Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII
            .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

//PrepDB.PrepPopulation(app);
// Ignora la convalida del certificato (solo per ambiente di sviluppo/test)
AppContext.SetSwitch("System.Net.Security.DisableCngCertChainPolicy", true);
PrepDB.ApplyDatabaseMigrations(app);
app.Run();

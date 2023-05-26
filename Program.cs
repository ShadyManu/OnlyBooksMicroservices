var builder = WebApplication.CreateBuilder(args);

// Aggiunta dei controllers e dei service
builder.Services.AddControllers();
builder.Services.AddScoped<IBookService, BookService>();

// Aggiunta della configurazione delle mappature dell'Automapper
builder.Services.AddAutoMapper(new[]{
    typeof(ProfileMapper).Assembly
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// Stringa per connessione al Database
var connectionStringDb = builder.Configuration["ConnectionStrings:MyDatabase"];

builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(connectionStringDb, providerOptions =>
        {
            providerOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(1), null);
            providerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });

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
app.Run();

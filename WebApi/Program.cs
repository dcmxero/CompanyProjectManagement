using Application.Auth;
using Application.Services;
using Infrastructure.XmlStorage.Config;
using Infrastructure.XmlStorage.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Auth;

var builder = WebApplication.CreateBuilder(args);

// MVC + Swagger endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// XML config (absolútna cesta)
var contentRoot = builder.Environment.ContentRootPath; // .../WebApi
var configPath = Path.GetFullPath(Path.Combine(contentRoot, "..", "config", "config.xml"));
builder.Services.AddSingleton(new XmlConfigProvider(configPath));

// DI – repo a services
builder.Services.AddSingleton<IXmlProjectRepository, XmlProjectRepository>();
builder.Services.AddScoped<IProjectAppService, ProjectAppService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// JWT settings cez Options
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
          ?? throw new InvalidOperationException("Missing Jwt section.");
if (string.IsNullOrWhiteSpace(jwt.Key))
{
    throw new InvalidOperationException("Missing Jwt:Key (base64).");
}
var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(jwt.Key));

// Default schémy = JWT bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("[JWT] Failed: " + ctx.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            Console.WriteLine("[JWT] Challenge: " + ctx.ErrorDescription);
            return Task.CompletedTask;
        },
        OnMessageReceived = ctx =>
        {
            // ukáže, čo presne prišlo v Authorization
            var hdr = ctx.Request.Headers.Authorization.ToString();
            Console.WriteLine("[JWT] Authorization header: " + hdr);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Swagger s Bearer
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Company Project Management API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT v hlavičke Authorization. Príklad: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

// 0) CONST názov politiky
const string FrontendCors = "FrontendCors";

// 1) Services
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
    // .AllowCredentials()
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(FrontendCors);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

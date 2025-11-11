using Application.Auth;
using Application.Services;
using Infrastructure.XmlStorage.Config;
using Infrastructure.XmlStorage.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Xml.Linq;
using WebApi.Auth;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
//  Serilog z Config/config.xml
// ============================================================================
var contentRoot = builder.Environment.ContentRootPath;
var configPath = Path.Combine(contentRoot, "Config", "config.xml");

string logPathAbs = Path.Combine(contentRoot, "logs", "app-.log");
LogEventLevel minLevel = LogEventLevel.Information;
int fileSizeMB = 10;
int retained = 5;

try
{
    var xml = XDocument.Load(configPath);
    var logging = xml.Root?.Element("logging");

    var logPathXml = logging?.Element("logPath")?.Value;
    if (!string.IsNullOrWhiteSpace(logPathXml))
    {
        var cfgDir = Path.GetDirectoryName(configPath)!;
        logPathAbs = Path.GetFullPath(Path.Combine(cfgDir, logPathXml));
    }

    var levelTxt = logging?.Element("minimumLevel")?.Value ?? "Information";
    minLevel = Enum.Parse<LogEventLevel>(levelTxt, ignoreCase: true);

    if (int.TryParse(logging?.Element("fileSizeLimit")?.Value, out var s)) fileSizeMB = s;
    if (int.TryParse(logging?.Element("retainedFiles")?.Value, out var r)) retained = r;
}
catch (Exception ex)
{
    Console.WriteLine($"[Logging] Cannot read {configPath}: {ex.Message}");
}

Directory.CreateDirectory(Path.GetDirectoryName(logPathAbs)!);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(minLevel)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: logPathAbs,
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: fileSizeMB * 1024 * 1024,
        retainedFileCountLimit: retained,
        rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

// ============================================================================
//  MVC + Swagger endpoints
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// XML config provider
builder.Services.AddSingleton(new XmlConfigProvider(configPath));

// DI – repo a services
builder.Services.AddSingleton<IXmlProjectRepository, XmlProjectRepository>();
builder.Services.AddScoped<IProjectAppService, ProjectAppService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// ============================================================================
//  JWT (z appsettings.json -> sekcia "Jwt")
// ============================================================================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
          ?? throw new InvalidOperationException("Missing Jwt section.");

if (string.IsNullOrWhiteSpace(jwt.Key))
    throw new InvalidOperationException("Missing Jwt:Key (base64).");

var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(jwt.Key));

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
});

builder.Services.AddAuthorization();

// ============================================================================
//  Swagger + Bearer
// ============================================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Company Project Management API",
        Version = "v1",
        Description = "ASP.NET Core Web API with XML storage and JWT authentication."
    });

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
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================================
//  CORS
// ============================================================================
const string FrontendCors = "FrontendCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ============================================================================
//  BUILD & RUN
// ============================================================================
var app = builder.Build();

Log.Information("Serilog initialized → {Path} (level {Level}, size {SizeMB}MB, keep {Keep})",
    logPathAbs, minLevel, fileSizeMB, retained);

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(FrontendCors);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

Log.CloseAndFlush();

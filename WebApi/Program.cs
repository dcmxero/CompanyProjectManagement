using Application.Services;
using Infrastructure.XmlStorage.Config;
using Infrastructure.XmlStorage.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Config + repositories + services
builder.Services.AddSingleton(new XmlConfigProvider("../config/config.xml"));
builder.Services.AddSingleton<IXmlProjectRepository, XmlProjectRepository>();
builder.Services.AddScoped<IProjectAppService, ProjectAppService>();

var app = builder.Build();

app.MapControllers();

app.Run();

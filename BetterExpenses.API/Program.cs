global using static BetterExpenses.Common.Extensions.EnvironmentExtensions;
using BetterExpenses.API.Services;
using BetterExpenses.Common.Database;
using BetterExpenses.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            []
        }
    });
});

const string blazorWebAppOrigins = "Blazor Web App";



builder.Services.ConfigureCors();

builder.Services.AddControllers();
builder.Services.AddRazorPages();


builder.Services.BindConfiguration(builder.Configuration);
builder.Services.BindCommonConfiguration(builder.Configuration);

builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.ConfigureMongo(builder.Configuration);
builder.Services.ConfigurePostgres(builder.Configuration);

builder.Services.ConfigureServices();
builder.Services.ConfigureCommonServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(blazorWebAppOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

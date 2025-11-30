using HrManagmentSystem_API.Middleware;
using HrMangmentSystem_API.Extension_Method;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationService();
builder.Services.AddAutoMapperProfiles();
builder.Services.AddConfigureDatabases(builder.Configuration);
builder.Services.AddLocaizationResource(); 

builder.Services.AddControllers();

builder.Services.AddLocalization(options => options.ResourcesPath = "");

var jwtSection = builder.Configuration.GetSection("JwtSettings");
var key = jwtSection["Key"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; 
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),

            ClockSkew = TimeSpan.FromMinutes(30)
        };
    });

builder.Services.AddAuthorization();




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseAuthentication();

app.UseAddLocalization();

app.UseMiddleware<CurrentTenantMiddleware>(); //after Authentication : because httpContext.User fill from JWT 

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();

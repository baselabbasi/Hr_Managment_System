using HrManagmentSystem_API.Extension_Method;
using HrManagmentSystem_API.Middleware;
using HrMangmentSystem_API.Extension_Method;
using HrMangmentSystem_Application.Extension_Method;
using HrMangmentSystem_Infrastructure.Extension_Method;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationService();
builder.Services.AddAutoMapperProfiles();
builder.Services.AddConfigureDatabases(builder.Configuration);
builder.Services.AddLocaizationResource(builder.Configuration) ;
builder.Services.AddLeaveAccrualQuartz(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers()
              .AddJsonOptions(options =>
              {
                  options.JsonSerializerOptions.Converters.Add(
                      new JsonStringEnumConverter());
              });

builder.Services.AddLocalization(options => options.ResourcesPath = "");


builder.Services.AddAuthorization();




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


//await DataSeeding.SeedAsync(app.Services);


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

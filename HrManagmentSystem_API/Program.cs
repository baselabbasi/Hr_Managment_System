using HrMangmentSystem_API.Extension_Method;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationService();
builder.Services.AddAutoMapperProfiles();
builder.Services.AddConfigureDatabases(builder.Configuration);
builder.Services.AddLocaizationResource(); 

builder.Services.AddControllers();

builder.Services.AddLocalization(options => options.ResourcesPath = "");




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAddLocalization(); 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Services;
using PortfolioApi.Endpoints;


var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("PortfolioDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'PortfolioDatabase' ontbreekt.");

builder.Services.AddDbContext<PortfolioDbContext>(dbOptions =>
    dbOptions.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddOpenApi();
//Swaggergen wordt toegevoegd
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogpostService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapBlogpostEndpoints();
app.MapProjectEndpoints();

app.Run();

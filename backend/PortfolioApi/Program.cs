using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Services;
using PortfolioApi.Dtos;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("PortfolioDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'PortfolioDatabase' ontbreekt.");

builder.Services.AddDbContext<PortfolioDbContext>(dbOptions =>
    dbOptions.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//Swaggergen wordt toegevoegd
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ProjectService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var projectRoutes = app.MapGroup("/api/projects")
    .WithTags("Projecten");

    projectRoutes.MapGet("/", async (ProjectService service) =>
{
    var projecten = await service.HaalAlleOpAsync();
    return TypedResults.Ok(projecten);
});

projectRoutes.MapPost("/", async (ProjectCreateDto invoer, ProjectService service) =>
{
    var project = await service.MaakAanAsync(invoer);
    return TypedResults.Created($"/api/projects/{project.Id}", project);
});

app.Run();

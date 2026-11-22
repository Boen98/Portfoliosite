using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
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

    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<PortfolioDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("KanPortfolioBewerken", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Editor", "Admin"));
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapBlogpostEndpoints();
app.MapProjectEndpoints();
app.MapAuthEndpoints();

await IdentitySeeder.SeedRolesAsync(app.Services);

app.Run();
public partial class Program { }
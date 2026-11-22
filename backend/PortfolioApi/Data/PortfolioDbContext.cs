using Microsoft.EntityFrameworkCore;
using PortfolioApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PortfolioApi.Data;
//inheritance van DbContext, zodat we de database kunnen benaderen
public class PortfolioDbContext : IdentityDbContext<IdentityUser>
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projecten { get; set; }
    public DbSet<Blogpost> Blogposts { get; set; }
}
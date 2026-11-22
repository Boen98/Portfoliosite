using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioApi.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] rollen = { "Gast", "Editor", "Admin" };

        foreach (var rolnaam in rollen)
        {
            if (await roleManager.RoleExistsAsync(rolnaam))
            {
                continue;
            }

            var resultaat = await roleManager.CreateAsync(
                new IdentityRole(rolnaam));

            if (!resultaat.Succeeded)
            {
                var fouten = string.Join("; ",
                    resultaat.Errors.Select(fout => fout.Description));

                throw new InvalidOperationException(
                    $"De rol '{rolnaam}' kon niet worden aangemaakt: {fouten}");
            }
        }
    }
}
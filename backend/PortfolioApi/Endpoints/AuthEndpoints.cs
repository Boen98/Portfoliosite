using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PortfolioApi.Data;
using PortfolioApi.Dtos;

namespace PortfolioApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", RegistreerAsync)
            .WithTags("Auth")
            .AllowAnonymous();

        app.MapPost("/api/auth/login", InloggenAsync)
           .WithTags("Auth")
           .AllowAnonymous();
    }
    private static async Task<IResult> RegistreerAsync(
        RegistratieDto invoer,
        UserManager<IdentityUser> userManager,
        PortfolioDbContext dbContext)
    {
        var email = invoer.Email?.Trim();
        var wachtwoord = invoer.Password;

        if (string.IsNullOrWhiteSpace(email) ||
            email.Length > 256 ||
            !new EmailAddressAttribute().IsValid(email))
        {
            return Results.BadRequest(new
            {
                melding = "Vul een geldig e-mailadres in."
            });
        }

        if (string.IsNullOrWhiteSpace(wachtwoord))
        {
            return Results.BadRequest(new
            {
                melding = "Vul een wachtwoord in."
            });
        }

        var gebruiker = new IdentityUser
        {
            Email = email,
            UserName = email
        };

        await using var transactie =
            await dbContext.Database.BeginTransactionAsync();

        var registratie = await userManager.CreateAsync(gebruiker, wachtwoord);

        if (!registratie.Succeeded)
        {
            return Results.BadRequest(new
            {
                melding = "Registratie mislukt.",
                fouten = registratie.Errors.Select(fout => fout.Description).ToArray()
            });
        }

        var rolResultaat = await userManager.AddToRoleAsync(gebruiker, "Gast");

        if (!rolResultaat.Succeeded)
        {
            return Results.Problem(
                title: "Registratie kon niet worden voltooid.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        await transactie.CommitAsync();

        return Results.Ok(new { melding = "Registratie gelukt." });
    }

    private static async Task<IResult> InloggenAsync(
    LoginDto invoer,
    SignInManager<IdentityUser> signInManager)
{
    var email = invoer.Email?.Trim();
    var wachtwoord = invoer.Password;

    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(wachtwoord))
    {
        return Results.BadRequest(new
        {
            melding = "Vul je e-mailadres en wachtwoord in."
        });
    }

    var resultaat = await signInManager.PasswordSignInAsync(
        email,
        wachtwoord,
        isPersistent: false,
        lockoutOnFailure: true);

    if (!resultaat.Succeeded)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new { melding = "Inloggen gelukt." });
  }

}
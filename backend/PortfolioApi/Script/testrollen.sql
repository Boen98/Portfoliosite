USE [BoenPortfolioDb];

INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
SELECT gebruiker.Id, rol.Id
FROM (VALUES
    (N'EDITOR-OEFEN@EXAMPLE.TEST', N'EDITOR'),
    (N'ADMIN-OEFEN@EXAMPLE.TEST', N'ADMIN')
) AS toekenning(Gebruikersnaam, Rolnaam)
JOIN dbo.AspNetUsers AS gebruiker
    ON gebruiker.NormalizedUserName = toekenning.Gebruikersnaam
JOIN dbo.AspNetRoles AS rol
    ON rol.NormalizedName = toekenning.Rolnaam
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.AspNetUserRoles AS koppeling
    WHERE koppeling.UserId = gebruiker.Id
      AND koppeling.RoleId = rol.Id
);
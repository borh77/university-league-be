using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Solution.Identity.Core.Domain;
using Solution.Identity.Core.UseCases;

namespace Solution.Identity.Infrastructure.Database.Services;

public class JwtTokenGenerator : ITokenGenerator
{
    public GeneratedToken Generate(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(JwtSettingsBuilder.ExpiryHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("name", user.FullName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettingsBuilder.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtSettingsBuilder.Issuer,
            audience: JwtSettingsBuilder.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var serialized = new JwtSecurityTokenHandler().WriteToken(token);
        return new GeneratedToken(serialized, expiresAt);
    }
}

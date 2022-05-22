using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SampleApp.Application.Domain.ValueObjects;

public class Authentication
{
    public Authentication(string audience, string issuer, string hmacSecretKey, string expireInDays)
    {
        Audience = audience;
        Issuer = issuer;
        ExpireInDays = expireInDays;
        HmacSecretKey = hmacSecretKey;
    }

    public string Audience { get; }

    public string Issuer { get; }

    public string ExpireInDays { get; }

    public string HmacSecretKey { get; }

    public string GenerateAccessToken(string username)
    {
        var symmetricKey = Convert.FromBase64String(HmacSecretKey);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
            Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(ExpireInDays)),
            Audience = Audience,
            Issuer = Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(symmetricKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}

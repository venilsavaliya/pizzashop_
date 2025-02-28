namespace BLL.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using BLL.Interfaces;

public class JwtService : IJwtService
{

    private readonly IConfiguration _config;


    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateJwtToken(string Email, string Role, int day = 7)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, Email),
            new Claim(ClaimTypes.Role, Role),
        };

        var jwtConfig = _config.GetSection("jwt");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig["issuer"],
            audience: jwtConfig["audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(day),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool IsTokenExpired(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
            return true; // Invalid token, consider it expired

        var jwtToken = tokenHandler.ReadToken(token);

        if (jwtToken == null)
            return true;

        return jwtToken.ValidTo < DateTime.UtcNow; // Compare with current time
    }

    public string GetEmailDetailsFromToken(string Token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (Token == null)
        {
            return "nothing";
        }
        var jwtToken = handler.ReadJwtToken(Token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return email;
    }

}

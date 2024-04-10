using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StudentPortal.Models;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.Services.Implementations;

public class JWTAuthenticator : IJWTAuthenticator
{
    private readonly JWTConfiguration _jwtConfiguration;

    public JWTAuthenticator(JWTConfiguration jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task<JWTToken> GenerateJwtToken(ApplicationUser user, string? expires = null,
        List<Claim>? additionalClaims = null)
    {
        JwtSecurityTokenHandler jwtTokenHandler = new();
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
        IdentityOptions _options = new();

        var claims = new List<Claim>
        {
            new Claim("Id", user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id),
            new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
        };

        if (additionalClaims != null) claims.AddRange(additionalClaims);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = string.IsNullOrWhiteSpace(expires)
                ? DateTime.Now.AddHours(double.Parse(_jwtConfiguration.Expires))
                : DateTime.Now.AddMinutes(double.Parse(expires)),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfiguration.Issuer,
            Audience = _jwtConfiguration.Audience
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return new JWTToken
        {
            Token = jwtToken,
            Issued = DateTime.Now,
            Expires = tokenDescriptor.Expires
        };
    }
}
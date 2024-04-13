using System.Security.Claims;
using System.Threading.Tasks;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;

namespace StudentPortal.Services.Interfaces;

public interface IJWTAuthenticator
{
    Task<JWTToken> GenerateJwtToken(ApplicationUser user, string? expires = null, List<Claim>? additionalClaims = null);
}
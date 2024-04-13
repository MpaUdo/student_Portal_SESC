using Microsoft.AspNetCore.Identity;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;

namespace StudentPortal.Services.Interfaces;

public interface IAuthService
{
    Task<LoggedInUserResponse> UserLogin(LoginRequest request);
    Task<IdentityResult> Register(RegisterRequest request);
}
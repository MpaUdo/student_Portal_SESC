using Microsoft.AspNetCore.Identity;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Interfaces;
using System.Security.Claims;
using System.Text.RegularExpressions;
using StudentPortal.Models.Dtos.Requests;

namespace StudentPortal.Services.Implementations;
public class AuthService : IAuthService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(IServiceFactory serviceFactory, UserManager<ApplicationUser> userManager)
    {
        _serviceFactory = serviceFactory;
        _userManager = userManager;
    }

    public async Task<LoggedInUserResponse> UserLogin(LoginRequest request)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(request.Username.ToLower().Trim());

        if (user == null)
            throw new InvalidOperationException("Invalid username or password");

       
        bool result = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
            throw new InvalidOperationException("Invalid username or password");

        JWTToken userToken = await _serviceFactory.GetService<IJWTAuthenticator>().GenerateJwtToken(user);

        return new LoggedInUserResponse
        {
            JwtToken = userToken,
            Username = user.UserName,
            Email = user.Email
        };
    }
    
    public async Task<IdentityResult> Register(RegisterRequest request)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(request.Username.ToLower().Trim());

        if (user != null)
            throw new InvalidOperationException("user already exists!");

        return  (await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email
        }, password: request.Password));
    }
}
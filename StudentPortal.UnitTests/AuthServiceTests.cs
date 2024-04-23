using Microsoft.AspNetCore.Identity;
using Moq;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Implementations;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.UnitTests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private  LoginRequest _loginRequest;
        
    private readonly ApplicationUser _applicationUser;

    public AuthServiceTests()
    {
        // Arrange: Set up mock dependencies
        var mockServiceFactory = new Mock<IServiceFactory>();
        var mockJwtAuthenticator = new Mock<IJWTAuthenticator>();

        // Arrange: Set up mock UserManager
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null
        );

        _loginRequest = new LoginRequest { Username = "testuser", Password = "password" };
        _applicationUser = new ApplicationUser { UserName = "testuser", Email = "test@example.com" };

        // Mock UserManager behavior
        _mockUserManager.Setup(u => u.FindByNameAsync(_loginRequest.Username))
            .ReturnsAsync(_applicationUser);

        // Mock UserManager behavior
        _mockUserManager.Setup(u => u.CheckPasswordAsync(_applicationUser, _loginRequest.Password))
            .ReturnsAsync(true);

        mockServiceFactory.Setup(f => f.GetService<IJWTAuthenticator>())
            .Returns(mockJwtAuthenticator.Object);
            
        mockServiceFactory.Setup(expression: f => f.GetService<IJWTAuthenticator>().GenerateJwtToken(_applicationUser, null, null))
            .ReturnsAsync(new JWTToken());
            

        // Create AuthService instance with mock dependencies
        _authService = new AuthService(mockServiceFactory.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task UserLogin_ValidCredentials_ReturnsLoggedInUserResponse()
    {
        // Act
        var result = await _authService.UserLogin(_loginRequest);

        // Assert
        Assert.NotNull(result.JwtToken);
        Assert.Equal(_applicationUser.UserName, result.Username);
        Assert.Equal(_applicationUser.Email, result.Email);
    }
        
    [Fact]
    public async Task UserLogin_InValidCredentials_ThrowsInvalidOperationException()
    {
        //Arrange
        _loginRequest.Password = string.Empty;
            
        // Assert
        var equals =
            (await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.UserLogin(_loginRequest)))
            .Message.Equals("Invalid username or password");
        Assert.True(equals);
    }
}
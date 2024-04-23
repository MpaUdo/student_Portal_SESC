using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bogus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using StudentPortal.Data.Context;
using StudentPortal.Data.Extensions;
using StudentPortal.Integrations.Tests.Handlers;
using StudentPortal.IntegrationsTests.Factories;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Interfaces;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace StudentPortal.IntegrationsTests;

public class StudentIntegrationTests : IClassFixture<StudentApiAppFactory<Program>>
{
    private readonly StudentApiAppFactory<Program> _factory;
    private HttpClient _httpClient;
    private RegisterRequest _registerRequest;
    private LoginRequest _loginRequest;
    private UpdateStudentProfileDto _updateStudentProfile;
     
    public StudentIntegrationTests(StudentApiAppFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        _registerRequest =  new RegisterRequest
        {
            Username = "john",
            Email = "john@gmail.com",
            Password = "John337@@"
        };
        
        _loginRequest = new LoginRequest
        {
            Username = _registerRequest.Username,
            Password = _registerRequest.Password
        };

        _updateStudentProfile = new UpdateStudentProfileDto
        {
            Firstname = "John",
            Surname = "Doe"
        };
        
        var serviceProvider = _factory.Services.CreateScope().ServiceProvider;
        var authContext = serviceProvider.GetService<AuthContext>();
        authContext.Database.EnsureCreated();
        
        var appContext = serviceProvider.GetService<ApplicationContext>();
        appContext.Database.EnsureCreated();
    }
    
    
    // Invalid username or password
    [Fact]
    public async Task GivenWrongLoginCredentials_WhenLoginIsInvoked_ThenOfBadRequestIsReturned()
    {
        // Arrange
        // Act
        var response =  await _httpClient.PostAsJsonAsync("/api/auth/login", _loginRequest);
        var contentResponse = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<dynamic>(contentResponse)?.message;
        
        // Assert
        Assert.Equal("Invalid username or password", responseObj.ToString());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode );
    }

    
    [Fact]
    public async Task GivenAlreadyExistingUserNameOrEmail_WhenGetRegisterIsInvoked_ThenOfBadRequestIsReturned()
    {
        // Arrange
        await AuthenticateUserAsync();
        
        // Act
        var updateResponse = await _httpClient.PutAsJsonAsync("/api/student/profile", _updateStudentProfile);
        
        var confirmProfileUpdate = await _httpClient.GetAsync("/api/student/profile");
        var content = await confirmProfileUpdate.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<dynamic>(content)?.data;
        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode );
        Assert.Equal(HttpStatusCode.OK, confirmProfileUpdate.StatusCode );
        Assert.Equal(_updateStudentProfile.Firstname, responseObj.firstname.ToString());
        Assert.Equal(_updateStudentProfile.Surname, responseObj.surname.ToString());
       
    }
    
    [Fact]
    public async Task GivenFirstNameOrLastname_WhenGetUpdateProfileIsInvoked_ThenOfOkResponseIsReturned()
    {
        // Arrange
        await AuthenticateUserAsync();
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", _registerRequest);
        var content = await response.Content.ReadAsStringAsync();

        var responseObj = JsonConvert.DeserializeObject<dynamic>(content)?.message;
        // Assert
        Assert.Equal("account already exists", responseObj.ToString());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode );
    }


    [Fact]
    public async Task GivenPaginatedListOfCourses_WhenGetCoursesIsInvoked_ThenOkResponseCollectionIsReturned()
    {
         // Act
         await AuthenticateUserAsync();
         var response = await _httpClient.GetAsync("api/courses");
         // Assert
         var successResponse = await response.Content.ReadAsStringAsync();
         var responseObj = JsonConvert.DeserializeObject<dynamic>(successResponse)?.data;
         var paginatedCourses = JsonConvert.DeserializeObject<PagedResponse<CourseDto>>(responseObj?.ToString()) as PagedResponse<CourseDto>;
         Assert.NotEmpty(paginatedCourses.Items);
         Assert.NotNull(paginatedCourses.MetaData);
         Assert.Equal(HttpStatusCode.OK, response.StatusCode );
    }
    
    [Fact]
    public async Task GivenPaginatedListOfCoursesWithNoAuth_WhenGetCoursesIsInvoked_ThenOfUnauthorizedResponseIsReturned()
    {
        // Arrange
        // Act
        var response = await _httpClient.GetAsync("api/courses");
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode );
    }

    private async Task AuthenticateUserAsync()
    {
        _httpClient = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IFinanceService, TestFinanceService>();
                    services.AddScoped<ILibraryService, TestLibraryService>();
                });
            })
            .CreateClient();

        await _httpClient.PostAsJsonAsync("/api/auth/register", _registerRequest);
        var response =  await _httpClient.PostAsJsonAsync("/api/auth/login", _loginRequest);
        var successResponse = await response.Content.ReadAsStringAsync();

        var responseObj = JsonConvert.DeserializeObject<dynamic>(successResponse)?.data?.jwtToken.token;
        
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer",responseObj?.ToString());
    }
}
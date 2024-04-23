using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using StudentPortal.Data.Interfaces;
using StudentPortal.Models.Dtos;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Implementations;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.UnitTests;

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILibraryService> _mockLibraryService;
    private readonly Mock<IFinanceService> _mockFinanceService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IRepository<Student>> _mockStudentRepository;

    public StudentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLibraryService = new Mock<ILibraryService>();
        _mockFinanceService = new Mock<IFinanceService>();
        _mockMapper = new Mock<IMapper>();
        _mockAuthService = new Mock<IAuthService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockStudentRepository = new Mock<IRepository<Student>>();
        _mockStudentRepository.Setup(r => r.GetSingleByAsync(It.IsAny<Expression<Func<Student, bool>>>(),
                It.IsAny<Func<IQueryable<Student>, IOrderedQueryable<Student>>>(),
                null,
                null,
                It.IsAny<Func<IQueryable<Student>, IIncludableQueryable<Student, object>>>(),
                false))
            .ReturnsAsync(new Student());
        _mockUnitOfWork.Setup(f => f.GetRepository<Student>()).Returns(_mockStudentRepository.Object);
    }
    
    [Fact]
    public async Task Register_WhenAccountAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var studentService = new StudentService(
            _mockUnitOfWork.Object,
            _mockLibraryService.Object,
            _mockFinanceService.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockHttpContextAccessor.Object
        );

        var existingStudent = new Student { Username = "existinguser", Email = "existing@example.com" };
        var registerDto = new RegisterStudentDto
        {
            Username = existingStudent.Username,
            Email = existingStudent.Email,
            Password = "TestPassword123"
        };

        _mockStudentRepository.Setup(r => r.GetSingleByAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync(existingStudent);

        // Act & Assert
        var correctException =
            (await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await studentService.Register(registerDto))).Message.Equals("account already exists");
        Assert.True(correctException);
    }
    
    [Fact]
    public async Task UpdateProfile_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        var studentService = new StudentService(
            _mockUnitOfWork.Object,
            _mockLibraryService.Object,
            _mockFinanceService.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockHttpContextAccessor.Object
        );

        var updateDto = new UpdateStudentProfileDto { Firstname = "FirstName", Surname = "Surname"};
        _mockHttpContextAccessor.SetupGet(c => c.HttpContext.User.Identity.Name).Returns("nonexistentuser");
        _mockStudentRepository.Setup(r => r.GetSingleByAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync((Student)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await studentService.UpdateProfile(updateDto));
    }

    [Fact]
    public async Task GetStudentProfile_WhenProfileExists_ShouldReturnProfileDto()
    {
        // Arrange
        var studentService = new StudentService(
            _mockUnitOfWork.Object,
            _mockLibraryService.Object,
            _mockFinanceService.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockHttpContextAccessor.Object
        );

        var existingUser = new Student { Username = "testuser", Firstname = "Firstname"};
        _mockHttpContextAccessor.SetupGet(c => c.HttpContext.User.Identity.Name).Returns(existingUser.Username);
        _mockStudentRepository.Setup(r => r.GetSingleByAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync(existingUser);

        _mockUnitOfWork.Setup(f => f.GetRepository<Student>()).Returns(_mockStudentRepository.Object);

        _mockMapper.Setup(f => f.Map<StudentProfileDto>(existingUser)).Returns(new StudentProfileDto
        {
            Firstname = existingUser.Firstname,
            StudentId = existingUser.StudentId
        });
        // Act
        var profileDto = await studentService.GetStudentProfile();

        // Assert
        Assert.NotNull(profileDto);
        Assert.Equal(existingUser.StudentId, profileDto.StudentId);
        Assert.Equal(existingUser.Firstname, profileDto.Firstname);
        // Add more assertions for other properties if needed
    }

    [Fact]
    public async Task GetStudentProfile_WhenProfileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var studentService = new StudentService(
            _mockUnitOfWork.Object,
            _mockLibraryService.Object,
            _mockFinanceService.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockHttpContextAccessor.Object
        );

        _mockHttpContextAccessor.SetupGet(c => c.HttpContext.User.Identity.Name).Returns("nonexistentuser");

        // Act & Assert
       var errorMessage =  (await Assert.ThrowsAsync<InvalidOperationException>(async () => await studentService.GetStudentProfile())).Message;
       Assert.Equal("Profile doesn't exist", errorMessage);
    }

    [Fact]
    public async Task GraduationStatus_WhenInvoicesOutstanding_ShouldReturnFalse()
    {
        // Arrange
        
        var mockHttpContext = new Mock<HttpContext>();
        var mockUser = new Mock<ClaimsPrincipal>();
        var mockIdentity = new Mock<ClaimsIdentity>();

// Setup the mock objects to simulate a valid user identity
        var existingUser = new Student { Username = "testuser", StudentId = "C12345678" };
        
        _mockHttpContextAccessor.SetupGet(c => c.HttpContext.User.Identity.Name).Returns(existingUser.Username);
        _mockStudentRepository.Setup(r => r.GetSingleByAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync(existingUser);
        
        _mockUnitOfWork.Setup(f => f.GetRepository<Student>()).Returns(_mockStudentRepository.Object);
        var studentService = new StudentService(
            _mockUnitOfWork.Object,
            _mockLibraryService.Object,
            _mockFinanceService.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockHttpContextAccessor.Object
        );
       

        var financeAccount = new FinanceAccountDto() { HasOutStandingBalance = true };
        _mockFinanceService.Setup(f => f.GetAccountOutstanding(existingUser.StudentId))
            .ReturnsAsync(financeAccount);

        // Act
        var graduationStatus = await studentService.GetGraduationStatus();

        // Assert
        Assert.False(graduationStatus); // Assuming graduation status is based on outstanding balance
    }

    [Fact]
    public async Task GetGraduationStatus_WhenProfileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var studentService = new StudentService(
            _mockUnitOfWork.Object,
            _mockLibraryService.Object,
            _mockFinanceService.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockHttpContextAccessor.Object
        );

        _mockUnitOfWork.Setup(f => f.GetRepository<Student>()).Returns(_mockStudentRepository.Object);
        _mockHttpContextAccessor.SetupGet(c => c.HttpContext.User.Identity.Name).Returns("nonexistentuser");
        _mockStudentRepository.Setup(r => r.GetSingleByAsync(It.IsAny<Expression<Func<Student, bool>>>(),
                It.IsAny<Func<IQueryable<Student>, IOrderedQueryable<Student>>>(),
                null,
                null,
                It.IsAny<Func<IQueryable<Student>, IIncludableQueryable<Student, object>>>(),
                false))
            .ReturnsAsync(new Student());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await studentService.GetGraduationStatus());
    }
}
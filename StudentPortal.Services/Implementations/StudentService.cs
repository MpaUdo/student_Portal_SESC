using AutoMapper;
using Microsoft.AspNetCore.Http;
using StudentPortal.Data.Interfaces;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILibraryService _libraryService;
        private readonly IFinanceService _financeService;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRepository<Student> _studentRepository;

        public StudentService(IUnitOfWork unitOfWork, ILibraryService libraryService, IFinanceService financeService,
            IMapper mapper, IAuthService authService, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _libraryService = libraryService;
            _financeService = financeService;
            _mapper = mapper;
            _authService = authService;
            _contextAccessor = contextAccessor;
            _studentRepository = _unitOfWork.GetRepository<Student>();
        }

        public async Task Register(RegisterStudentDto request)
        {
            var student = await _studentRepository.GetSingleByAsync(s =>
                s.Username.ToLower() == request.Username || s.Email.ToLower() == request.Email.ToLower());
            if (student != null)
                throw new InvalidOperationException("account already exists");
            
            var model = _mapper.Map<Student>(request);
            model.CreatedBy = request.Username;
            model.StudentId = $"C{GenerateRandomAlphanumericString(8)}";
            _studentRepository.Add(model);

            var financeAccountCreationSuccessful = await _financeService.CreateAccount(new AccountDto
            {
                StudentId = model.StudentId
            });

            var libraryAccountCreationSuccessful = await _libraryService.CreateAccount(new AccountDto
            {
                StudentId = model.StudentId
            });

            if (libraryAccountCreationSuccessful && financeAccountCreationSuccessful)
            {
                var identityResult = await _authService.Register(new RegisterRequest()
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                });
                
                if (identityResult.Succeeded)
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException(string.Join(",", identityResult.Errors.Select(e => e.Description)));
                }
                
            }
            else
            {
                throw new InvalidOperationException("Creating an account fails at the moment due to an internal service downtime!");
            }
        }

        public async Task UpdateProfile(UpdateStudentProfileDto request)
        {
           var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
            var student = await _studentRepository.GetSingleByAsync(s =>
                username != null && s.Username.ToLower() ==  username.ToLower());

            if (student!=null)
            {
                var updatedStudent = _mapper.Map(request, student);
                updatedStudent.ModifiedBy = username;
                await  _studentRepository.UpdateAsync(updatedStudent);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Profile doesn't exist");
            }

        }

        public async Task<StudentProfileDto> GetStudentProfile()
        {
            var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
            var student = await _studentRepository.GetSingleByAsync(s =>
                s.Username.ToLower() == username.ToLower());
            if (student!=null)
                return _mapper.Map<StudentProfileDto>(student);
            throw new InvalidOperationException("Profile doesn't exist");
        }

        public async Task<bool> GetGraduationStatus()
        {
            var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
            var student = await _studentRepository.GetSingleByAsync(s =>
                s.Username.ToLower() == username.ToLower());

            if (student == null)
                throw new InvalidOperationException("Profile doesn't exist");

            return !(await _financeService.GetAccountOutstanding(student.StudentId)).HasOutStandingBalance;
           

        }
        private string GenerateRandomAlphanumericString(int length)
        {
            var random = new Random();
            const string alphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(alphanumericChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

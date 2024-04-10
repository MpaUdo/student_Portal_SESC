using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StudentPortal.Data.Extensions;
using StudentPortal.Data.Interfaces;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFinanceService _financeService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<StudentEnrollment> _studentEnrollmentRepository;

    public CourseService(IUnitOfWork unitOfWork, IMapper mapper, IFinanceService financeService, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _financeService = financeService;
        _contextAccessor = contextAccessor;
        _courseRepository = unitOfWork.GetRepository<Course>();
        _studentEnrollmentRepository = unitOfWork.GetRepository<StudentEnrollment>();
    }

    public async Task<PagedResponse<CourseDto>> GetCourses(CoursesRequest request)
    {
       var pagedItems = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? await _courseRepository.GetPagedItems(request)
            : await _courseRepository.GetPagedItems(request, c => c.Title.ToLower().Contains(request.SearchTerm.ToLower()));
       return _mapper.Map<PagedResponse<CourseDto>>(pagedItems);
    }

    public async Task<CourseDto> GetCourse(string courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);

        if(course == null)
            throw new InvalidOperationException("Not Found");

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<string> Enroll(string courseId)
    {
      var course =  await _courseRepository.GetSingleByAsync(c => c.Id == courseId,include: x => x.Include(c => c.StudentEnrollments) );
      var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
      var student = await _unitOfWork.GetRepository<Student>().GetSingleByAsync(s =>
          s.Username.ToLower() == username.ToLower());
      if (course == null) 
          throw new InvalidOperationException($"{courseId} is not found");


      var response = await _financeService.GenerateInvoice(new GenerateInvoiceDto()
      {
          Amount = course.Fee,
          Account = new AccountDto
          {
              StudentId = student.StudentId,
          }
      });

      course.StudentEnrollments.Add(new StudentEnrollment
      {
          StudentId = student.Id,
          CreatedBy = student.Username,
          InvoiceReference = response
      });

        await _courseRepository.UpdateAsync(course);
     await _unitOfWork.SaveChangesAsync();
      return response;

    }
    public async Task<PagedResponse<StudentEnrollmentDto>> GetStudentEnrollment(StudentEnrollmentRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.StudentId))
        {
            var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
            var student = await _unitOfWork.GetRepository<Student>().GetSingleByAsync(s =>
                s.Username.ToLower() == username.ToLower());
            request.StudentId = student.Id;
        }

        var pagedItems = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? await _studentEnrollmentRepository.GetQueryable().Include(s => s.Course).GetPagedItems(request, c => c.StudentId == request.StudentId)
            : await _studentEnrollmentRepository.GetQueryable().Include(i => i.Course).GetPagedItems(request, c => c.StudentId == request.StudentId);
        return _mapper.Map<PagedResponse<StudentEnrollmentDto>>(pagedItems);
    }
}
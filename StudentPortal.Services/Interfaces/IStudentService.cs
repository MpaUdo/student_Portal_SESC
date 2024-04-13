using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;

namespace StudentPortal.Services.Interfaces;

public interface IStudentService
{
    Task Register(RegisterStudentDto request);
    Task UpdateProfile(UpdateStudentProfileDto request);
    Task<StudentProfileDto> GetStudentProfile();
    Task<bool> GetGraduationStatus();
}
using AutoMapper;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;

namespace StudentPortal.Models.Configuration.MappingProfiles;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<RegisterStudentDto, Student>();
        CreateMap<UpdateStudentProfileDto, Student>()
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.Now));
        CreateMap<Student, StudentProfileDto>();
        CreateMap<StudentEnrollment, StudentEnrollmentDto>()
            .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));

    }
}
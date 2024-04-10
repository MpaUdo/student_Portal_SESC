using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;

namespace StudentPortal.Models.Configuration.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>();

            CreateMap<StudentEnrollment, StudentEnrollmentDto>();
        }
    }
}

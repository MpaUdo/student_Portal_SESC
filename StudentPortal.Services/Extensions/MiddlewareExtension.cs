using Microsoft.Extensions.DependencyInjection;
using StudentPortal.Data.Context;
using StudentPortal.Data.Implementation;
using StudentPortal.Data.Interfaces;
using StudentPortal.Services.Implementations;
using StudentPortal.Services.Interfaces;


namespace StudentPortal.Services.Extensions
{
    public static class MiddlewareExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork<ApplicationContext>>();
            services.AddTransient<IJWTAuthenticator, JWTAuthenticator>();
            services.AddTransient<IServiceFactory, ServiceFactory>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}

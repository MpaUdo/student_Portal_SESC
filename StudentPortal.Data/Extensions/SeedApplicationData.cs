using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentPortal.Data.Context;
using StudentPortal.Models.Entities;

namespace StudentPortal.Data.Extensions;

public static class SeedApplicationData
{
    public static async Task EnsurePopulated(this IApplicationBuilder app)
    {

        var courseFaker = new Faker<Course>()
            .RuleFor(u => u.Title, f => f.Lorem.Letter(50))
            .RuleFor(u => u.Description, f => f.Lorem.Paragraphs(2))
            .RuleFor(u => u.Fee, f => f.Finance.Amount())
            .RuleFor(u => u.CreatedBy, f => f.Lorem.Letter(50));

            var courses = courseFaker.Generate(1000);
            var appContext = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();

            if (!await appContext.Courses.AnyAsync())
            {
               await appContext.AddRangeAsync(courses);
               await appContext.SaveChangesAsync();
            }
    }
}
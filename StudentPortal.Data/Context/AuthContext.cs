using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models.Entities;
using System.Reflection.Emit;

namespace StudentPortal.Data.Context;

public class AuthContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
    IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
    IdentityUserToken<string>>
{
    public AuthContext(DbContextOptions<AuthContext> options):
        base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(e => e.Id)   
                .ValueGeneratedOnAdd();
            base.OnModelCreating(builder);
        });
    }
}
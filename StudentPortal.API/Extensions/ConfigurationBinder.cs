using StudentPortal.Models;

namespace StudentPortal.API.Extensions;

public static class ConfigurationBinder
{
    public static IServiceCollection BindConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        JWTConfiguration jwt = new();
        AppConfiguration appConfiguration = new();

        configuration.GetSection("JwtConfig").Bind(jwt);
        configuration.GetSection("appConfiguration").Bind(appConfiguration);

        services.AddSingleton(jwt);
        services.AddSingleton(appConfiguration);
       
        return services;
    }
}
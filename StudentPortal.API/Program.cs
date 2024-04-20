using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentPortal.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using StudentPortal.Data.Extensions;
using StudentPortal.Services.Extensions;
using Polly;
using Polly.Extensions.Http;
using StudentPortal.Services.Implementations;
using StudentPortal.Services.Interfaces;
using System.Reflection;
using StudentPortal.API.Extensions;
using StudentPortal.API.Handlers;
using StudentPortal.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string appDbConnString = builder.Configuration.GetConnectionString("AppConnection");
string authDbConnString = builder.Configuration.GetConnectionString("AuthConnection");



builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(appDbConnString, s =>
    {
        s.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(20), null);
    }).EnableSensitiveDataLogging(builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
});

builder.Services.AddDbContext<AuthContext>(options =>
{
    options.UseNpgsql(authDbConnString, s =>
        {
            s.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(20), null);
        })
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
});



builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
    {
        var alphabet = "abcdefghijklmnopqrstuvwxyz";
        config.User.AllowedUserNameCharacters = $"{alphabet}{alphabet.ToUpper()}0123456789/-@.";
        config.User.RequireUniqueEmail = true;
        config.Password.RequiredLength = 8;
        config.Password.RequireDigit = true;
        config.Password.RequiredUniqueChars = 0;
        config.Password.RequireNonAlphanumeric = true;
        config.Password.RequireUppercase = true;
        config.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<AuthContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true
        };
    });



builder.Services.BindConfigurations(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.Load("StudentPortal.Models"));
builder.Services.RegisterServices();
builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers(setupAction => { setupAction.ReturnHttpNotAcceptable = true; }).AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());


    }).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
}));

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transactions Micro-service", Version = "v1" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        },
    });
});

IAsyncPolicy<HttpResponseMessage>
    circuitBreakerPolicy =
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrTransientHttpStatusCode()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.ServiceUnavailable || msg.StatusCode == HttpStatusCode.InternalServerError)
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));

var financeBaseUri = builder.Configuration["appConfiguration:FinanceBaseUri"];
var libraryBaseUri = builder.Configuration["appConfiguration:LibraryBaseUri"];

builder.Services.AddHttpClient<IFinanceService, FinanceService>()
    .AddPolicyHandler(circuitBreakerPolicy)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(financeBaseUri));

builder.Services.AddHttpClient<ILibraryService, LibraryService>()
    .AddPolicyHandler(circuitBreakerPolicy)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(libraryBaseUri));

var app = builder.Build();

var serviceProvider = app.Services.CreateScope().ServiceProvider;

var appContext = serviceProvider.GetService<ApplicationContext>();
var authContext = serviceProvider.GetService<AuthContext>();
var logger = app.Services.CreateScope().ServiceProvider.GetService<ILogger>();

try
{
    await appContext?.RunPendingMigrationsAsync();
    await authContext?.RunPendingMigrationsAsync();
    await SeedApplicationData.EnsurePopulated(app);
}
catch (Exception e)
{
    logger?.LogError(e, string.Format("Migration Failed !:{0}", e.Message));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.ConfigureException(builder.Environment);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    logger?.LogInformation("Starting  Application...");
    app.Run();
}
catch (Exception ex)
{
    logger?.LogCritical(ex, "Application terminated unexpectedly!");
}
public partial class Program { }


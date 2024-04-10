using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StudentPortal.Data.Context;
using StudentPortal.Data.Extensions;

namespace StudentPortal.IntegrationsTests.Factories;

     public class StudentApiAppFactory<TProgram> :  WebApplicationFactory<TProgram> , IAsyncLifetime where TProgram : class 
     {
        private const string Username = "postgres";
        private const string Password = "yourStrong(!)Password";
        private const ushort PostgresPort = 5432;
        private readonly IContainer _postgresqlContainer;
        
        private WebApplicationFactory<Program> _webApplicationFactory;
        
        public StudentApiAppFactory()
        {
            _postgresqlContainer = new ContainerBuilder()
                .WithImage("postgres:latest")
                .WithPortBinding(PostgresPort)
                .WithEnvironment("POSTGRES_USER", Username)
                .WithEnvironment("POSTGRES_PASSWORD", Password)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(PostgresPort))
                .Build();
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var host = _postgresqlContainer.Hostname;
            var port = _postgresqlContainer.GetMappedPublicPort(PostgresPort);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AuthContext>));
                services.AddDbContext<AuthContext>(options =>
                {
                    options.UseNpgsql($"Server={host};Port={port};Database=AuthDb;User Id={Username};Password={Password}");
                });
                
                services.RemoveAll(typeof(DbContextOptions<ApplicationContext>));
                services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseNpgsql($"Server={host};Port={port};Database=StudentDb;User Id={Username};Password={Password}");
                });
            });
        }
        public async Task InitializeAsync()
        {
            await _postgresqlContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _postgresqlContainer.DisposeAsync();
        }
    }
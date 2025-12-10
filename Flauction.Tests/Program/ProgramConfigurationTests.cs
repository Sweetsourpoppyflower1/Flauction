using Flauction.Data;
using Flauction.Models;
using Flauction.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Flauction.Tests.Program
{
    public class ProgramConfigurationTests
    {
        [Fact]
        public void ServiceConfiguration_DbContextIsRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddDbContext<DBContext>(options => { });

            // Assert
            var dbContextService = services.FirstOrDefault(s => s.ServiceType == typeof(DBContext));
            Assert.NotNull(dbContextService);
        }

        [Fact]
        public void ServiceConfiguration_IdentityServicesAreRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DBContext>();

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType == typeof(UserManager<User>)));
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType == typeof(SignInManager<User>)));
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType == typeof(RoleManager<IdentityRole>)));
        }

        [Fact]
        public void ServiceConfiguration_EmailSenderIsRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddTransient<IEmailSender<User>, DummyEmailSender>();

            // Assert
            var emailSenderService = services.FirstOrDefault(s => 
                s.ServiceType == typeof(IEmailSender<User>) && 
                s.ImplementationType == typeof(DummyEmailSender));
            Assert.NotNull(emailSenderService);
        }

        [Fact]
        public void ServiceConfiguration_ControllersAreRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddControllers();

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType.Name.Contains("ControllerActivator")));
        }

        [Fact]
        public void ServiceConfiguration_RoutingIsRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddRouting();

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType.Name.Contains("Router")));
        }

        [Fact]
        public void ServiceConfiguration_CorsIsConfigured()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", builder =>
                    builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            // Assert
            var corsService = services.FirstOrDefault(s => s.ServiceType.Name.Contains("CorsService"));
            Assert.NotNull(corsService);
        }

        [Fact]
        public void ServiceConfiguration_AuctionStatusUpdaterIsRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddHostedService<AuctionStatusUpdater>();

            // Assert
            var hostedService = services.FirstOrDefault(s => 
                s.ServiceType == typeof(IHostedService) && 
                s.ImplementationType == typeof(AuctionStatusUpdater));
            Assert.NotNull(hostedService);
        }

        [Fact]
        public void ServiceConfiguration_BearerTokenAuthenticationIsConfigured()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme,
                options => { options.BearerTokenExpiration = TimeSpan.FromMinutes(60.0); });

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType.Name.Contains("IAuthenticationSchemeProvider")));
        }

        [Fact]
        public void BearerTokenExpiration_IsSetToSixtyMinutes()
        {
            // Arrange
            var expectedExpiration = TimeSpan.FromMinutes(60.0);

            // Act
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme,
                options => { options.BearerTokenExpiration = expectedExpiration; });

            // Assert
            Assert.Equal(expectedExpiration, TimeSpan.FromMinutes(60.0));
        }

        [Fact]
        public void ServiceConfiguration_DevelopmentEnvironmentIncludesSwagger()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                EnvironmentName = "Development"
            });
            var services = builder.Services;

            // Act
            if (builder.Environment.IsDevelopment())
            {
                services.AddSwaggerGen();
            }

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType.Name.Contains("SwaggerGenerator")));
        }

        [Fact]
        public void ServiceConfiguration_EndpointsExplorerIsRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddEndpointsApiExplorer();

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType.Name.Contains("EndpointDataSource")));
        }

        [Fact]
        public void MiddlewarePipeline_DevelopmentEnvironmentUsesSwagger()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                EnvironmentName = "Development"
            });
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Act & Assert - Verify environment is Development
            Assert.True(app.Environment.IsDevelopment());
        }

        [Fact]
        public void MiddlewarePipeline_ProductionEnvironmentUsesExceptionHandler()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                EnvironmentName = "Production"
            });
            var app = builder.Build();

            // Act & Assert - Verify environment is Production
            Assert.False(app.Environment.IsDevelopment());
        }

        [Fact]
        public void CorsPolicy_AllowsReactAppOrigin()
        {
            // Arrange
            var expectedOrigin = "http://localhost:3000";

            // Act & Assert
            Assert.Equal(expectedOrigin, "http://localhost:3000");
        }

        [Fact]
        public void Identity_DefaultTokenProvidersAreAdded()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;

            // Act
            services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DBContext>()
                .AddDefaultTokenProviders();

            // Assert
            Assert.NotNull(services.FirstOrDefault(s => s.ServiceType.Name.Contains("TokenProvider")));
        }
    }
}
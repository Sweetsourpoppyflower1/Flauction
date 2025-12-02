using Flauction.Data;
using Flauction.Models;
using Flauction.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NuGet.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Flauction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DBContext>();

            builder.Services.AddScoped<RoleManager<IdentityRole>>();
            builder.Services.AddTransient<IEmailSender<User>, DummyEmailSender>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Please enter a valid token",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                                Scheme = "Bearer",
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                    });
                });
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", builder =>
                    builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            builder.Services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme,
                options => { options.BearerTokenExpiration = TimeSpan.FromMinutes(60.0); });

            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddResponseCompression();
            }

            var app = builder.Build();

            IdentitySeeder.SeedAsync(app.Services, builder.Configuration).GetAwaiter().GetResult();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors("AllowReactApp");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapIdentityApi<User>();

            app.Run();
        }
    }
}
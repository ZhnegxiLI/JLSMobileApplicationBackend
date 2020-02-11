using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JLSDataAccess;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using JLSDataModel.Models.User;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using JLSMobileApplication.Heplers;
using Microsoft.AspNetCore.Identity.UI.Services;
using JLSMobileApplication.Services;
using Serilog;
using Microsoft.Extensions.Options;
using LjWebApplication.Middleware;
using Newtonsoft.Json.Serialization;

namespace JLSMobileApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddDbContext<JlsDbContext>(
             options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"),
                 builder => builder.UseRowNumberForPaging()) // IMPORTANT : use of ef function take() Skip()
             );
            services.AddDefaultIdentity<User>()
            .AddEntityFrameworkStores<JlsDbContext>();

            // Configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            /* Configure the log with serilog */
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            Log.Information("Start logging");

            //配置邮件发送
            services.AddTransient<IEmailService, EmailService>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //Email settings
                options.User.RequireUniqueEmail = true;
            });

            //配置JWT 密钥
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
             .AddJwtBearer("JwtBearer", jwtBearerOptions =>
             {
                 jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtSecret)),
                     //ValidateIssuer = true,
                    ValidIssuer = appSettings.JwtIssuer,
                   // ValidateAudience = true,
                    ValidAudience = appSettings.JwtAudience,
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
             });

            // 配置跨域
            services.AddCors(options =>
            {
                options.AddPolicy(appSettings.MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080", "ionic://localhost", "http://localhost", "http://localhost:8100", "http://176.176.221.117", "capacitor://localhost")
                            .AllowAnyHeader()
                            .WithMethods()
                            .AllowCredentials(); ;
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(Configuration.GetSection("AppSettings:MyAllowSpecificOrigins").Value);

           app.UseErrorHandling();

            app.UseAuthentication();

           app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}

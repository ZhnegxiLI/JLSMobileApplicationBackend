using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JLSConsole.Heplers;
using JLSConsoleApplication.Middleware;
using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSDataAccess.Repositories;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace JLSConsoleApplication
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";//Todo add into the appsettings
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options=>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            }) ;

            services.AddDbContext<JlsDbContext>(
             options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"),
                 builder => builder.UseRowNumberForPaging()) // IMPORTANT : use of ef function take() Skip()
             );

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200", "http://localhost:4201")
                            .AllowAnyHeader()
                            .WithMethods()
                            .AllowCredentials(); ;
                    });
            });

            services.AddDefaultIdentity<User>()
            .AddEntityFrameworkStores<JlsDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //Email settings
                //options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;


            });

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

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
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     // ValidIssuer = appSettings.Site,
                     //ValidAudience = appSettings.Audience,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ClockSkew = TimeSpan.Zero//TimeSpan.FromMinutes(5)
                 };
             });


            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IReferenceRepository, ReferenceRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


       

            app.UseCors(MyAllowSpecificOrigins);
            //});
            var cachePeriod = env.IsDevelopment() ? "600" : "604800"; // Todo add into the appsettings缓存时间 

            app.UseStaticFiles(new StaticFileOptions //TODO, if not exists create配置静态文件夹
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), "images")),// Todo add into the configure
                RequestPath = "/images",
                OnPrepareResponse = ctx =>
                {
                    // Requires the following import:
                    // using Microsoft.AspNetCore.Http;
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

            app.UseErrorHandling();

            app.UseAuthentication();

            //var cachePeriod = env.IsDevelopment() ? "600" : "604800"; // Todo add into the appsettings缓存时间 

            //app.UseStaticFiles(new StaticFileOptions //TODO, if not exists create配置静态文件夹
            //{
            //    FileProvider = new PhysicalFileProvider(
            //       Path.Combine(Directory.GetCurrentDirectory(), "images")),// Todo add into the configure
            //    RequestPath = "/images",
            //    OnPrepareResponse = ctx =>
            //    {
            //        // Requires the following import:
            //        // using Microsoft.AspNetCore.Http;
            //        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
            //    }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

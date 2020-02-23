﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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

                // Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //Email settings
                //options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;


            });

            /*services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
             .AddJwtBearer("JwtBearer", jwtBearerOptions =>
             {
                 jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("JWT:Secret").Value)),
                     ValidateIssuer = true,
                     ValidIssuer = Configuration.GetSection("JWT:Issuer").Value,
                     ValidateAudience = true,
                     ValidAudience = Configuration.GetSection("JWT:Audience").Value,
                     ValidateLifetime = true, //validate the expiration and not before values in the token
                     ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                 };
             });*/

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

            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

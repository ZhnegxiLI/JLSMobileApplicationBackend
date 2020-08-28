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
using JLSDataAccess.Interfaces;
using JLSDataAccess.Repositories;
using JLSDataModel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Hangfire;
using JLSMobileApplication.hubs;

namespace JLSMobileApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            /* Init autoMapper: map the relation for object */
            services.AddAutoMapper();

            /* Init mvc */
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                /* Configure the json output date format */
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            /* Init dbContext */
            services.AddDbContext<JlsDbContext>(
             /* Get connections string from config */
             options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"), 
                 /* Configure the pagination (take() skip())*/
                 builder => builder.UseRowNumberForPaging()) 
             );

            /* Init identity */
            services.AddDefaultIdentity<User>()
            .AddRoles<IdentityRole<int>>() // user id: int
            .AddEntityFrameworkStores<JlsDbContext>();

            // Configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            /* Init  serilog */
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            Log.Information("Start logging");

            // Inject email service (send email function)
            services.AddTransient<IEmailService, EmailService>();
            // Inject export service  
            services.AddTransient<IExportService, ExportService>();
            // Inject format email and message service 
            services.AddTransient<ISendEmailAndMessageService, SendEmailAndMessageService>();

            /* Init identity password option */
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            });

            // Init jwt credential 
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            /* Init jwt authorization */
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
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ClockSkew = TimeSpan.FromMinutes(5)
                 };
             });

            /* Init cors: todo place the allowed origins in the appsettings */
            var origins = appSettings.AllowedOrigins.Split(";");
            services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins(origins)
                            .AllowAnyHeader()
                            .WithMethods()
                            .AllowCredentials();
                    });
            });

            /* Init signalR (use for realtime data exchange, use for chat)*/
            services.AddSignalR();

            /* Init handFire (timer: trigger some action in some periode, use for send email) */
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnectionString")));

            /* Inject different reporsitory */
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IReferenceRepository, ReferenceRepository>();
            services.AddScoped<IAdressRepository, AdressRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IAnalyticsReporsitory, AnalyticsRepository>();

            services.AddScoped<TokenModel>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, JlsDbContext context, UserManager<User> userManager, ISendEmailAndMessageService timerEmailService)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            /* Configure Hangfire trigger */
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            /* Running time: 1min, Action: SendEmailInDb */
            RecurringJob.AddOrUpdate(() => timerEmailService.SendEmailInDb(), Cron.Minutely);

            /* Apply cors rule */
            app.UseCors("_myAllowSpecificOrigins");

            /* Set cache time : todo place into appsettings */
            var cachePeriod = env.IsDevelopment() ? "600" : "604800";


            /* This function will create new folder if folder not exist and return current folder if exists */
            System.IO.Directory.CreateDirectory("/images");
            /* Configure staticFiles path /images */
            app.UseStaticFiles(new StaticFileOptions 
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), "images")),// Todo add into the configure
                RequestPath = "/images",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

            /* Initialization action: create a super admin in database */
            Initialization.AddAdminUser(userManager, context);

            app.UseErrorHandling();

            /* JWT authorization */
            app.UseAuthentication();

            /* Configure router path of signalR */
            app.UseSignalR(options =>
            {
                options.MapHub<MessageHub>("/MessageHub");
            });

            /* Configure mvc model */
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}

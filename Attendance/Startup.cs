using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Attendance.Services;
using Attendance.Services.Mappings;
using Attendance.Services.Providers;
using Attendance.Services.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Operations.Data;
using System;
using System.IO;
using Westwind.AspNetCore.LiveReload;

namespace Attendance
{
    public class Startup
    {
        private IWebHostEnvironment _env { get; }
        private IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLiveReload(config =>
            {
                config.LiveReloadEnabled = true;
                config.ClientFileExtensions = ".css,.js,.htm,.html,.ts,.razor,.cs,.cshtml,.jsx,.tsx";
                config.FolderToMonitor = Path.GetFullPath(Path.Combine(_env.ContentRootPath, ".."));
                config.ServerRefreshTimeout = 1000;
                config.FileInclusionFilter = path =>
                {
                    if (path.Contains("node_modules") ||
                        path.Contains("\\obj") ||
                        path.Contains("\\bin")
                    )
                    {
                        return FileInclusionModes.DontRefresh;
                    }
                    return FileInclusionModes.ContinueProcessing;
                };
            });
            services.AddRazorPages(options =>
            {
                options.Conventions
                .AddPageRoute("/VettingInfo/Index", "")
                .AllowAnonymousToPage("/Login")
                ;
            })

                .AddRazorRuntimeCompilation()
                ;
            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.Filters.Add(new HandlerFilter());

            })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddRazorRuntimeCompilation();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddDbContext<AttendanceContext>(options => options.UseSqlServer(_configuration.GetConnectionString("AttendanceContext")));
            services.AddDbContext<OperationsDbContext>(option => option.UseSqlServer(_configuration.GetConnectionString("Operations")));
            services.AddLogging();
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AttendanceContext>()
            .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Login");
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBriefcaseRepository, BriefcaseRepository>();
            services.AddSingleton<IBriefcaseImportRepository, BriefcaseImportRepository>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISqlConnectionProvider, SqlCompactConnectionProvider>();
            services.AddScoped<ISqlCeEngineProvider, SqlCeEngineProvider>();
            services.AddScoped<IBriefcaseService, BriefcaseService>();
            services.AddScoped<INewBriefcaseService, NewBriefcaseService>();
            services.AddScoped<ICompactVettingsService, CompactVettingsService>();
            services.AddScoped<IVettingInfosService, VettingInfosService>();
            services.AddScoped<IVettingsService, VettingsService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ISqlSettingBuilder, SqlConnectionBuilder>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Login";
                options.ReturnUrlParameter = "returnUrl";
                options.SlidingExpiration = true;
            });
            services.AddAntiforgery();
            services.AddAutoMapper(typeof(AutoMapping));

        }

        public void Configure(IApplicationBuilder app, IAntiforgery antiforgery)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseLiveReload();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(next => context =>
            {
                if (context.Request.Path.StartsWithSegments("/Briefcases"))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false });
                }
                return next(context);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().RequireAuthorization();
                endpoints.MapControllerRoute(
                 name: "Briefcases",
                 pattern: "/Briefcases/{*path}",
                 defaults: new { page = "/Briefcases/Index" });
                //endpoints.MapFallbackToPage("/Briefcases/{*path}", "/Briefcases");
            });

            if (_env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:5173/");
                });
            }
        }
    }
}
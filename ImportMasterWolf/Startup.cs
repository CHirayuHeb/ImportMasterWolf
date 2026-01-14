using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.DBConnect;

namespace ImportMasterWolf
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().AddControllersAsServices();
            services.AddDbContext<HRMS>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HRMS")));
            services.AddDbContext<LAMP>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LAMP")));
            services.AddDbContext<IT>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IT")));
            services.AddDbContext<PrdInvBF_Prd>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PrdInvBf_Prd")));
            services.AddDbContext<WolfApproveCore_thaistanley>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("WolfApproveCore.thaistanley")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(x => {
                    x.Cookie.Name = "Remember";
                    x.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    x.LoginPath = "/Login/Index"; //path login
                    x.LogoutPath = "/Login/Logout"; //path loout
                    x.AccessDeniedPath = "/ErrorCase/Index";
                    x.ReturnUrlParameter = "returnurl";
                    x.Cookie.IsEssential = true;
                    x.SlidingExpiration = true; // ✅ ต่อเวลาอัตโนมัติเมื่อมีการใช้งาน
                });

            services.AddAuthorization(x =>
            {
                x.AddPolicy("Checked", y => { y.RequireClaim(ClaimTypes.Country, "ImportMasterWolf"); });
                x.AddPolicy("perUser", y => { y.RequireClaim(ClaimTypes.Role, GlobalVariable.perUser); });
                x.AddPolicy("perAdmin", y => { y.RequireClaim(ClaimTypes.Role, GlobalVariable.perAdmin); });
                x.AddPolicy("perHCM", y => { y.RequireClaim(ClaimTypes.Role, GlobalVariable.perHCM); });
                x.AddPolicy("perGeneral", y => { y.RequireClaim(ClaimTypes.Role, GlobalVariable.perUser, GlobalVariable.perAdmin, GlobalVariable.perHCM); });
                x.AddPolicy("perEmergency", y => { y.RequireClaim(ClaimTypes.Role, GlobalVariable.EmergencyPermission); });
            });


            services.AddMemoryCache();
            services.AddSession(x => x.Cookie.IsEssential = true);
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}");
            });
            app.UseCookiePolicy();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kudvenkat.DataAccess.Models;
using kudvenkat.Repositories;
using kudvenkat.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace kudvenkat {
    public class Startup {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config) {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContextPool<AppDbContext>(options => options.UseMySql(_config.GetConnectionString("EmployeeDBConnection"), b => b.MigrationsAssembly("kudvenkat.DataAccess")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.ConfigureApplicationCookie(options => {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            services.AddAuthorization(options => {
                options.AddPolicy(nameof(AuthPolicies.DeleteRolePolicy),
                    policy => policy.RequireClaim("Delete Role", "true"));

                options.AddPolicy(nameof(AuthPolicies.EditRolePolicy),
                   policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                options.AddPolicy(nameof(AuthPolicies.AdminRolePolicy),
                    policy => policy.RequireClaim("Admin"));
            });

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

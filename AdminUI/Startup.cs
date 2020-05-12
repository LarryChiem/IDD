using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Areas.Identity.Data;
using AdminUI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Common.Data;

namespace AdminUI
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
            services.AddControllersWithViews();
            services.AddProgressiveWebApp();
            services.AddMvc().AddRazorRuntimeCompilation();

            services.AddDbContext<SubmissionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureDB"), 
                    b => b.MigrationsAssembly("AdminUI")));

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AdminUIUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedData(roleManager, userManager);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Homescreen route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

            });
        }

        private async void SeedData(RoleManager<IdentityRole> roleManager, UserManager<AdminUIUser> userManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);

        }
        private async Task SeedUsers(UserManager<AdminUIUser> userManager)
        {
            var user = await userManager.FindByNameAsync("Admin@AdminUI.com");
            if (user == null)
            {
                user = new AdminUIUser
                {
                    UserName = "Admin",
                    Email = "Admin@AdminUI.com",
                    Name = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Administrator");
            }

            user = await userManager.FindByNameAsync("Employee");
            if (user == null)
            {
                user = new AdminUIUser
                {
                    UserName = "Employee",
                    Email = "Employee@AdminUI.com",
                    Name = "Employee",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "password");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Employee");
            }
        }

        private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var result = await roleManager.RoleExistsAsync("Employee");
            if (!result)
                await roleManager.CreateAsync(new IdentityRole{ Name = "Employee"});

            result = await roleManager.RoleExistsAsync("Administrator");
            if (!result)
                await roleManager.CreateAsync(new IdentityRole{ Name = "Administrator"});
        }
    }
}

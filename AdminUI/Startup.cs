using System.Linq;
using AdminUI.Data;
using AdminUI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Common.Data;
using Common.MigrationUtilities;

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

            services.AddDbContext<PayPeriodContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureDB"), 
                    b => b.MigrationsAssembly("AdminUI")));


            DbContextOptions<SubmissionContext> options = new DbContextOptions<SubmissionContext>();
            options.UseSqlServer(Configuration.GetConnectionString("AzureDB"));
            SubmissionContext subcontext = new SubmissionContext(options);
            UriMigrationHelper.UpgradeCommaFixSubmissions(subcontext);
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager, PayPeriodContext ppcontext, SubmissionContext scontext)
        {
            UpdateDatabase(app);
            GlobalVariables.CurrentPayPeriod = ppcontext.PayPeriods.FirstOrDefault(p => p.Current);
            SeedRoles(roleManager);

            if (env.IsDevelopment())
            {
                MockData.InitializeSubmissionDb(scontext);
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

        private async void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var result = await roleManager.RoleExistsAsync("Employee");
            if (!result)
                await roleManager.CreateAsync(new IdentityRole{ Name = "Employee"});

            result = await roleManager.RoleExistsAsync("Administrator");
            if (!result)
                await roleManager.CreateAsync(new IdentityRole{ Name = "Administrator"});
        }
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SubmissionContext>())
                {
                    context.Database.Migrate();
                }
                using (var context = serviceScope.ServiceProvider.GetService<PayPeriodContext>())
                {
                    context.Database.Migrate();
                }
                using (var context = serviceScope.ServiceProvider.GetService<AdminUIUserContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}

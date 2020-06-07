using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using Appserver.Data;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.MigrationUtilities;
using Microsoft.Azure.Documents.SystemFunctions;

namespace Appserver
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
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson();

            services.AddDbContext<SubmissionStagingContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureDB"),
                    b => b.MigrationsAssembly("Appserver")));

            services.AddDbContext<SubmissionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureDB"),
                    b => b.MigrationsAssembly("AdminUI")));

            DbContextOptions<SubmissionStagingContext> options = new DbContextOptions<SubmissionStagingContext>();
            SubmissionStagingContext context = new SubmissionStagingContext(options);
            ModelUtils.UpgradeCommaFixStaging(context);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UpdateDatabase(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors(builder =>
              builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
            // Homescreen route
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/");

            // Image upload route
            endpoints.MapControllerRoute(
                name: "image_upload_route",
                pattern: "{controller=Home}/{action=Timesheet}");

            // Admin login route
            endpoints.MapControllerRoute(
                name: "admin_login_route",
                pattern: "{controller=Admin}/{action=Login}/");

            // Check if Timesheet Ready
            endpoints.MapControllerRoute(
                name: "timesheet_ready_route",
                pattern: "{controller=Timesheet}/{action=Ready}/");

            // Check if Timesheet Ready
            endpoints.MapControllerRoute(
                name: "test_timesheet_ready_route",
                pattern: "{controller=Timesheet}/{action=ReadyTest}/");

                // Validate Timesheet
                endpoints.MapControllerRoute(
                name: "timesheet_validate_route",
                pattern: "{controller=Timesheet}/{action=Validate}/");

            // Submit Timesheet
            endpoints.MapControllerRoute(
                name: "timesheet_submit_route",
                pattern: "{controller=Timesheet}/{action=Submit}/");

            // Check Timesheet Received
            endpoints.MapControllerRoute(
                name: "timesheet_received_route",
                pattern: "{controller=Timesheet}/{action=Received}/");

            // Upload Documents as form
            endpoints.MapControllerRoute(
                name: "document_upload_form_route",
                pattern: "{controller=ImageUpload}/{action=DocAsForm}");
        });
        }
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SubmissionStagingContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}

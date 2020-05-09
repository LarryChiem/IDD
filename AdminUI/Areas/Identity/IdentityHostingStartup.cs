using System;
using AdminUI.Areas.Identity.Data;
using AdminUI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AdminUI.Areas.Identity.IdentityHostingStartup))]
namespace AdminUI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AdminUIUserContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AzureDB")));

                services.AddDefaultIdentity<AdminUIUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<AdminUIUserContext>();
            });
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ContactManagerAppTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureServices(services => {
            
                var descriptor = services.SingleOrDefault(temp => 
                temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if(descriptor != null)
                {
                    //services.Remove(descriptor);
                    services.Remove(
               services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))
           );

                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestingDB");
                });


            });
        }
    }
}

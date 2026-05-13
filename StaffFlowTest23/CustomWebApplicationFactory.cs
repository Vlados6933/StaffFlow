using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using StaffFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StaffFlowTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                //var descriper = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                //if (descriper != null)
                //    services.Remove(descriper);

                //var optionsDescriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions));

                //if (optionsDescriptor != null)
                //    services.Remove(optionsDescriptor);

                //var dbConnectionDescriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(System.Data.Common.DbConnection));

                //if (dbConnectionDescriptor != null)    
                //    services.Remove(dbConnectionDescriptor);            

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("DatabaseForTesting");
                });
            });
        }
    }
}

using DinkToPdf.Contracts;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;
using StaffFlow.Filters.ActionFilters;

namespace StaffFlow.Extensions.StartupExtensions
{
    public static class ConfigureSreviseExtension
    {
        public static IServiceCollection ConfigureSrevise(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews(options =>
            {
                var logger = services.BuildServiceProvider().
                GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "MyKey-From-Global",
                    Value = "MyValue-From-Global",
                    Order = 2
                });
            });

            services.AddTransient<ResponseHeaderActionFilter>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsService, PersonsService>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddHttpLogging();
            
            return services;
        }
    }
}

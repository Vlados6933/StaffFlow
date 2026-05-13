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
            services.AddControllersWithViews();

            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsService, PersonsService>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddHttpLogging();
            
            return services;
        }
    }
}

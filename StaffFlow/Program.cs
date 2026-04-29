using DinkToPdf;
using DinkToPdf.Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StaffFlow.Extensions.StartupExtensions;

namespace StaffFlow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Serilog
            builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider service, LoggerConfiguration loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(service);
            });

            builder.Services.ConfigureSrevise(builder.Configuration);

            if (builder.Environment.IsEnvironment("Test") == false)
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                });
            }

            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpLogging();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}

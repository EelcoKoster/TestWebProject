using DataLayer;
using DataLayer.Interfaces;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace FunctionDI
{
    public sealed class Program
    {
        public static void Main()
        {
            var host = new HostBuilder();
            //host.ConfigureSerilog();
            host.ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson());
            host.ConfigureOpenApi();
            host.ConfigureAppConfiguration(config => config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables()
                    .AddJsonFile("local.settings.json"));

            host.ConfigureServices(svc =>
            {
                string dbPath = Environment.GetEnvironmentVariable("DbPath");
                var contextBuilder = new DbContextOptionsBuilder<DataContext>().UseSqlite($"Data Source={dbPath}");
                using (DataContext dataContext = new DataContext(contextBuilder.Options))
                {
                    Console.WriteLine("Check for DB migrations...");
                    dataContext.Database.Migrate();
                }
                svc.AddDbContext<DataContext>(options => options.UseSqlite($"Data Source={dbPath}"));
                Console.WriteLine($"Created dbcontext with path {dbPath}");
                svc.AddScoped<IClientHandler, ClientHandler>();
                svc.AddScoped<ICompanyHandler, CompanyHandler>();
                svc.AddScoped<ICompanyContactHandler, CompanyContactHandler>();
            });

            var func = host.Build();
            func.Run();
        }
    }
}
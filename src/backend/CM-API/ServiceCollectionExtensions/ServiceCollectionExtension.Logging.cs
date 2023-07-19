using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace FunctionDI.ServiceCollectionExtensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static HostBuilder ConfigureSerilog(this HostBuilder builder) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Version", "1.0.0")
                .WriteTo.Console()
                .CreateLogger();


            builder.ConfigureServices((hostContext, services) =>
             {
                 services.AddSingleton<ILogger>(Log.Logger);
             });

            return builder;
        }
    }
}

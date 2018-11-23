namespace PsychologyBot
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddAzureWebAppDiagnostics();
                })
                .UseSerilog(
                    (hostingContext, loggerConfiguration) =>
                        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
                .UseStartup<Startup>()
                .Build();
        }
    }
}
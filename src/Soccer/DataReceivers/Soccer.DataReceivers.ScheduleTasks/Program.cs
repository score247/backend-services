namespace Soccer.DataReceivers.ScheduleTasks
{
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Soccer.DataReceivers.ScheduleTasks._Shared.Helpers;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            return WebHost.CreateDefaultBuilder(args)
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddProvider(new HangfireLogProvider());
               })
               .UseConfiguration(configuration)
               .UseStartup<Startup>()
               .UseUrls($"http://0.0.0.0:{configuration["Port"]}");
        }
    }
}
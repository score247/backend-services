﻿namespace Soccer.EventProcessors
{
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

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
               })
               .UseConfiguration(configuration)
               .UseStartup<Startup>()
               .UseUrls($"http://0.0.0.0:{configuration["Port"]}");
        }
    }
}
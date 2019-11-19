namespace Soccer.API
{
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

#pragma warning disable S1118 // Utility classes should not have public constructors

    public class Program
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
               .UseStartup<Startup>()
               .UseUrls($"http://localhost:{configuration["HostingPort"]}");
        }
    }

#pragma warning restore S1118 // Utility classes shold not have public constructors
}
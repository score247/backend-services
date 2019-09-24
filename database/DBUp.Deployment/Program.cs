using DbUp;
using DbUp.Helpers;
using DBUp.Deployment.PreProcessors;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DBUp.Deployment
{
    public static class Program
    {
        static int Main(string[] args)
        {
            // TODO configurations for dev, test, main environments
            var connectionString =
                args.FirstOrDefault()
                ?? "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_dev1;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";

            var dir = new DirectoryInfo("../../../../store-procedures").FullName;            

            var subDirs = Directory.GetDirectories(dir);

            foreach (var sub in subDirs)
            {
                Console.WriteLine($"Deploying scripts in {sub}");

                var upgrader =
                    DeployChanges.To
                        .MySqlDatabase(connectionString)
                        .WithPreprocessor(new DelimiterPreProcessor())
                        .WithScriptsFromFileSystem(sub)
                        .LogToConsole()
                        .JournalTo(new NullJournal())
                        .Build();

                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
#if DEBUG
                    Console.ReadLine();
#endif
                    return -1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}

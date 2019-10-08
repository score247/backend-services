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
        private static int Main(string[] args)
        {
            //LOCAL DEV
            // var connectionString = "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_dev;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";
            var connectionString = "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_dev;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";
            // var connectionString = "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_dev2;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";

            //LOCAL TEST
            // var connectionString = "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_test;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";

            //LOCAL MAIN
            // var connectionString = "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_main;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";

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
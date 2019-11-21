﻿using System;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Helpers;
using DBUp.Deployment.Models;
using DBUp.Deployment.PreProcessors;
using Newtonsoft.Json;

namespace DBUp.Deployment
{
    public static class Program
    {

        private static int Main(string[] args)
        {
            var settingPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "app-settings.dev.json");
            var settings = File.ReadAllText(settingPath);
            var connectionConfiguration = JsonConvert.DeserializeObject<ConnectionConfiguration>(settings);

            foreach (var config in connectionConfiguration.Connections)
            {
                // InstallNewDatabase(config.ToString());
                Console.ForegroundColor = ConsoleColor.Blue;

                Console.WriteLine("==============");
                Console.WriteLine($"Deploying scripts to {config.Database} database");
                Console.WriteLine();

                Console.ResetColor();

                InstallStoredProcedures(config.ToString());
                //InstallSprintChanges(config.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }


        public static int InstallNewDatabase(string connectionString)
        {
            //1. run schema scripts: create tables + indexes
            InstallSchemas(connectionString);

            //2. create event schedulers

            //3. run sp
            InstallStoredProcedures(connectionString);

            return 0;
        }

        public static int InstallSprintChanges(string connectionString) 
        {
            //1. run schema scripts: create tables + indexes
            InstallSchemas(connectionString, true);

            //2. create event schedulers

            //3. run sp
            InstallStoredProcedures(connectionString);

            return 0;
        }

        public static int InstallSchemas(string connectionString, bool forSprint = false)
        {

            var dir = new DirectoryInfo("../../../../schema").FullName;

            if (forSprint)
            {
                dir = new DirectoryInfo("../../../../current-sprint-schema-changes").FullName;
            }

            return RunScripts(connectionString, new string[] { dir });
        }

        public static int InstallStoredProcedures(string connectionString)
        {
            var dir = new DirectoryInfo("../../../../store-procedures").FullName;
            var subDirs = Directory.GetDirectories(dir);

            return RunScripts(connectionString, subDirs);
        }

        private static int RunScripts(string connectionString, string[] subDirs)
        {
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

            return 0;
        }
    }
}
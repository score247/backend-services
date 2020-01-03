using System;
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
            var environment = "test";
            var settingPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"app-settings.{environment}.json");
            var settings = File.ReadAllText(settingPath);
            var connectionConfiguration = JsonConvert.DeserializeObject<ConnectionConfiguration>(settings);

            foreach (var config in connectionConfiguration.Connections)
            {
                Console.ForegroundColor = ConsoleColor.Blue;

                Console.WriteLine("==============");
                Console.WriteLine($"Deploying scripts to {config.Database} database");
                Console.WriteLine();

                Console.ResetColor();

                //InstallNewDatabase(config.ToString()); // Only run when you create new database

                InstallStoredProcedures(config.ToString());
                //InstallReProcessStoredProcedures(config.ToString(), environment, config.Database);

                //InstallEventSchedulers(config.ToString(), environment, config.Database);

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

            //4. init data
            InitData(connectionString);

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

        public static int InstallEventSchedulers(string connectionString, string environment, string dbName)
        {
            var dir = new DirectoryInfo("../../../../event-scheduler").FullName;

            return RunScripts(connectionString, new string[] { dir }, true, environment, dbName);
        }

        public static int InitData(string connectionString)
        {
            var dir = new DirectoryInfo("../../../../import").FullName;

            var subDirs = Directory.GetDirectories(dir);

            return RunScripts(connectionString, subDirs);
        }

        public static int InstallStoredProcedures(string connectionString)
        {
            var dir = new DirectoryInfo("../../../../store-procedures").FullName;
            var subDirs = Directory.GetDirectories(dir);

            return RunScripts(connectionString, subDirs);
        }

        public static int InstallReProcessStoredProcedures(string connectionString, string environment, string dbName)
        {
            var dir = new DirectoryInfo("../../../../reprocess-store-procedures").FullName;
            var subDirs = Directory.GetDirectories(dir);

            return RunScripts(connectionString, subDirs, true, environment, dbName);
        }

        private static int RunScripts(string connectionString, string[] subDirs, bool changeDbName = false, string environment = "", string dbName = "")
        {
            foreach (var sub in subDirs)
            {
                Console.WriteLine($"Deploying scripts in {sub}");

                var upgrader = BuildUpgrader(connectionString, sub, changeDbName, environment, dbName);

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

        private static DbUp.Engine.UpgradeEngine BuildUpgrader(string connectionString, string sub, bool changeDbName = false, string environment = "", string dbName = "")
           => changeDbName
               ? DeployChanges.To
                           .MySqlDatabase(connectionString)
                           .WithPreprocessor(new DatabaseNamePreProcessor(dbName, environment))
                           .WithPreprocessor(new DelimiterPreProcessor())
                           .WithScriptsFromFileSystem(sub)
                           .LogToConsole()
                           .JournalTo(new NullJournal())
                           .Build()
               : DeployChanges.To
                       .MySqlDatabase(connectionString)
                       .WithPreprocessor(new DelimiterPreProcessor())
                       .WithScriptsFromFileSystem(sub)
                       .LogToConsole()
                       .JournalTo(new NullJournal())
                       .Build();
    }
}
﻿using DbUp;
using DbUp.Helpers;
using DBUp.Deployment.PreProcessors;
using System;
using System.Linq;
using System.Reflection;

namespace DBUp.Deployment
{
    public static class Program
    {
        static int Main(string[] args)
        {
            var connectionString =
                args.FirstOrDefault()
                ?? "Data Source=10.18.200.109;Port=3386;Initial Catalog=score247_local_dev1;Persist Security Info=True;User ID=user;Password=1234aa;Allow User Variables=True;";

            var upgrader =
                DeployChanges.To
                    .MySqlDatabase(connectionString)
                    .WithPreprocessor(new DelimiterPreProcessor())
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
﻿namespace Soccer.API.Shared.Configurations
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IAppSettings
    {
        bool EnabledDatabaseMigration { get; }

        int NumberOfTopMatches { get; }

        int NumOfDaysToShowOddsBeforeKickoffDate { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(
            IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            EnabledDatabaseMigration = GetValue<bool>(nameof(EnabledDatabaseMigration));
            NumberOfTopMatches = GetValue<int>(nameof(NumberOfTopMatches));
            NumOfDaysToShowOddsBeforeKickoffDate = GetValue<int>(nameof(NumOfDaysToShowOddsBeforeKickoffDate));

            if (NumberOfTopMatches <= 0)
            {
                NumberOfTopMatches = int.MaxValue;
            }
        }

        public bool EnabledDatabaseMigration { get; }

        public int NumberOfTopMatches { get; }

        public int NumOfDaysToShowOddsBeforeKickoffDate { get; }

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }
    }
}
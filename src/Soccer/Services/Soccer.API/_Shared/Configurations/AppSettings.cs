namespace Soccer.API.Shared.Configurations
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IAppSettings
    {
        bool EnabledDatabaseMigration { get; }

        int NumberOfTopMatches { get; }

        int NumOfDaysToShowOddsBeforeKickoffDate { get; }

        int MatchShortCacheTimeDuration { get; }

        int MatchLongCacheTimeDuration { get; }

        int OddsShortCacheTimeDuration { get; }

        int OddsLongCacheTimeDuration { get; }

        int NumOfMinutesToLoadClosedMatch { get; }
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
            MatchShortCacheTimeDuration = GetValue<int>(nameof(MatchShortCacheTimeDuration));
            MatchLongCacheTimeDuration = GetValue<int>(nameof(MatchLongCacheTimeDuration));
            OddsShortCacheTimeDuration = GetValue<int>(nameof(OddsShortCacheTimeDuration));
            OddsLongCacheTimeDuration = GetValue<int>(nameof(OddsLongCacheTimeDuration));

            if (NumberOfTopMatches <= 0)
            {
                NumberOfTopMatches = int.MaxValue;
            }
        }

        public bool EnabledDatabaseMigration { get; }

        public int NumberOfTopMatches { get; }

        public int NumOfDaysToShowOddsBeforeKickoffDate { get; }

        public int MatchShortCacheTimeDuration { get; }

        public int MatchLongCacheTimeDuration { get; }

        public int OddsShortCacheTimeDuration { get; }

        public int OddsLongCacheTimeDuration { get; }

        public int NumOfMinutesToLoadClosedMatch { get; }

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
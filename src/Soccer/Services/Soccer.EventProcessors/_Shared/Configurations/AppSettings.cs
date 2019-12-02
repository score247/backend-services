namespace Soccer.EventProcessors.Shared.Configurations
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IAppSettings
    {
        int NumOfDaysToShowOddsBeforeKickoffDate { get; }

        int CorrectTimelineSpanInMinutes { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(
            IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            NumOfDaysToShowOddsBeforeKickoffDate = GetValue<int>(nameof(NumOfDaysToShowOddsBeforeKickoffDate));
            CorrectTimelineSpanInMinutes = GetValue<int>(nameof(CorrectTimelineSpanInMinutes));
        }

        public int NumOfDaysToShowOddsBeforeKickoffDate { get; }

        public int CorrectTimelineSpanInMinutes { get; }

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
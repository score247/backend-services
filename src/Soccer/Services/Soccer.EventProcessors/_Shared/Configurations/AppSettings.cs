namespace Soccer.EventProcessors.Shared.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IAppSettings
    {
        int NumOfDaysToShowOddsBeforeKickoffDate { get; }
        IEnumerable<InternationalLeague> InternationalLeagues { get; }
    }

    public class InternationalLeague
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(
            IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            NumOfDaysToShowOddsBeforeKickoffDate = GetValue<int>(nameof(NumOfDaysToShowOddsBeforeKickoffDate));

            var internationalLeagues = new List<InternationalLeague>();
            configuration.Bind("InternationalLeagues", internationalLeagues);
            InternationalLeagues = internationalLeagues;
        }

        public int NumOfDaysToShowOddsBeforeKickoffDate { get; }

        public IEnumerable<InternationalLeague> InternationalLeagues { get; }

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
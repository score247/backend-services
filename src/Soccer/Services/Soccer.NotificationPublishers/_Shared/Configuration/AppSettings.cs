using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Soccer.NotificationPublishers._Shared.Configuration
{
    public interface IAppSettings
    {
        IEnumerable<string> AllowedCorsUrls { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(
            IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            AllowedCorsUrls = configuration.GetSection(nameof(AllowedCorsUrls)).Get<string[]>()?.AsEnumerable() ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> AllowedCorsUrls { get; }

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

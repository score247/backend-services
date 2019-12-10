namespace Soccer.DataReceivers.EventListeners.Shared.Configurations
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IAppSettings
    {
        string EncryptKey { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            EncryptKey = GetValue<string>(nameof(EncryptKey));
        }

        public string EncryptKey { get; }

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
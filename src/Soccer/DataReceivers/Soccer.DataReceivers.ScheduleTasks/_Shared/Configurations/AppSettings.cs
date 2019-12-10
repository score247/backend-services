using System;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace Soccer.DataReceivers.ScheduleTasks.Shared.Configurations
{
    public interface IAppSettings
    {
        ScheduleTasksSettings ScheduleTasksSettings { get; }

        bool EnabledHangfireUI { get; }

        string EncryptKey { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            var scheduleTaskSetting = new ScheduleTasksSettings();
            configuration.Bind("ScheduleTasks", scheduleTaskSetting);
            ScheduleTasksSettings = scheduleTaskSetting;

            EnabledHangfireUI = GetValue<bool>(nameof(EnabledHangfireUI));
            EncryptKey = GetValue<string>(nameof(EncryptKey));
        }

        public ScheduleTasksSettings ScheduleTasksSettings { get; }

        public bool EnabledHangfireUI { get; }

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
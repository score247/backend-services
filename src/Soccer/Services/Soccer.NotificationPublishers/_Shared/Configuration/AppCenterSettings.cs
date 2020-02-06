namespace Soccer.NotificationPublishers._Shared.Configuration
{
    public interface IAppCenterSettings
    {
#pragma warning disable S3996 // URI properties should not be strings
        string ServiceUrl { get; set; }
#pragma warning restore S3996 // URI properties should not be strings

        string Organization { get; set; }

        string ApiKey { get; set; }

        string DeviceTarget { get; set; }

        string AndroidAppName { get; set; }

        string iOSAppName { get; set; }
    }

    public class AppCenterSettings : IAppCenterSettings
    {
        public string ServiceUrl { get; set; }

        public string Organization { get; set; }

        public string ApiKey { get; set; }

        public string DeviceTarget { get; set; }

        public string AndroidAppName { get; set; }

        public string iOSAppName { get; set; }
    }
}

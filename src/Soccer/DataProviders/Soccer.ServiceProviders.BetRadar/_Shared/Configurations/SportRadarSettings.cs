namespace Soccer.DataProviders.SportRadar.Shared.Configurations
{
    using System.Collections.Generic;
    using System.Linq;
    using Score247.Shared.Enumerations;

    public interface ISportRadarSettings
    {
#pragma warning disable S3996 // URI properties should not be strings
        string ServiceUrl { get; set; }
#pragma warning restore S3996 // URI properties should not be strings

        string PushEventEndpoint { get; set; }

        bool EnabledResponseLog { get; set; }

        IEnumerable<SportSettings> Sports { get; set; }

        SportSettings SoccerSettings { get; }
    }

    public class SportRadarSettings : ISportRadarSettings
    {
#pragma warning disable S3996 // URI properties should not be strings
        public string ServiceUrl { get; set; }
#pragma warning restore S3996 // URI properties should not be strings

        public string PushEventEndpoint { get; set; }

        public IEnumerable<SportSettings> Sports { get; set; }

        public SportSettings SoccerSettings => Sports.FirstOrDefault(sport => sport.Id == Sport.Soccer.Value);

        public bool EnabledResponseLog { get; set; }
    }

    public class OddsSetting
    {
        public string Key { get; set; }

        public string AccessLevel { get; set; }

        public string Version { get; set; }

        public bool EnabledOddsJobs { get; set; } = true;

        public string GetOddsScheduleCron { get; set; }

        public string GetOddsChangeScheduleCron { get; set; }
#pragma warning disable S109 // Magic numbers should not be used
        public int GetOddsChangeMinuteInterval { get; set; } = 5;

        public int FetchScheduleDateSpan { get; set; } = 3;
#pragma warning restore S109 // Magic numbers should not be used
    }

    public class SportSettings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AccessLevel { get; set; }

        public string Version { get; set; }

        public IEnumerable<Region> Regions { get; set; }

        public OddsSetting OddsSetting { get; set; }

        public string TrackerWidgetLink { get; set; }
    }

    public class Region
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string PushKey { get; set; }
    }
}
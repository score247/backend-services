namespace Soccer.DataProviders.SportRadar.Shared.Configurations
{
    using System.Collections.Generic;

    public interface ISportRadarSettings
    {
        string ServiceUrl { get; }

        IEnumerable<SportSettings> Sports { get; }
    }

    public class SportRadarSettings : ISportRadarSettings
    {
        public string ServiceUrl { get; }

        public IEnumerable<SportSettings> Sports { get; }
    }

    public class SportSettings
    {
        public int Id { get; }

        public string Name { get; }

        public IEnumerable<Region> Regions { get;  }
    }

    public class Region
    {
        public string Name { get; }

        public string Key { get;  }

        public string PushKey { get; }
    }
}
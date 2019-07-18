namespace Soccer.DataProviders.SportRadar._Shared.Configurations
{
    using System.Collections.Generic;

    public interface ISportRadarSettings
    {
        string ServiceUrl { get; }

        IEnumerable<SportSettings> Sports { get; }
    }

    public class SportRadarSettings : ISportRadarSettings
    {
        public string ServiceUrl { get; set; }

        public IEnumerable<SportSettings> Sports { get; set; }
    }

    public class SportSettings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Region> Regions { get; set; }
    }

    public class Region
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string PushKey { get; set; }
    }
}
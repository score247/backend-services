namespace Soccer.DataProviders.SportRadar.Shared.Configurations
{
    using System.Collections.Generic;

    public interface ISportRadarSettings
    {
        string ServiceUrl { get; set; }

        IEnumerable<SportSettings> Sports { get; set; }
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
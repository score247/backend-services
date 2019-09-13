namespace Soccer.Core.Matches.Models
{
    using MessagePack;
    using Score247.Shared.Base;
#pragma warning disable S109 // Magic numbers should not be used
    [MessagePackObject]
    public class Venue : BaseModel
    {
        [Key(2)]
        public int Capacity { get; set; }

        [Key(3)]
        public string CityName { get; set; }

        [Key(4)]
        public string CountryName { get; set; }

        [Key(5)]
        public string CountryCode { get; set; }
    }
#pragma warning restore S109 // Magic numbers should not be used
}
namespace Soccer.Core.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using MessagePack;
    using Newtonsoft.Json;
    using Score247.Shared.Base;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;

    [MessagePackObject(keyAsPropertyName: true)]
    public class TimelineEvent : BaseEntity
    {
        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public EventType Type { get; set; }

        [Key(3)]
        public DateTimeOffset Time { get; set; }

        [Key(4)]
        public byte MatchTime { get; set; }

        [Key(5)]
        public string StoppageTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(6)]
        public string MatchClock { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(7)]
        public string Team { get; set; }

        [Key(8)]
        public int Period { get; set; }

        [Key(9)]
        public PeriodType PeriodType { get; set; }

        [Key(10)]
        public int HomeScore { get; set; }

        [Key(11)]
        public int AwayScore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(12)]
        public GoalScorer GoalScorer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(13)]
        public Player Assist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(14)]
        public Player Player { get; set; }

        [Key(15)]
        public int InjuryTimeAnnounced { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(16)]
        public Player HomeShootoutPlayer { get; set; }

        [Key(17)]
        public bool IsHomeShootoutScored { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Key(18)]
        public Player AwayShootoutPlayer { get; set; }

        [Key(19)]
        public bool IsAwayShootoutScored { get; set; }

        [Key(20)]
        public int ShootoutHomeScore { get; set; }

        [Key(21)]
        public int ShootoutAwayScore { get; set; }

        [Key(22)]
        public bool IsFirstShoot { get; set; }

        [Key(23)]
        public bool IsHome => Team?.ToLowerInvariant() == "home";

        [Key(24)]
        public string TestString { get; set; }

        [IgnoreMember]
        public string Description { get; set; }

        [IgnoreMember]
        public string Outcome { get; set; }

        [IgnoreMember]
        public string PenaltyStatus { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IgnoreMember]
        public IEnumerable<Commentary> Commentaries { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IgnoreMember]
        public Player PlayerOut { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IgnoreMember]
        public Player PlayerIn { get; set; }
    }
}
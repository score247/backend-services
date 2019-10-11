namespace Soccer.Core.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using MessagePack;
    using Newtonsoft.Json;
    using Score247.Shared.Base;
    using Shared.Enumerations;
    using Soccer.Core.Teams.Models;

    [MessagePackObject(keyAsPropertyName: true)]
    public class TimelineEvent : BaseEntity
    {
        public string Name { get; set; }

        public EventType Type { get; set; }

        public DateTimeOffset Time { get; set; }

        public byte MatchTime { get; set; }

        public string StoppageTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MatchClock { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Team { get; set; }

        public int Period { get; set; }

        public PeriodType PeriodType { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GoalScorer GoalScorer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player Assist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player Player { get; set; }

        public int InjuryTimeAnnounced { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player HomeShootoutPlayer { get; set; }

        public bool IsHomeShootoutScored { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player AwayShootoutPlayer { get; set; }

        public bool IsAwayShootoutScored { get; set; }

        public int ShootoutHomeScore { get; set; }

        public int ShootoutAwayScore { get; set; }

        public bool IsFirstShoot { get; set; }

        public bool IsHome => Team?.ToLowerInvariant() == "home";

        [IgnoreMember]
        public string Description { get; set; }

        [IgnoreMember]
        public string Outcome { get; set; }

        [IgnoreMember]
        public string PenaltyStatus { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Commentary> Commentaries { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IgnoreMember]
        public Player PlayerOut { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IgnoreMember]
        public Player PlayerIn { get; set; }

        public void UpdateScore(int homeScore, int awayScore)
        {
            HomeScore = homeScore;
            AwayScore = awayScore;
        }
    }
}
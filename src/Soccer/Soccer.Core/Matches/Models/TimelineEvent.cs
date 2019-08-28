﻿namespace Soccer.Core.Matches.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Score247.Shared.Base;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;

    public class TimelineEvent : BaseEntity
    {
        public EventType Type { get; set; }

        public DateTimeOffset Time { get; set; }

        public int MatchTime { get; set; }

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
        public IEnumerable<Commentary> Commentaries { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player Assist { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player PlayerOut { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player PlayerIn { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player Player { get; set; }

        public int InjuryTimeAnnounced { get; set; }

        public string Description { get; set; }

        public string Outcome { get; set; }

        public string StoppageTime { get; set; }

        public int ShootoutHomeScore { get; set; }

        public int ShootoutAwayScore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player HomeShootoutPlayer { get; set; }

        public bool IsHomeShootoutScored { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player AwayShootoutPlayer { get; set; }

        public bool IsAwayShootoutScored { get; set; }

        public bool IsHome => Team?.ToLowerInvariant() == "home";

        public bool IsFirstShoot { get; set; }

        public string PenaltyStatus { get; set; }
    }
}
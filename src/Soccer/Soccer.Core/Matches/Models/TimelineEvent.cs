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
#pragma warning disable S107 // Methods should not have too many parameters

        [JsonConstructor]
        public TimelineEvent(
            string id,
            EventType type,
            DateTimeOffset time,
            byte matchTime,
            string stoppageTime,
            string team,
            int period,
            PeriodType periodType,
            int homeScore,
            int awayScore,
            GoalScorer goalScorer,
            Player assist,
            Player player,
            int injuryTimeAnnounced,
            IEnumerable<Commentary> commentaries,
            Player playerOut,
            Player playerIn) : base(id)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            Type = type;
            Time = time;
            MatchTime = matchTime;
            StoppageTime = stoppageTime;
            Team = team;
            Period = period;
            PeriodType = periodType;
            HomeScore = homeScore;
            AwayScore = awayScore;
            GoalScorer = goalScorer;
            Assist = assist;
            Player = player;
            InjuryTimeAnnounced = injuryTimeAnnounced;
            Commentaries = commentaries;
            PlayerOut = playerOut;
            PlayerIn = playerIn;
        }

        public EventType Type { get; }

        public DateTimeOffset Time { get; }

        public byte MatchTime { get; }

        public string StoppageTime { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Team { get; }

        public int Period { get; }

        public PeriodType PeriodType { get; }

        public int HomeScore { get; private set; }

        public int AwayScore { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GoalScorer GoalScorer { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player Assist { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player Player { get; }

        public int InjuryTimeAnnounced { get; }

        public bool IsHome => Team?.ToLowerInvariant() == "home";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Commentary> Commentaries { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player PlayerOut { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player PlayerIn { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player HomeShootoutPlayer { get; set; }

        public bool IsHomeShootoutScored { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Player AwayShootoutPlayer { get; set; }

        public bool IsAwayShootoutScored { get; set; }

        public int ShootoutHomeScore { get; set; }

        public int ShootoutAwayScore { get; set; }

        public bool IsFirstShoot { get; set; }

        [IgnoreMember]
        public string PenaltyStatus { get; set; }

        public void UpdateScore(int homeScore, int awayScore)
        {
            HomeScore = homeScore;
            AwayScore = awayScore;
        }
    }
}
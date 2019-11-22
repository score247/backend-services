using MessagePack;
using Newtonsoft.Json;

namespace Soccer.Core.Teams.Models
{
    [MessagePackObject]
    public class TeamStanding
    {
#pragma warning disable S107 // Methods should not have too many parameters

        [JsonConstructor]
        public TeamStanding(
            string id,
            string name,
            int rank,
            TeamOutcome outcome,
            int played,
            int win,
            int draw,
            int loss,
            int goalsFor,
            int goalsAgainst,
            int goalDiff,
            int points,
            int change)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            Id = id;
            Name = name;
            Rank = rank;
            Outcome = outcome;
            Played = played;
            Win = win;
            Draw = draw;
            Loss = loss;
            GoalsFor = goalsFor;
            GoalsAgainst = goalsAgainst;
            GoalDiff = goalDiff;
            Points = points;
            Change = change;
        }

        [Key(0)]
        public string Id { get; }

        [Key(1)]
        public string Name { get; }

#pragma warning disable S109 // Magic numbers should not be used

        [Key(2)]
        public int Rank { get; }

        [Key(3)]
        public TeamOutcome Outcome { get; }

        [Key(4)]
        public int Played { get; }

        [Key(5)]
        public int Win { get; }

        [Key(6)]
        public int Draw { get; }

        [Key(7)]
        public int Loss { get; }

        [Key(8)]
        public int GoalsFor { get; }

        [Key(9)]
        public int GoalsAgainst { get; }

        [Key(10)]
        public int GoalDiff { get; }

        [Key(11)]
        public int Points { get; }

        [Key(12)]
        public int Change { get; }

#pragma warning restore S109 // Magic numbers should not be used
    }
}
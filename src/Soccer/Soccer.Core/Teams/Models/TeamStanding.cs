namespace Soccer.Core.Teams.Models
{
    public class TeamStanding
    {
#pragma warning disable S107 // Methods should not have too many parameters
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

        public string Id { get; }

        public string Name { get; }

        public int Rank { get; }

        public TeamOutcome Outcome { get; }

        public int Played { get; }

        public int Win { get; }

        public int Draw { get; }

        public int Loss { get; }

        public int GoalsFor { get; }

        public int GoalsAgainst { get; }

        public int GoalDiff { get; }

        public int Points { get; }

        public int Change { get; }
    }
}
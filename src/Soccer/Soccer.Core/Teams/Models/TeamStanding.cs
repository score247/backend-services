namespace Soccer.Core.Teams.Models
{
    public class TeamStanding
    {
        public string Id { get; }
        public string Name { get; }
        public int Rank { get; }
        public string CurrentOutcome { get; }
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
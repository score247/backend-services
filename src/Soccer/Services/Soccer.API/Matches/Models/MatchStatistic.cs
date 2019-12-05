namespace Soccer.API.Matches.Models
{
    using System.Linq;
    using MessagePack;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Teams.Models;

    [MessagePackObject(keyAsPropertyName: true)]
    public class MatchStatistic
    {
        private const int twoTeams = 2;

        public MatchStatistic()
        {
        }

        public MatchStatistic(Match match)
        {
            MatchId = match.Id;

            if (match.Teams != null
                && match.Teams.Count() >= twoTeams)
            {
                var homeTeam = match.Teams.FirstOrDefault(t => t.IsHome);
                var awayTeam = match.Teams.FirstOrDefault(t => !t.IsHome);

                if (homeTeam != null)
                {
                    HomeStatistic = homeTeam.Statistic;
                }

                if (awayTeam != null)
                {
                    AwayStatistic = awayTeam.Statistic;
                }
            }
        }

        public string MatchId { get; }
#pragma warning disable S109 // Magic numbers should not be used

        public TeamStatistic HomeStatistic { get; }

        public TeamStatistic AwayStatistic { get; }
#pragma warning restore S109 // Magic numbers should not be used
    }
}
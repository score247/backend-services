using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timelines.Models;

namespace Soccer.Core.Timelines.QueueMessages
{
    public interface IMatchCommentaryFetchedMessage
    {
        string LeagueId { get; }

        string MatchId { get; }

        TimelineCommentary Commentary { get; }

        Language Language { get; }
    }

    public class MatchCommentaryFetchedMessage : IMatchCommentaryFetchedMessage
    {
        public MatchCommentaryFetchedMessage(string leagueId, string matchId, TimelineCommentary commentary, Language language)
        {
            LeagueId = leagueId;
            MatchId = matchId;
            Commentary = commentary;
            Language = language;
        }

        public string MatchId { get; }

        public string LeagueId { get; }

        public TimelineCommentary Commentary { get; }

        public Language Language { get; }
    }
}
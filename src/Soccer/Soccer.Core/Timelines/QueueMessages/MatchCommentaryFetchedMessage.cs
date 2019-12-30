using System;
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

        DateTimeOffset EventDate { get; }
    }

    public class MatchCommentaryFetchedMessage : IMatchCommentaryFetchedMessage
    {
        public MatchCommentaryFetchedMessage(
            string leagueId,
            string matchId,
            TimelineCommentary commentary,
            Language language,
            DateTimeOffset eventDate = default)
        {
            LeagueId = leagueId;
            MatchId = matchId;
            Commentary = commentary;
            Language = language;
            EventDate = eventDate;
        }

        public string MatchId { get; }

        public string LeagueId { get; }

        public TimelineCommentary Commentary { get; }

        public Language Language { get; }

        public DateTimeOffset EventDate { get; }
    }
}
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Teams.QueueMessages
{
    public interface IHeadToHeadFetchedMessage
    {
        string HomeTeamId { get; }

        string AwayTeamId { get; }

        Match HeadToHeadMatch { get; }

        Language Language { get; }
    }

    public class HeadToHeadFetchedMessage : IHeadToHeadFetchedMessage
    {
        public HeadToHeadFetchedMessage(string homeTeamId, string awayTeamId, Match headToHeadMatch, Language language)
        {
            HomeTeamId = string.IsNullOrWhiteSpace(homeTeamId)
                ? headToHeadMatch?.Teams.FirstOrDefault(t => t.IsHome)?.Id
                : homeTeamId;
            AwayTeamId = string.IsNullOrWhiteSpace(awayTeamId)
                ? headToHeadMatch?.Teams.FirstOrDefault(t => !t.IsHome)?.Id
                : awayTeamId;
            HeadToHeadMatch = headToHeadMatch;
            Language = language;
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public Match HeadToHeadMatch { get; }

        public Language Language { get; }
    }
}
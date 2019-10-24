using System.Collections.Generic;
using Soccer.Core.Teams.Models;

namespace Soccer.Core.Teams.QueueMessages
{
    public interface IHeadToHeadFetchedMessage
    {
        string HomeTeamId { get; }

        string AwayTeamId { get; }

        IReadOnlyList<HeadToHead> HeadToHeads { get; }
    }

    public class HeadToHeadFetchedMessage : IHeadToHeadFetchedMessage
    {
        public HeadToHeadFetchedMessage(string homeTeamId, string awayTeamId, IReadOnlyList<HeadToHead> headToHeads)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            HeadToHeads = headToHeads;
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public IReadOnlyList<HeadToHead> HeadToHeads { get; }
    }
}
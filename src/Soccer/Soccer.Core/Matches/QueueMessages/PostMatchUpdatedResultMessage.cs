namespace Soccer.Core.Matches.QueueMessages
{
    using Soccer.Core.Matches.Models;

    public interface IPostMatchUpdatedResultMessage
    {
        string MatchId { get; }

        string Language { get; }

        MatchResult Result { get; }
    }

    public class PostMatchUpdatedResultMessage : IPostMatchUpdatedResultMessage
    {
        public PostMatchUpdatedResultMessage(string matchId, string language, MatchResult result)
        {
            MatchId = matchId;
            Language = language;
            Result = result;
        }

        public string MatchId { get; }

        public string Language { get; }

        public MatchResult Result { get; }
    }
}

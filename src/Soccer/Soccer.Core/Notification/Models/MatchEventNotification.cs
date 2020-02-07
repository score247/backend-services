using Score247.Shared.Enumerations;

namespace Soccer.Core.Notification.Models
{
    public class MatchEventNotification
    {
        public MatchEventNotification(string matchId, string title, string content, string[] userIds = null, bool isIOS = true) 
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            Title = title;
            Content = content;
            UserIds = userIds;
            IsIOS = isIOS;
        }

        public string MatchId { get; }

        public string Title { get; }

        public string Content { get; }

        public byte SportId { get; }

        public string[] UserIds { get; } 

        public bool IsIOS { get; }
    }
}

namespace Soccer.Core.Matches.QueueMessages
{
    using System;
    using Soccer.Core.Shared.Enumerations;

    public interface IMatchUpdatedConditionsMessage
    {
        string MatchId { get; }

        string Referee { get; }

        int Attendance { get; }

        Language Language { get; }

        DateTimeOffset EventDate { get; }
    }

    public class MatchUpdatedConditionsMessage : IMatchUpdatedConditionsMessage
    {
        public MatchUpdatedConditionsMessage(string matchId, string referee, int attendance, Language language, DateTimeOffset eventDate = default)
        {
            MatchId = matchId;
            Referee = referee;
            Attendance = attendance;
            Language = language;
            EventDate = eventDate;
        }

        public string MatchId { get; }

        public string Referee { get; }

        public int Attendance { get; }

        public Language Language { get; }

        public DateTimeOffset EventDate { get; }
    }
}

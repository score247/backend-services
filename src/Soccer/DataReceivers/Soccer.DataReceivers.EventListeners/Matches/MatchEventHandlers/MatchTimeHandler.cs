namespace Soccer.DataReceivers.EventListeners.Matches.MatchEventHandlers
{
    using System.Collections.Generic;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;

    public interface IMatchTimeHandler
    {
        void Handle(MatchEvent matchEvent);
    }

    public class MatchTimeHandler : IMatchTimeHandler
    {
        private static readonly IDictionary<int, int> PeriodStartTimeMapper =
            new Dictionary<int, int>
            {
                    { MatchStatus.FirstHaft.Value, 1 },
                    { MatchStatus.SecondHaft.Value, 46 },
                    { MatchStatus.FirstHaftExtra.Value, 91 },
                    { MatchStatus.SecondHaftExtra.Value, 106 }
            };

        public void Handle(MatchEvent matchEvent)
        {
            if (matchEvent.MatchResult.EventStatus.IsLive())
            {
                var timeline = matchEvent.Timeline;
                var matchTime = timeline.MatchTime;
                var matchStatus = matchEvent.MatchResult.MatchStatus.Value;

                if (timeline.Type.IsPeriodStart && PeriodStartTimeMapper.ContainsKey(matchStatus))
                {
                    matchTime = PeriodStartTimeMapper[matchStatus];
                }

                if (!string.IsNullOrEmpty(timeline.StoppageTime))
                {
                    matchTime += int.Parse(timeline.StoppageTime);
                }

                if (matchTime > 0)
                {
                    // TODO : Change to store match time in latest timeline property
                    matchEvent.Timeline.MatchTime = matchTime;
                }
            }
        }
    }
}
namespace Soccer.Core.Matches.Events
{
    using Soccer.Core.Matches.Models;

    public interface IMatchEventsReceivedEvent
    {
        MatchEvent MatchEvent { get; }
    }
}
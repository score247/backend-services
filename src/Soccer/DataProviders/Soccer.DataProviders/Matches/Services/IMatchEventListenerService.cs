namespace Soccer.DataProviders.Matches.Services
{
    using Soccer.Core.Matches.Models;
    using System;

    public interface IMatchEventListenerService
    {
        void ListenEvents(Action<MatchEvent> handler);
    }
}
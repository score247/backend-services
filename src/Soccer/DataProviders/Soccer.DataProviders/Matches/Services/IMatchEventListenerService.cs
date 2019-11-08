namespace Soccer.DataProviders.Matches.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Soccer.Core.Matches.Models;

    public interface IMatchEventListenerService
    {
        string Name { get; }

        Task ListenEvents(Action<MatchEvent> handler, CancellationToken cancellationToken);
    }
}
namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Score247.Shared;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Teams.Models;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.Database.Matches.Criteria;

    public class RedCardEventConsumer : IConsumer<IRedCardEventMessage>
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public RedCardEventConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IRedCardEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            var processedRedCards = await GetProcessedRedCards(matchEvent.MatchId, matchEvent.Timeline.Team);

            var teamStats = new TeamStatistic(
                processedRedCards.Count(x => x.Type.IsRedCard()),
                processedRedCards.Count(x => x.Type.IsYellowRedCard()));

            await messageBus.Publish<ITeamStatisticUpdatedMessage>(new TeamStatisticUpdatedMessage(matchEvent.MatchId, matchEvent.Timeline.IsHome, teamStats, true));
            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }

        private async Task<IList<TimelineEvent>> GetProcessedRedCards(string matchId, string teamId)
        {
            var timelineEvents = (await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(matchId))).ToList();

            return timelineEvents == null
                ? new List<TimelineEvent>()
                : timelineEvents
                    .Where(t => t.Team == teamId && (t.Type.IsRedCard() || t.Type.IsYellowRedCard()))
                    .ToList();
        }
    }
}
namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.Database.Matches.Criteria;

    public class CardEventConsumer : IConsumer<ICardEventMessage>
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public CardEventConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ICardEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            var processedRedCards = await GetProcessedCards(matchEvent.MatchId, matchEvent.Timeline.Team);

            var teamStats = new TeamStatistic(
                GetNumberOfCardEvents(matchEvent.Timeline, processedRedCards, EventType.RedCard),
                GetNumberOfCardEvents(matchEvent.Timeline, processedRedCards, EventType.YellowRedCard),
                GetNumberOfCardEvents(matchEvent.Timeline, processedRedCards, EventType.YellowCard));

            await messageBus.Publish<ITeamStatisticUpdatedMessage>(new TeamStatisticUpdatedMessage(matchEvent.MatchId, matchEvent.Timeline.IsHome, teamStats, true));
            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }

        private async Task<IList<TimelineEvent>> GetProcessedCards(string matchId, string teamId)
        {
            var timelineEvents = (await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(matchId))).ToList();

            return timelineEvents == null
                ? new List<TimelineEvent>()
                : timelineEvents
                    .Where(t => t.Team == teamId && (t.Type.IsRedCard() || t.Type.IsYellowRedCard() || t.Type.IsYellowCard()))
                    .ToList();
        }

        internal static int GetNumberOfCardEvents(TimelineEvent timelineEvent, IList<TimelineEvent> processedCards, EventType eventType)
        {
            var numberOfCards = processedCards.Count(x => x.Type == eventType);

            if (timelineEvent.Type == eventType && !processedCards.Any(timeline => timeline.Id == timelineEvent.Id))
            {
                return numberOfCards + 1;
            }

            return numberOfCards;
        }
    }
}
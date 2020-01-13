using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.Database.Matches.Criteria;
using Soccer.Database.Timelines.Commands;
using Soccer.EventProcessors.Shared.Configurations;

namespace Soccer.EventProcessors.Timeline
{
    public class RemoveTimelineEventsConsumer : IConsumer<IMatchTimelinesConfirmedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public RemoveTimelineEventsConsumer(
            IDynamicRepository dynamicRepository,
            IBus messageBus,
            IAppSettings appSettings)
        {
            this.dynamicRepository = dynamicRepository;
            this.messageBus = messageBus;
            this.appSettings = appSettings;
        }

        public async Task Consume(ConsumeContext<IMatchTimelinesConfirmedMessage> context)
        {
            if (context?.Message == null
                || string.IsNullOrWhiteSpace(context.Message.MatchId)
                || context.Message.Timelines?.Any() == false)
            {
                return;
            }

            var currentTimelineEvents = (await GetCurrentTimelineEvents(context.Message))?.ToList();

            if (currentTimelineEvents?.Any() == false)
            {
                return;
            }

            var removedTimelineEvents = (currentTimelineEvents.Except(context.Message.Timelines)).ToList();

            if (removedTimelineEvents.Any())
            {
                await dynamicRepository.ExecuteAsync(new RemoveTimelineCommand(
                    context.Message.MatchId,
                    removedTimelineEvents.ToList()));

                await messageBus.Publish<IMatchTimelinesRemovedMessage>(new MatchTimelinesRemovedMessage(
                    context.Message.MatchId,
                    removedTimelineEvents.Select(timeline => timeline.Id).ToArray()));
            }
        }

        private async Task<IEnumerable<TimelineEvent>> GetCurrentTimelineEvents(IMatchTimelinesConfirmedMessage message)
        {
            var currentTimelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(
                    message.MatchId,
                    message.EventDate));

            return message.EventStatus.IsClosed()
                ? currentTimelineEvents
                : currentTimelineEvents?.Where(timeline => timeline.Time <= DateTimeOffset.Now.AddMinutes(-appSettings.CorrectTimelineSpanInMinutes));
        }
    }
}
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
    public class RemoveTimelinesConsumer : IConsumer<IMatchTimelinesConfirmedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly IAppSettings appSettings;

        public RemoveTimelinesConsumer(
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

            var currentTimelines = await GetCurrentTimelines(context.Message);

            if (currentTimelines?.Any() == false)
            {
                return;
            }

            var removedTimelines = currentTimelines.Except(context.Message.Timelines);

            if (removedTimelines.Any())
            {
                await dynamicRepository.ExecuteAsync(new RemoveTimelineCommand(
                    context.Message.MatchId,
                    removedTimelines.ToList()));

                await messageBus.Publish<IMatchTimelinesRemovedMessage>(new MatchTimelinesRemovedMessage(
                    context.Message.MatchId,
                    removedTimelines.Select(timeline => timeline.Id).ToArray()));
            }
        }

        private async Task<IEnumerable<TimelineEvent>> GetCurrentTimelines(IMatchTimelinesConfirmedMessage message)
        {
            var currentTimelines = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(
                    message.MatchId,
                    message.EventDate));

            return message.EventStatus.IsClosed()
                ? currentTimelines
                : currentTimelines?.Where(timeline => timeline.Time <= DateTimeOffset.Now.AddMinutes(-appSettings.CorrectTimelineSpanInMinutes));
        }
    }
}

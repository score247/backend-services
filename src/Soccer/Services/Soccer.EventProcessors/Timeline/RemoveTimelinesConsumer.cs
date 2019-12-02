using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.Database.Matches.Criteria;
using Soccer.Database.Timelines.Commands;

namespace Soccer.EventProcessors.Timeline
{
    public class RemoveTimelinesConsumer : IConsumer<IMatchTimelinesConfirmedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public RemoveTimelinesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchTimelinesConfirmedMessage> context)
        {
            if (context?.Message == null
                || string.IsNullOrWhiteSpace(context.Message.MatchId)
                || context.Message.Timelines?.Any() == false)
            {
                return;
            }

            var latestTimeline = context.Message.Timelines
                .OrderBy(timeline => timeline.Time)
                .LastOrDefault();

            var currentTimelines = await GetCurrentTimelines(context.Message, latestTimeline);

            if (currentTimelines?.Any() == false)
            {
                return;
            }

            var removedTimelines = currentTimelines.Except(context.Message.Timelines);

            if (removedTimelines.Any())
            {
                await dynamicRepository.ExecuteAsync(new RemoveTimelineCommand(context.Message.MatchId, removedTimelines.ToList()));
            }
        }

        private async Task<IEnumerable<TimelineEvent>> GetCurrentTimelines(IMatchTimelinesConfirmedMessage message, TimelineEvent latestTimeline)
        => (await dynamicRepository
                .FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(
                    message.MatchId,
                    message.EventDate)))
                ?.Where(timeline => timeline.Time <= latestTimeline.Time);
    }
}

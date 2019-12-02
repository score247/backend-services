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
            if (string.IsNullOrWhiteSpace(context?.Message?.MatchId))
            {
                return;
            }

            var latestTimeline = context.Message.Timelines
                .OrderBy(timeline => timeline.Time)
                .LastOrDefault();

            if (latestTimeline == null)
            {
                return;
            }

            //TODO if closed match???
            var currentTimelines = (await dynamicRepository
                .FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(
                    context.Message.MatchId,
                    context.Message.EventDate)))
                ?.Where(timeline => timeline.Time <= latestTimeline.Time)
                .ToList();

            if (currentTimelines?.Count == 0)
            {
                return;
            }

            var removedTimelines = currentTimelines 
                .Except(context.Message.Timelines)
                .ToList();

            if (removedTimelines.Any())
            {
                await dynamicRepository.ExecuteAsync(new RemoveTimelineCommand(context.Message.MatchId, removedTimelines));
            }
        }
    }
}

using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;

namespace Soccer.EventProcessors.Timeline
{
    public class FetchTimelineConsumer : IConsumer<ITimelineFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchTimelineConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ITimelineFetchedMessage> context)
        {
            var timeline = context.Message;

            if (timeline != null)
            {
                await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(timeline.MatchId, timeline.Timeline, timeline.Language));
            }
        }
    }
}

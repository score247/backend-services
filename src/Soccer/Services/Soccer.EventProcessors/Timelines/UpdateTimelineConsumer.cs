using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Timeline.QueueMessages;
using Soccer.Database.Matches.Commands;

namespace Soccer.EventProcessors.Timelines
{   
    public class UpdateTimelineConsumer : IConsumer<ITimelineUpdatedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateTimelineConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ITimelineUpdatedMessage> context)
        {
            if (context.Message == null)
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(context.Message.MatchId, context.Message.Timeline, context.Message.Language));
        }
    }
}

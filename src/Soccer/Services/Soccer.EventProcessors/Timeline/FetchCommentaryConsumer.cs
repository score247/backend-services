using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Timeline.QueueMessages;
using Soccer.Database.Timelines.Commands;

namespace Soccer.EventProcessors.Timeline
{
    public class FetchCommentaryConsumer : IConsumer<IMatchCommentaryFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchCommentaryConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchCommentaryFetchedMessage> context)
        {
            if (context.Message == null)
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(new InsertCommentaryCommand(
                context.Message.MatchId,
                context.Message.Commentary.TimelineId,
                context.Message.Commentary.Commentaries,
                context.Message.Language));
        }     
    }
}

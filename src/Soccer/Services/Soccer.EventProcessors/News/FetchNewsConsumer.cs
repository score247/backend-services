using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.News.QueueMessages;
using Soccer.Database.News.Commands;

namespace Soccer.EventProcessors.News
{
    public class FetchNewsConsumer : IConsumer<INewsFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchNewsConsumer(
            IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<INewsFetchedMessage> context)
        {
            if (context?.Message?.NewsList == null || context.Message.NewsList.Any())
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(new InsertNewsCommand(context.Message.NewsList, context.Message.Language));
        }
    }
}

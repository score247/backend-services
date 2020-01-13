using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.News.QueueMessages;
using Soccer.Database.News.Commands;

namespace Soccer.EventProcessors.News
{
    public class FetchNewsImageConsumer : IConsumer<INewsImageFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchNewsImageConsumer(
            IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<INewsImageFetchedMessage> context)
        {
            if (string.IsNullOrWhiteSpace(context?.Message?.ImageName) || string.IsNullOrWhiteSpace(context?.Message?.ImageContent))
            {
                return;
            }

            await dynamicRepository.ExecuteAsync(new InsertNewsImageCommand(context.Message.ImageName, context.Message.ImageContent));
        }
    }
}

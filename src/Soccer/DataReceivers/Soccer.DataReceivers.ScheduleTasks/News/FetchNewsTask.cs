using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.News.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.EyeFootball._Shared.Configurations;
using Soccer.DataProviders.News.Services;

namespace Soccer.DataReceivers.ScheduleTasks.News
{
    public interface IFetchNewsTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchNewsFeed();
    }

    public class FetchNewsTask : IFetchNewsTask
    {
        private const string NEWS_FEED_XML_PATH = "football_news.xml";

        private readonly IEyeFootballSettings settings;
        private readonly IBus messageBus;
        private readonly INewsService newsService;

        public FetchNewsTask(
            IEyeFootballSettings settings, 
            IBus messageBus,
            INewsService newsService)
        {
            this.settings = settings;
            this.messageBus = messageBus;
            this.newsService = newsService;
        }

        public async Task FetchNewsFeed()
        {
            var newsFeeds = await newsService.GetNewsFeed($"{settings.ServiceUrl}/{NEWS_FEED_XML_PATH}");

            await messageBus.Publish<INewsFetchedMessage>(new NewsFetchedMessage(newsFeeds, Language.en_US));
        }
    }
}

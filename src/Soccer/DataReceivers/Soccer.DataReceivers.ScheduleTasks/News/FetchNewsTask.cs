using System.Linq;
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

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchNewsImage(string imageName, string imageLink);
    }

    public class FetchNewsTask : IFetchNewsTask
    {
        private const string NEWS_FEED_XML_PATH = "football_news.xml";

        private readonly IEyeFootballSettings settings;
        private readonly IBus messageBus;
        private readonly INewsService newsService;
        private readonly IBackgroundJobClient backgroundJobClient;

        public FetchNewsTask(
            IEyeFootballSettings settings, 
            IBus messageBus,
            INewsService newsService,
            IBackgroundJobClient backgroundJobClient)
        {
            this.settings = settings;
            this.messageBus = messageBus;
            this.newsService = newsService;
            this.backgroundJobClient = backgroundJobClient;
        }

        public async Task FetchNewsFeed()
        {
            var newsFeeds = await newsService.GetNewsFeed($"{settings.ServiceUrl}/{NEWS_FEED_XML_PATH}");            

            await messageBus.Publish<INewsFetchedMessage>(new NewsFetchedMessage(newsFeeds, Language.en_US));

            foreach (var news in newsFeeds)
            {
                if (!string.IsNullOrWhiteSpace(news.ImageSource)) 
                {
                    var imageName = GenerateImageName(news.Link, news.ImageSource);

                    backgroundJobClient.Enqueue<IFetchNewsTask>(job => job.FetchNewsImage(imageName, news.ImageSource));
                }
            }
        }

        private static string GenerateImageName(string guid, string imageLink)
        => $"{guid.Split('/').LastOrDefault()}-{imageLink.Split('/').LastOrDefault()}";

        public async Task FetchNewsImage(string imageName, string imageLink)
        {
            var newsImage = await newsService.GetNewsImage(imageName, imageLink);

            await messageBus.Publish<INewsImageFetchedMessage>(new NewsImageFetchedMessage(newsImage.Name, newsImage.Content));
        }
    }
}

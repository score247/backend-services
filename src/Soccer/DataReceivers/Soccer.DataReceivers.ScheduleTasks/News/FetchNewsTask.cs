using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using HtmlAgilityPack;
using MassTransit;
using Soccer.DataProviders.News.Services;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

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
        private readonly IAppSettings appSettings;
        private readonly IBus messageBus;
        private readonly INewsService newsService;

        public FetchNewsTask(
            IAppSettings appSettings, 
            IBus messageBus,
            INewsService newsService)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.newsService = newsService;
        }

        public async Task FetchNewsFeed()
        {
            var newsFeed = await newsService.GetNewsFeed();

            foreach (var news in newsFeed)
            {
                var httpClient = new HttpClient();
                var htmlContent = await httpClient.GetStringAsync(news.Source);

                var html = new HtmlDocument();
                html.LoadHtml(htmlContent);

                var spans = html.DocumentNode.Descendants("span")
                    .Where(node => node.GetAttributeValue("itemprop", "").Equals("articleBody"));
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Flurl.Http;
using Soccer.Core.News.Models;
using Soccer.DataProviders.EyeFootball.News.DataMappers;
using Soccer.DataProviders.EyeFootball.News.HtmlParsers;
using Soccer.DataProviders.News.Services;

namespace Soccer.DataProviders.EyeFootball.News.Services
{
    public class NewsService : INewsService
    {
        public async Task<List<NewsItem>> GetNewsFeed(string feedUri)
        {
            var newsFeed = await GetRssFeeds(feedUri);

            var newsList = new List<NewsItem>();

            foreach (var news in newsFeed)
            {
                var htmlContent = await news.Source.GetStringAsync();

                var content = NewsHtmlParser.ParseNewsContent(htmlContent);
                var imageLink = NewsHtmlParser.ParseNewsImageSource(news);
                var author = NewsHtmlParser.ParseAuthor(htmlContent);

                var newsItem = new NewsItem(news.Title, content, imageLink, news.Guid, author, news.PublishedDate);

                newsList.Add(newsItem);
            }

            return newsList;
        }

        private static Task<List<NewsFeed>> GetRssFeeds(string feedUri)
        {
            return Task.Run(() =>
            {
                using (var xmlr = XmlReader.Create(feedUri))
                {
                    var feedItems = SyndicationFeed
                                        .Load(xmlr)
                                        .GetRss20Formatter()
                                        .Feed
                                        .Items;

                    return feedItems.Select(item => NewsMapper.MapNewsFeed(item)).ToList();
                }
            });
        }
    }
}

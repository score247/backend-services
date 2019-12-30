using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Refit;
using Soccer.Core.News.Models;
using Soccer.DataProviders.EyeFootball.News.DataMappers;
using Soccer.DataProviders.EyeFootball.News.Dtos;
using Soccer.DataProviders.News.Services;

namespace Soccer.DataProviders.EyeFootball.News.Services
{
    public interface INewsApi
    {
        [Get("/football_news.xml")]
        Task<NewsFeedDto> GetNewsFeed();
    }

    public class NewsService : INewsService
    {
        private readonly INewsApi newsApi;

        public NewsService(INewsApi newsApi)
        {
            this.newsApi = newsApi;
        }

        public async Task<List<NewsFeed>> GetNewsFeed()
        {
            //var newsList = await newsApi.GetNewsFeed();
            //var title = newsList.Channel?.Title;

            //return newsList
            //    .Items
            //    .Select(newsItem => NewsMapper.MapNewsFeed(newsItem))
            //    .ToList();

            using (var xmlr = XmlReader.Create("https://www.eyefootball.com/football_news.xml"))
            {
                // get the items within a feed
                var feedItems = SyndicationFeed
                                    .Load(xmlr)
                                    .GetRss20Formatter()
                                    .Feed
                                    .Items;

                // print out details about each item in the feed
                foreach (var item in feedItems)
                {

                }
            }

            return new List<NewsFeed>();
        }
    }
}

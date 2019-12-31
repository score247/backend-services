using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Soccer.Core.News.Models;
using Soccer.DataProviders.EyeFootball.News.DataMappers;
using Soccer.DataProviders.News.Services;

namespace Soccer.DataProviders.EyeFootball.News.Services
{

    public class NewsService : INewsService
    {
        public Task<List<NewsFeed>> GetNewsFeed()
        {
            return Task.Run(() => {
                using (var xmlr = XmlReader.Create("https://www.eyefootball.com/football_news.xml"))
                {
                    // get the items within a feed
                    var feedItems = SyndicationFeed
                                        .Load(xmlr)
                                        .GetRss20Formatter()
                                        .Feed
                                        .Items;

                    // print out details about each item in the feed
                    return feedItems.Select(item => NewsMapper.MapNewsFeed(item)).ToList();
                }
            });
        }
    }
}

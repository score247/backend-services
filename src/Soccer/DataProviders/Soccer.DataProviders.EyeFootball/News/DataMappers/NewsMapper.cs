using System;
using System.Linq;
using System.ServiceModel.Syndication;
using Soccer.Core.News.Models;

namespace Soccer.DataProviders.EyeFootball.News.DataMappers
{
    public static class NewsMapper
    {
        public static NewsFeed MapNewsFeed(SyndicationItem syncdicationItem)
        => new NewsFeed(
                syncdicationItem.Title.Text,
                null,
                null,
                syncdicationItem.Links.FirstOrDefault()?.Uri.ToString(),
                DateTimeOffset.UtcNow);
    }
}

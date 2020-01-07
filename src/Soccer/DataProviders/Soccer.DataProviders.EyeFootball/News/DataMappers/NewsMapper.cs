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
                syncdicationItem.Id,
                syncdicationItem.Title.Text,             
                syncdicationItem.Links.FirstOrDefault()?.Uri.ToString(),
                syncdicationItem.Summary.Text,
                syncdicationItem.PublishDate);
    }
}

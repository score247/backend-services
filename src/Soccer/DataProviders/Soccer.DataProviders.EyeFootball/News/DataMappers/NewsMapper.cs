using System;
using Soccer.Core.News.Models;
using Soccer.DataProviders.EyeFootball.News.Dtos;

namespace Soccer.DataProviders.EyeFootball.News.DataMappers
{
    public static class NewsMapper
    {
        public static NewsFeed MapNewsFeed(NewsFeedItemDto newsFeedDto)
        => new NewsFeed(
                newsFeedDto.Title, 
                null, 
                null, 
                newsFeedDto.Link, 
                DateTimeOffset.UtcNow);

    }
}

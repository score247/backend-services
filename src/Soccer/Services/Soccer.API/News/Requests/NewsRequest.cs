using System.Collections.Generic;
using MediatR;
using Score247.Shared.Enumerations;
using Soccer.Core.News.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.API.News.Requests
{
    public class NewsRequest : IRequest<IEnumerable<NewsItem>>
    {
        public NewsRequest(string language)
        {
            Language = Enumeration.FromDisplayName<Language>(language);
        }

        public Language Language { get; }
    }
}

using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Soccer.Core.News.Models;

namespace Soccer.DataProviders.EyeFootball.News.HtmlParsers
{
    public static class NewsHtmlParser
    {
        public static string ParseNewsImageSource(NewsFeed news)
        {
            var newsDesc = new HtmlDocument();
            newsDesc.LoadHtml(news.Summary);

            return newsDesc.DocumentNode.Descendants("img")
                .FirstOrDefault()
                ?.GetAttributeValue("src", "");
        }

        public static string ParseNewsContent(string htmlContent)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            var content = html.DocumentNode.Descendants("span")
                .FirstOrDefault(node => node.GetAttributeValue("itemprop", "").Equals("articleBody"))
                ?.InnerText;

            return string.IsNullOrWhiteSpace(content) 
                ? string.Empty 
                : RemoveComments(content);
        }


        public static string RemoveComments(string content) 
        => Regex.Replace(content, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
    }

}

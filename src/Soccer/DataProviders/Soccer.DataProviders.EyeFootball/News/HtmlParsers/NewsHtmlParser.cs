using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Soccer.DataProviders.EyeFootball.News.HtmlParsers
{
    public static class NewsHtmlParser
    {
        private const char NewLine = '\n';

        public static string ParseNewsImageSource(string htmlContent)
        {
            var newsDesc = new HtmlDocument();
            newsDesc.LoadHtml(htmlContent);

            return newsDesc.DocumentNode.Descendants("p")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("article_image"))
                .Descendants("img")
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
                : RemoveComments(content).TrimStart(NewLine).TrimEnd(NewLine);
        }

        public static string ParseAuthor(string htmlContent)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            var content = html.DocumentNode.Descendants("a")
                .FirstOrDefault(node => node.GetAttributeValue("rel", "").Equals("author"))
                ?.InnerText;

            return string.IsNullOrWhiteSpace(content)
                ? string.Empty
                : RemoveComments(content);
        }

        public static string RemoveComments(string content) 
        => Regex.Replace(content, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
    }

}

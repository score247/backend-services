using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Soccer.DataProviders.EyeFootball.News.Dtos
{
    //TODO immutable

    [XmlRoot(ElementName = "rss", Namespace = "http://www.w3.org/2005/Atom")]
    public class NewsFeedDto
    {
        [XmlElement("channel")]
        public NewsFeedChannel Channel { get; set; }
    }

    public partial class NewsFeedChannel 
    {
        [XmlElement("title")]
        public string Title { get; set; }

        //[XmlArrayItem("item")]
        //public List<NewsFeedItemDto> Items { get; set; }
    }

    public class NewsFeedItemDto 
    {
        [XmlElement("title")]
        public string Title { get; set; }

        //public DateTimeOffset PubDate { get; set; }

        //public string Guid { get; set; }

        //public string Description { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }
    }
}

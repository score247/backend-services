using System.Collections.Generic;

namespace Soccer.NotificationServices.AppCenter.Dtos
{
    public class ContentDto
    {
        public string name { get; set; }

        public string title { get; set; }

        public string body { get; set; }

        public IDictionary<string, string> custom_data { get; set; }
    }
}

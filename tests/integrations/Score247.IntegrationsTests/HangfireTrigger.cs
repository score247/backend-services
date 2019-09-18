namespace Score247.IntegrationsTests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HangfireTrigger
    {
        private readonly string HangfireUrl;

        public HangfireTrigger(string hangfireUrl = "http://ha.nexdev.net:7873/hangfire/recurring/trigger")
        {
            HangfireUrl = hangfireUrl;
        }

        public async Task TriggerFetchMatch()
        {
            var jobs = new[]
            {
                new KeyValuePair<string,string>("jobs[]", "FetchPreMatch"),
                new KeyValuePair<string,string>("jobs[]", "FetchPostMatch"),
                new KeyValuePair<string,string>("jobs[]", "FetchLiveMatch")
            };

            await TiggerJobs(jobs);
        }

        public async Task TriggerFetchOdds()
        {
            var jobs = new[] { new KeyValuePair<string, string>("jobs[]", "FetchOddsChangeLogs") };

            await TiggerJobs(jobs);
        }

        private async Task TiggerJobs(KeyValuePair<string, string>[] jobs)
        {
            using (HttpClient triggerClient = new HttpClient
            {
                BaseAddress = new Uri(HangfireUrl)
            })
            {
                var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);

                var fetchContent = new FormUrlEncodedContent(jobs);

                request.Content = fetchContent;

                using (HttpResponseMessage httpResponseMessage = await triggerClient.SendAsync(request))
                {
                    _ = httpResponseMessage;
                }
            }
        }
    }
}
namespace Soccers.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;
    using RestSharp;

    [InProcessAttribute, MinColumn, MaxColumn]
    public class ApiBenchmark
    {
        private static Dictionary<string, string> Urls = new Dictionary<string, string>
        {
            { "New Api", "http://10.19.10.52:51147/api" },
            { "Old Api", "http://10.19.10.52:51145/api" }
        };

        public IEnumerable<object[]> GetMatchesBenchmarkParams()
        {
            yield return new object[] { "New Api", new DateTime(2019, 08, 28, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "Old Api", new DateTime(2019, 08, 28, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "New Api", new DateTime(2019, 08, 29, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "Old Api", new DateTime(2019, 08, 29, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "New Api", new DateTime(2019, 08, 30, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "Old Api", new DateTime(2019, 08, 30, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "New Api", new DateTime(2019, 08, 31, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "Old Api", new DateTime(2019, 08, 31, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "New Api", new DateTime(2019, 09, 01, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "Old Api", new DateTime(2019, 09, 01, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "New Api", new DateTime(2019, 09, 02, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { "Old Api", new DateTime(2019, 09, 02, 0, 0, 0, DateTimeKind.Utc) };
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetMatchesBenchmarkParams))]
        public void GetMatchByDate(string apiName, DateTime date)
        {
            var client = new RestClient(Urls[apiName]);
            var request = new RestRequest("/soccer/en-US/matches");

            request.AddParameter("fd", date.Date);
            request.AddParameter("td", date.AddDays(1).Date.AddMinutes(-1));
            request.AddParameter("tz", new TimeSpan(7, 0, 0));

            var response = client.Get(request);

            Console.WriteLine($"{apiName} - {date} - {response.RawBytes.Length}");
        }
    }
}
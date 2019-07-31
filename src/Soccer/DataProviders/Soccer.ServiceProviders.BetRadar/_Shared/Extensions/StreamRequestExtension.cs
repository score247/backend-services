using System;
using System.Net;

namespace Soccer.DataProviders.SportRadar.Shared.Extensions
{
    public static class StreamRequestExtension
    {
        public static HttpWebRequest CreateListenRequest(this Uri endpoint)
        {
            var req = HttpWebRequest.Create(endpoint) as HttpWebRequest;

            req.AllowAutoRedirect = false;
            WebResponse reply;

            try
            {
                req.GetResponse();
            }
            catch (WebException e)
            {
                reply = e.Response;
                var redirectUrl = reply.Headers["Location"];
                req = (HttpWebRequest)HttpWebRequest.Create(new Uri(redirectUrl));
                req.KeepAlive = true;
            }

            return req;
        }
    }
}
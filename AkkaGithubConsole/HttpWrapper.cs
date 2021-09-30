using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AkkaGithubConsole
{
    public class HttpWrapper : IHttpWrapper
    {
        public HttpClient HttpClient { get; }

        public HttpWrapper()
        {
            HttpClient = new HttpClient { BaseAddress = new Uri("https://api.github.com/") };
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("product", "1"));
        }
    }
}
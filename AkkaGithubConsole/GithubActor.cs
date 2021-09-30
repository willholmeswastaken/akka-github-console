using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaGithubConsole.Models.External;
using Newtonsoft.Json;

namespace AkkaGithubConsole
{
    public class GithubActor : ReceiveActor
    {
        private readonly HttpClient _httpClient;

        public GithubActor()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://api.github.com/users/")};
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("product", "1"));

            ReceiveAsync<GithubProfileRequest>(async req => await HandleProfileRequest(req));
        }

        private async Task HandleProfileRequest(GithubProfileRequest req)
        {
            var res = await _httpClient.GetAsync(req.Username);
            if(!res.IsSuccessStatusCode) Sender.Tell(new object());

            var profile = JsonConvert.DeserializeObject<GithubProfile>(await res.Content.ReadAsStringAsync());

            Sender.Tell(profile);
        }
    }
}

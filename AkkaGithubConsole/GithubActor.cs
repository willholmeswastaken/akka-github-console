using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using AkkaGithubConsole.Models;
using AkkaGithubConsole.Models.External;
using Newtonsoft.Json;

namespace AkkaGithubConsole
{
    public class GithubActor : ReceivePersistentActor
    {
        private readonly IHttpWrapper _httpWrapper;
        private readonly IList<GithubProfile> _searchedProfiles = new List<GithubProfile>();
        public override string PersistenceId => "Hardcoded";

        public GithubActor(IHttpWrapper httpWrapper)
        {
            _httpWrapper = httpWrapper;

            Recover<GithubProfile>(profile => _searchedProfiles.Add(profile));

            CommandAsync<GithubProfileRequest>(async req => await HandleProfileRequest(req));

            Command<GetCache>(get => Sender.Tell(_searchedProfiles));
        }

        private async Task HandleProfileRequest(GithubProfileRequest req)
        {
            var cachedProfile = _searchedProfiles.FirstOrDefault(x => x.Login == req.Username);
            GithubProfile profile;
            
            if (cachedProfile == null)
            {
                var res = await _httpWrapper.HttpClient.GetAsync($"users/{req.Username}");
                if (!res.IsSuccessStatusCode) Sender.Tell(new object());
                profile = JsonConvert.DeserializeObject<GithubProfile>(await res.Content.ReadAsStringAsync());
                Persist(profile, p => _searchedProfiles.Add(p));
            }
            else
            {
                profile = cachedProfile;
            }

            Sender.Tell(profile);
        }
    }
}

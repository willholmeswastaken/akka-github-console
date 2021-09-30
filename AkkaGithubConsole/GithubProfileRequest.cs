namespace AkkaGithubConsole
{
    public class GithubProfileRequest : GithubRequest
    {
        public GithubProfileRequest(string username)
        {
            Username = username;
        }
        public string Username { get; }
    }
}

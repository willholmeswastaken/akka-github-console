using System.Net.Http;

namespace AkkaGithubConsole
{
    public interface IHttpWrapper
    {
        HttpClient HttpClient { get; }
    }
}
using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Extensions.DependencyInjection;
using AkkaGithubConsole.Models.External;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaGithubConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<GithubActor>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var actorSystem = ActorSystem.Create("gh-actor-system");
            actorSystem.UseServiceProvider(serviceProvider);

            var actor = actorSystem.ActorOf(actorSystem.DI().Props<GithubActor>(), "GithubActor");
            Console.WriteLine("**********************");
            Console.WriteLine("Github Console Browser");
            Console.WriteLine("**********************");
            while (true)
            {
                Console.WriteLine("Please enter the username of an account to find on github");
                var username = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Please try again!");
                    continue;
                }

                var result = await actor.Ask(new GithubProfileRequest(username)) as GithubProfile;

                Console.WriteLine($"{result.Name} has {result.PublicRepos} public repos.");
                Console.WriteLine(result.Bio);
                Console.WriteLine($"{result.Name} is based in {result.Location}");

                bool anotherSearch;
                while (true)
                {
                    Console.WriteLine("Would you like to find another account? yes/no");

                    var anotherOne = Console.ReadLine()?.ToLower();
                    if (string.IsNullOrWhiteSpace(anotherOne) || anotherOne.Equals("yes"))
                    {
                        anotherSearch = true;
                        break;
                    }
                    if (anotherOne.Equals("no"))
                    {
                        anotherSearch = false;
                        break;
                    }
                    continue;
                }
                if(!anotherSearch)
                    break;
            }

            
        }
    }
}

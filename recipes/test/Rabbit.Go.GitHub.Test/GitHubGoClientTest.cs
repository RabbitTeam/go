/*using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rabbit.Go.GitHub.Test
{
    public class GitHubGoClientTest
    {
        private readonly IGitHubGoClient _gitHubGoClient;

        public GitHubGoClientTest()
        {
            _gitHubGoClient = new ServiceCollection()
                .AddOptions()
                .AddLogging()
                .AddGo()
                .AddGitHubGoClient("majian159", "xxxxxxxxxx")
                .BuildServiceProvider()
                .GetRequiredService<IGitHubGoClient>();
        }

        [Fact]
        public async Task GetGistTest()
        {
            var gists = await _gitHubGoClient.GetGistsAsync("majian159");

            Assert.NotEmpty(gists);

            var gist = gists.First();

            Assert.NotEmpty(gist.Files);
        }
    }
}*/
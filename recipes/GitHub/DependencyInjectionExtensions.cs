/*using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Rabbit.Go.Core;
using Rabbit.Go.Interceptors;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Go.GitHub
{
    public class GitHubGoClientOptions
    {
        public string AccessToken { get; set; }
    }

    public static class DependencyInjectionExtensions
    {
        internal class GitHubGoClientRequestInterceptor : IAsyncRequestInterceptor
        {
            private readonly GitHubGoClientOptions _options;

            public GitHubGoClientRequestInterceptor(IOptions<GitHubGoClientOptions> optionsAccessor)
            {
                _options = optionsAccessor.Value;
            }

            #region Implementation of IAsyncRequestInterceptor

            public Task OnRequestExecutionAsync(RequestExecutingContext context, RequestExecutionDelegate next)
            {
                var request = context.GoContext.Request;

                const string key = "Authorization";
                if (!request.Headers.TryGetValue(key, out var values) || values == StringValues.Empty)
                {
                    request.AddHeader(key, "Basic " + _options.AccessToken);
                }

                return next();
            }

            #endregion Implementation of IAsyncRequestInterceptor
        }

        public static IServiceCollection AddGitHubGoClient(this IServiceCollection services, string userName, string token)
        {
            return services
                .AddGitHubGoClient(options => { options.AccessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{token}")); });
        }

        public static IServiceCollection AddGitHubGoClient(this IServiceCollection services, Action<GitHubGoClientOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return services
                .Configure(configure)
                .AddSingleton<GitHubGoClientRequestInterceptor>()
                .Configure<GoOptions>(options =>
                {
                    options.Types.Add(typeof(IGitHubGoClient));
                    options.Interceptors.AddService<GitHubGoClientRequestInterceptor>();
                })
                .AddSingleton(s => s.GetRequiredService<IGoFactory>().CreateInstance<IGitHubGoClient>());
        }
    }
}*/
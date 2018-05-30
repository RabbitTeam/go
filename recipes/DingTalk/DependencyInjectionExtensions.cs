using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rabbit.Go.Builder;
using Rabbit.Go.Builder.Internal;
using Rabbit.Go.Core;
using Rabbit.Go.Core.Builder;
using Rabbit.Go.DingTalk.Codec;
using Rabbit.Go.Http;
using System;
using System.Linq;

namespace Rabbit.Go.DingTalk
{
    public class DingTalkGoClientOptions
    {
        public string DefaultAccessToken { get; set; }
        public string BaseAddress { get; set; } = "https://oapi.dingtalk.com";
    }

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDingTalkGoClient(this IServiceCollection services)
        {
            return services.AddDingTalkGoClient(defaultAccessToken: null);
        }

        public static IServiceCollection AddDingTalkGoClient(this IServiceCollection services, string defaultAccessToken)
        {
            return services
                .AddDingTalkGoClient(options => { options.DefaultAccessToken = defaultAccessToken; });
        }

        public static IServiceCollection AddDingTalkGoClient(this IServiceCollection services, Action<DingTalkGoClientOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var goServices = services
                .AddGo()
                .AddOptions()
                .Configure(configure)
                .BuildServiceProvider();

            var dingTalkGoClientOptions = goServices.GetRequiredService<IOptions<DingTalkGoClientOptions>>().Value;
            var app = new GoApplicationBuilder(goServices)
                .UseMiddleware<ReflectiveMiddleware>()
                .UseMiddleware<CodecMiddleware>()
                .Use(async (context, next) =>
                {
                    var defaultAccessToken = dingTalkGoClientOptions.DefaultAccessToken;

                    var request = context.Request;
                    const string key = "access_token";

                    if (!string.IsNullOrEmpty(defaultAccessToken))
                    {
                        var queryString = request.QueryString.Value;
                        if (!request.Query.ContainsKey(key))
                        {
                            request.QueryString = new QueryString(QueryHelpers.AddQueryString(queryString, key, defaultAccessToken));
                        }
                        else
                        {
                            var query = QueryHelpers.ParseQuery(queryString).ToDictionary(i => i.Key, i => i.Value.ToString());
                            query[key] = defaultAccessToken;
                            request.QueryString = new QueryString(QueryHelpers.AddQueryString(string.Empty, query));
                        }
                    }

                    await next();
                })
                .UseMiddleware<HttpRequestMiddleware>()
                .Build();
            var dingTalkGoClient = new GoBuilder(app)
                .Encoder(DingTalkCodec.Encoder)
                .Decoder(DingTalkCodec.Decoder)
                .Target<IDingTalkGoClient>(dingTalkGoClientOptions.BaseAddress);

            services.AddSingleton(dingTalkGoClient);

            return services;
        }
    }
}
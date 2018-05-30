using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.Go.Builder;
using Rabbit.Go.Builder.Internal;
using Rabbit.Go.Core;
using Rabbit.Go.Core.Reflective;
using Rabbit.Go.Http;
using Rabbit.Go.Internal;
using Rabbit.Go.Linq2Rest;

namespace Rabbit.Boot.Starter.Go
{
    public class GoHostingStartup : IHostingStartup
    {
        #region Implementation of IHostingStartup

        /// <inheritdoc/>
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services
                    .AddSingleton<ITemplateParser, TemplateParser>()
                    .AddSingleton<IParameterExpanderLocator, ParameterExpanderLocator>();

                var app = new GoApplicationBuilder(services.BuildServiceProvider());
                app
                    .UseMiddleware<ReflectiveMiddleware>()
                    .UseMiddleware<Linq2RestMiddleware>()
                    .UseMiddleware<CodecMiddleware>()
                    .UseMiddleware<HttpRequestMiddleware>();
            });
        }

        #endregion Implementation of IHostingStartup
    }
}
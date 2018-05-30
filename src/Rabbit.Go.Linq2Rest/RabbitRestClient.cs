using Linq2Rest.Provider;
using Microsoft.AspNetCore.WebUtilities;
using Rabbit.Go;
using Rabbit.Go.Abstractions.Features;
using Rabbit.Go.Core;
using Rabbit.Go.Features;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Cdreader.Services.Linq2Rest
{
    public class RabbitRestClient : IRestClient
    {
        private readonly GoContext _goContext;
        private readonly GoRequestDelegate _invoker;

        public RabbitRestClient(GoContext goContext, GoRequestDelegate invoker)
        {
            var goFeature = goContext.Features.Get<IGoFeature>(); goFeature.Encoder = null;
            goFeature.Decoder = null;

            _goContext = goContext;
            _invoker = invoker;
        }

        #region Implementation of IDisposable

        /// <inheritdoc/>
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion Implementation of IDisposable

        #region Implementation of IRestClient

        private GoContext CloneGoContext()
        {
            var features = new FeatureCollection(_goContext.Features);

            var requestFeature = features.Get<IGoRequestFeature>();
            features.Set<IGoRequestFeature>(new GoRequestFeature
            {
                Scheme = requestFeature.Scheme,
                Host = requestFeature.Host,
                Port = requestFeature.Port,
                Body = requestFeature.Body,
                Headers = requestFeature.Headers,
                Method = requestFeature.Method,
                Path = requestFeature.Path,
                QueryString = requestFeature.QueryString
            });
            features.Set<IGoResponseFeature>(new GoResponseFeature());

            var goContext = new DefaultGoContext(features);
            return goContext;
        }

        public Stream Get(Uri uri)
        {
            var goContext = CloneGoContext();

            var pathAndQuery = uri.PathAndQuery;
            var queryStartIndex = pathAndQuery.IndexOf("?", StringComparison.OrdinalIgnoreCase);

            if (queryStartIndex != -1)
            {
                var queryString = pathAndQuery.Substring(queryStartIndex);

                var query = QueryHelpers.ParseNullableQuery(queryString);
                goContext.Request.QueryString = new QueryString(QueryHelpers.AddQueryString(goContext.Request.QueryString.ToString(),
                    query.ToDictionary(i => i.Key, i => i.Value.ToString())));
            }

            try
            {
                _invoker(goContext).GetAwaiter().GetResult();
            }
            catch (HttpRequestException)
            {
            }

            var response = goContext.Response;

            return response.StatusCode == 404 ? Stream.Null : response.Body;
        }

        public Stream Post(Uri uri, Stream input)
        {
            return null;
        }

        public Stream Put(Uri uri, Stream input)
        {
            return null;
        }

        public Stream Delete(Uri uri)
        {
            return null;
        }

        private static readonly Uri DefaultServiceBase = new Uri("http://linqtorest");
        public Uri ServiceBase => DefaultServiceBase;

        #endregion Implementation of IRestClient
    }
}
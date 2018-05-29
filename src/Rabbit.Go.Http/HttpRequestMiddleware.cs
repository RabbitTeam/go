using Microsoft.Extensions.Primitives;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rabbit.Go.Http
{
    public class HttpRequestMiddleware
    {
        private readonly GoRequestDelegate _next;
        private readonly HttpClient _httpClient = new HttpClient();

        public HttpRequestMiddleware(GoRequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(GoContext context)
        {
            var requestMessage = CreateHttpRequestMessage(context);

            var responseMessage = await _httpClient.SendAsync(requestMessage);

            await SetGoContextAsync(context, responseMessage);

            await _next(context);
        }

        private static string GetHostString(GoRequest request)
        {
            var host = request.Host;
            var port = request.Port ?? -1;
            if (port <= -1)
                return host;

            switch (request.Scheme)
            {
                case "https" when port != 443:
                    return host + ":" + port;

                case "http" when port != 80:
                    return host + ":" + port;
            }

            return host;
        }

        private static HttpRequestMessage CreateHttpRequestMessage(GoContext context)
        {
            var request = context.Request;
            var hostString = GetHostString(context.Request);

            var requestUrl = $"{request.Scheme}://{hostString}{request.Path}{request.QueryString}";
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), requestUrl);

            if (request.Body == Stream.Null)
                requestMessage.Content = new StringContent(string.Empty);
            else
                requestMessage.Content = new StreamContent(request.Body);

            foreach (var header in request.Headers)
            {
                requestMessage.Content.Headers.Remove(header.Key);
                requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
//                requestMessage.Content.Headers.Add(header.Key, header.Value.ToArray());
            }

            return requestMessage;
        }

        private static async Task SetGoContextAsync(GoContext context, HttpResponseMessage responseMessage)
        {
            var response = context.Response;

            foreach (var header in responseMessage.Content.Headers)
                response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));

            response.StatusCode = (int)responseMessage.StatusCode;
            response.Body = await responseMessage.Content.ReadAsStreamAsync();
        }
    }
}
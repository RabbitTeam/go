using Microsoft.AspNetCore.WebUtilities;
using Rabbit.Go;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Client
{
    public class SignatureMiddleware
    {
        private readonly GoRequestDelegate _next;

        public SignatureMiddleware(GoRequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(GoContext context)
        {
            var request = context.Request;
            var query = request.Query;

            if (query.Any())
            {
                var content = string.Join(string.Empty, query.OrderBy(i => i.Key).Select(i => i.Value.ToString()));

                var signValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
                request.QueryString = new QueryString(QueryHelpers.AddQueryString(request.QueryString.Value, "sign", signValue));
            }
            await _next(context);
        }
    }
}
using Cdreader.Services.Linq2Rest;
using Linq2Rest.Provider;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.Go.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Go.Linq2Rest
{
    public class Linq2RestMiddleware
    {
        private readonly GoRequestDelegate _next;

        public Linq2RestMiddleware(GoRequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(GoContext context)
        {
            var goFeature = context.Features.Get<IGoFeature>();

            var responseType = goFeature.ResponseType;
            if (!typeof(IQueryable).IsAssignableFrom(responseType))
            {
                await _next(context);
                return;
            }

            var elementType = responseType.GenericTypeArguments[0];

            if (elementType == null)
            {
                return;
            }

            goFeature.ResponseType = GetArrayType(elementType);

            var contextType = GetRestContextType(elementType);

            var instance = ActivatorUtilities.CreateInstance(null, contextType, new RabbitRestClient(context, _next), GoSerializerFactory.SerializerFactory);

            goFeature.ResponseInstance = contextType.GetProperty("Query").GetValue(instance);
        }

        private static readonly ConcurrentDictionary<Type, Type> ArrayTypes = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> RestContextTypes = new ConcurrentDictionary<Type, Type>();

        private static Type GetArrayType(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException(nameof(elementType));
            }

            return ArrayTypes.GetOrAdd(elementType, key => Array.CreateInstance(elementType, 0).GetType());
        }

        private static Type GetRestContextType(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException(nameof(elementType));
            }

            return RestContextTypes.GetOrAdd(elementType, key => typeof(RestContext<>).MakeGenericType(elementType));
        }
    }
}
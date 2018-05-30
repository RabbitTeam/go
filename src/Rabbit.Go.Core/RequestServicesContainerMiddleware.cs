using Microsoft.Extensions.DependencyInjection;
using Rabbit.Go.Abstractions.Features;
using System;
using System.Threading.Tasks;

namespace Rabbit.Go.Core
{
    public class RequestServicesContainerMiddleware
    {
        private readonly GoRequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestServicesContainerMiddleware(GoRequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public Task Invoke(GoContext context)
        {
            var features = context.Features;
            var servicesFeature = features.Get<IServiceProvidersFeature>();

            // All done if RequestServices is set
            if (servicesFeature?.RequestServices != null)
            {
                return _next.Invoke(context);
            }

            var requestServicesFeature = new RequestServicesFeature(_scopeFactory);
            try
            {
                features.Set<IServiceProvidersFeature>(requestServicesFeature);
                return _next.Invoke(context);
            }
            finally
            {
                requestServicesFeature.Dispose();
            }
        }

        public class RequestServicesFeature : IServiceProvidersFeature, IDisposable
        {
            private readonly IServiceScopeFactory _scopeFactory;
            private IServiceProvider _requestServices;
            private IServiceScope _scope;
            private bool _requestServicesSet;

            public RequestServicesFeature(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            #region Implementation of IServiceProvidersFeature

            /// <inheritdoc/>
            public IServiceProvider RequestServices
            {
                get
                {
                    if (_requestServicesSet)
                        return _requestServices;

                    _scope = _scopeFactory.CreateScope();
                    _requestServices = _scope.ServiceProvider;
                    _requestServicesSet = true;
                    return _requestServices;
                }

                set
                {
                    _requestServices = value;
                    _requestServicesSet = true;
                }
            }

            #endregion Implementation of IServiceProvidersFeature

            #region IDisposable

            /// <inheritdoc/>
            public void Dispose()
            {
                _scope?.Dispose();
                _scope = null;
                _requestServices = null;
            }

            #endregion IDisposable
        }
    }
}
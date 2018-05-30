using System;

namespace Rabbit.Go.Abstractions.Features
{
    public class ServiceProvidersFeature : IServiceProvidersFeature
    {
        #region Implementation of IServiceProvidersFeature

        /// <inheritdoc/>
        public IServiceProvider RequestServices { get; set; }

        #endregion Implementation of IServiceProvidersFeature
    }
}
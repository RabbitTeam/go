using System;

namespace Rabbit.Go.Abstractions.Features
{
    public class ServiceProvidersFeature : IServiceProvidersFeature
    {
        public IServiceProvider RequestServices { get; set; }
    }
}

using System;

namespace Rabbit.Go.Abstractions.Features
{
    public interface IServiceProvidersFeature
    {
        IServiceProvider RequestServices { get; set; }
    }
}

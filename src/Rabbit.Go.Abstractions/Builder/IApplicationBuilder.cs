using Rabbit.Go.Features;
using System;
using System.Collections.Generic;

namespace Rabbit.Go.Builder
{
    public interface IGoApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; set; }
        IFeatureCollection ServerFeatures { get; }
        IDictionary<string, object> Properties { get; }

        IGoApplicationBuilder Use(Func<GoRequestDelegate, GoRequestDelegate> middleware);

        IGoApplicationBuilder New();

        GoRequestDelegate Build();
    }
}
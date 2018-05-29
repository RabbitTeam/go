using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Rabbit.Go.Features
{
    public interface IHeaderDictionary : IDictionary<string, StringValues>
    {
        new StringValues this[string key] { get; set; }

        long? ContentLength { get; set; }
    }
}
using System.Collections.Generic;

namespace Rabbit.Go.Features
{
    public interface IItemsFeature
    {
        IDictionary<object, object> Items { get; set; }
    }
}

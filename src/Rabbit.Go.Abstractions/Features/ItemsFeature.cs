using Rabbit.Go.Features;
using System.Collections.Generic;

namespace Rabbit.Go.Abstractions.Features
{
    public class ItemsFeature : IItemsFeature
    {
        public ItemsFeature()
        {
            Items = new Dictionary<object, object>();
        }

        public IDictionary<object, object> Items { get; set; }
    }
}
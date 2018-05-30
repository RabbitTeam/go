using Rabbit.Go.Abstractions.Builder.Internal;
using Rabbit.Go.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Go.Builder.Internal
{
    public class GoApplicationBuilder : IGoApplicationBuilder
    {
        private readonly IList<Func<GoRequestDelegate, GoRequestDelegate>> _components = new List<Func<GoRequestDelegate, GoRequestDelegate>>();

        public GoApplicationBuilder(IServiceProvider serviceProvider)
        {
            Properties = new Dictionary<string, object>();
            ApplicationServices = serviceProvider;
        }

        private GoApplicationBuilder(GoApplicationBuilder builder)
        {
            Properties = builder.Properties;
        }

        public IServiceProvider ApplicationServices
        {
            get => GetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices);
            set => SetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices, value);
        }

        public IFeatureCollection ServerFeatures => GetProperty<IFeatureCollection>(Constants.BuilderProperties.ServerFeatures);

        public IDictionary<string, object> Properties { get; }

        private T GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out var value) ? (T)value : default(T);
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        public IGoApplicationBuilder Use(Func<GoRequestDelegate, GoRequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public IGoApplicationBuilder New()
        {
            return new GoApplicationBuilder(this);
        }

        public GoRequestDelegate Build()
        {
            GoRequestDelegate app = context => Task.CompletedTask;

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }
    }
}
using Castle.DynamicProxy;
using Rabbit.Go.Abstractions.Features;
using Rabbit.Go.Codec;
using Rabbit.Go.Core.Reflective;
using Rabbit.Go.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Go.Core.Builder
{
    public class GoBuilder
    {
        private readonly GoRequestDelegate _invoker;
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
        private static readonly IMethodDescriptorTable MethodDescriptorTable = new MethodDescriptorTable();
        private readonly Dictionary<string, Action<GoContext>> _actions = new Dictionary<string, Action<GoContext>>();

        public GoBuilder(GoRequestDelegate invoker)
        {
            _invoker = invoker;
        }

        public GoBuilder Encoder(IEncoder encoder)
        {
            _actions[nameof(Encoder)] = context => context.Features.Get<IGoFeature>().Encoder = encoder;
            return this;
        }

        public GoBuilder Decoder(IDecoder decoder)
        {
            _actions[nameof(Decoder)] = context => context.Features.Get<IGoFeature>().Decoder = decoder;
            return this;
        }

        private GoBuilder SetBaseAddress(string url)
        {
            _actions[nameof(SetBaseAddress)] = context =>
            {
                var uri = new Uri(url);
                var request = context.Request;
                request.Scheme = uri.Scheme;
                request.Host = uri.Host;
                if (!uri.IsDefaultPort)
                    request.Port = uri.Port;
            };

            return this;
        }

        private GoInterceptor _goInterceptor;

        public T Target<T>(string url)
        {
            if (_goInterceptor == null)
            {
                SetBaseAddress(url);
                foreach (var method in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    MethodDescriptorTable.Set(method);

                _goInterceptor = new GoInterceptor(() =>
                {
                    var features = new FeatureCollection();

                    features.Set<IGoRequestFeature>(new GoRequestFeature());
                    features.Set<IGoResponseFeature>(new GoResponseFeature());
                    features.Set<IGoFeature>(new GoFeature());
                    features.Set<IReflectiveFeature>(new ReflectiveFeature());
                    var context = new DefaultGoContext(features);

                    foreach (var action in _actions.Values)
                    {
                        action(context);
                    }

                    return context;
                }, _invoker, MethodDescriptorTable);
            }

            return (T)ProxyGenerator.CreateInterfaceProxyWithoutTarget(typeof(T), Enumerable.Empty<Type>().ToArray(), _goInterceptor);
        }
    }
}
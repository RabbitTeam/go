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

        private static void SetBaseAddress(string url, GoContext context)
        {
            var uri = new Uri(url);
            var request = context.Request;
            request.Scheme = uri.Scheme;
            request.Host = uri.Host;
            if (!uri.IsDefaultPort)
                request.Port = uri.Port;
        }

        // private GoInterceptor _goInterceptor;

        public object Target(Type type, string url)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                MethodDescriptorTable.Set(method);

            var goInterceptor = new GoInterceptor(() =>
            {
                var features = new FeatureCollection();

                features.Set<IGoRequestFeature>(new GoRequestFeature());
                features.Set<IGoResponseFeature>(new GoResponseFeature());
                features.Set<IGoFeature>(new GoFeature());
                features.Set<IReflectiveFeature>(new ReflectiveFeature());
                var context = new DefaultGoContext(features);

                SetBaseAddress(url, context);

                foreach (var action in _actions.Values)
                {
                    action(context);
                }

                return context;
            }, _invoker, MethodDescriptorTable);

            return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, Enumerable.Empty<Type>().ToArray(), goInterceptor);
            /*            if (_goInterceptor == null)
                        {
                            SetBaseAddress(url);
                            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
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

                        return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, Enumerable.Empty<Type>().ToArray(), _goInterceptor);*/
        }

        public T Target<T>(string url)
        {
            return (T)Target(typeof(T), url);
        }
    }
}
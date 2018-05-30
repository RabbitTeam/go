using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Rabbit.Go.Core.Utilities
{
    public static class MethodDescriptorUtilities
    {
        public static MethodDescriptor CreateMethodDescriptor(Type type, MethodInfo methodInfo)
        {
            var (method, address) = ResolveMethodAndAddress(type, methodInfo);

            var descriptor = new MethodDescriptor
            {
                RequestLine = new TemplateString(address),
                RequestMethod = method,
                Parameters = methodInfo.GetParameters().Select(CreateParameterDescriptor).ToArray(),
                Headers = GetHeaders(type, methodInfo)
            };
            return descriptor;
        }

        private static IDictionary<string, TemplateString> GetHeaders(Type type, MethodInfo methodInfo)
        {
            var headerProviders = type.GetCustomAttributes()
                .Concat(methodInfo.GetCustomAttributes())
                .Concat(methodInfo.GetParameters().SelectMany(p => p.GetCustomAttributes())).OfType<IHeaderProvider>()
                .ToArray();

            IDictionary<string, TemplateString> headers = new Dictionary<string, TemplateString>(StringComparer.OrdinalIgnoreCase);
            foreach (var provider in headerProviders)
                headers[provider.Name] = new TemplateString(provider.Value);

            var goHeadersAttribute = methodInfo.GetCustomAttribute<GoHeadersAttribute>();
            if (goHeadersAttribute != null)
            {
                var value = goHeadersAttribute.Value;
                var index = value.IndexOf(':');
                string name;
                if (index == -1)
                {
                    name = value;
                    value = string.Empty;
                }
                else
                {
                    name = value.Substring(0, index);
                    value = value.Substring(index + 1).Trim();
                }

                headers[name] = new TemplateString(value);
            }

            return headers;
        }

        private static (string Method, string Address) ResolveMethodAndAddress(Type type, MethodInfo methodInfo)
        {
            var goRequestLineAttribute = methodInfo.GetCustomAttribute<GoRequestLineAttribute>();
            if (goRequestLineAttribute != null)
            {
                var requestLine = goRequestLineAttribute.RequestLine;
                var endIndex = requestLine.IndexOf(' ');
                var method = requestLine.Substring(0, endIndex);
                requestLine = requestLine.Substring(endIndex + 1);
                return (method, requestLine);
            }
            else
            {
                var typeGoRequestAttribute = type.GetCustomAttributes().OfType<IGoRequestProvider>().SingleOrDefault();
                var methodGoRequestAttribute = methodInfo.GetCustomAttributes().OfType<IGoRequestProvider>().Single();

                var baseAddress = typeGoRequestAttribute?.Value ?? string.Empty;
                var methodAddress = methodGoRequestAttribute.Value;

                if (baseAddress != string.Empty)
                {
                    if (!baseAddress.StartsWith("/"))
                        baseAddress = baseAddress.Insert(0, "/");
                    if (baseAddress.EndsWith("/"))
                        baseAddress = baseAddress.TrimEnd('/');
                }

                if (!methodAddress.StartsWith("/"))
                    methodAddress = methodAddress.Insert(0, "/");
                var method = (methodGoRequestAttribute.Method ?? typeGoRequestAttribute?.Method) ?? HttpMethod.Get.Method;

                return (method, baseAddress + methodAddress);
            }
        }

        private static ParameterDescriptor CreateParameterDescriptor(ParameterInfo parameterInfo)
        {
            var goParameterAttribute = parameterInfo.GetCustomAttribute<GoParameterAttribute>();
            var attributes = parameterInfo.GetCustomAttributes().OfType<object>().ToList();
            if (goParameterAttribute == null)
            {
                goParameterAttribute = new GoParameterAttribute(parameterInfo.Name)
                {
                    Expander = typeof(ToStringParameterExpander)
                };
                attributes.Insert(0, goParameterAttribute);
            }

            return new ParameterDescriptor
            {
                Name = goParameterAttribute.Name,
                ParameterName = parameterInfo.Name,
                Attributes = attributes.ToArray(),
                ParameterType = parameterInfo.ParameterType
            };
        }
    }
}
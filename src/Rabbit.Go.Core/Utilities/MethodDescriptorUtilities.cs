using System;
using System.Linq;
using System.Reflection;

namespace Rabbit.Go.Core.Utilities
{
    public static class MethodDescriptorUtilities
    {
        public static MethodDescriptor[] CreateMethodDescriptors(Type type)
        {
            return type.GetMethods().Select(CreateMethodDescriptor).ToArray();
        }

        public static MethodDescriptor CreateMethodDescriptor(MethodInfo methodInfo)
        {
            var requestLine = methodInfo.GetCustomAttribute<GoRequestLineAttribute>().RequestLine;
            var endIndex = requestLine.IndexOf(' ');
            var method = requestLine.Substring(0, endIndex);
            requestLine = requestLine.Substring(endIndex + 1);

            var descriptor = new MethodDescriptor
            {
                RequestLine = new TemplateString(requestLine),
                RequestMethod = method,
                Parameters = methodInfo.GetParameters().Select(CreateParameterDescriptor).ToArray(),
                Headers = methodInfo.GetCustomAttributes<GoHeadersAttribute>()
                    .Select(i => i.Value)
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Select(value =>
                    {
                        var index = value.IndexOf(':');
                        if (index == -1)
                            return new
                            {
                                Name = value,
                                Value = string.Empty
                            };
                        return new
                        {
                            Name = value.Substring(0, index),
                            Value = value.Substring(index + 1).Trim()
                        };
                    })
                    .ToDictionary(i => i.Name, i => new TemplateString(i.Value))
            };
            return descriptor;
        }

        private static ParameterDescriptor CreateParameterDescriptor(ParameterInfo parameterInfo)
        {
            return new ParameterDescriptor
            {
                Name = parameterInfo.GetCustomAttribute<GoParameterAttribute>()?.Name ?? parameterInfo.Name,
                ParameterName = parameterInfo.Name,
                Attributes = parameterInfo.GetCustomAttributes().OfType<object>().ToArray(),
                ParameterType = parameterInfo.ParameterType
            };
        }
    }
}
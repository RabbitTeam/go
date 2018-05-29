using Rabbit.Go.Core;
using System;

namespace Rabbit.Go
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class GoParameterAttribute : Attribute
    {
        public GoParameterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Type Expander { get; set; } = typeof(ToStringParameterExpander);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoRequestLineAttribute : Attribute
    {
        public GoRequestLineAttribute(string requestLine)
        {
            RequestLine = requestLine;
        }

        public string RequestLine { get; }
    }

    #region Extensions

    [AttributeUsage(AttributeTargets.Method)]
    public class GoGetAttribute : GoRequestLineAttribute
    {
        /// <inheritdoc/>
        public GoGetAttribute(string requestLine) : base("GET " + requestLine)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoPostAttribute : GoRequestLineAttribute
    {
        /// <inheritdoc/>
        public GoPostAttribute(string requestLine) : base("POST " + requestLine)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoPutAttribute : GoRequestLineAttribute
    {
        /// <inheritdoc/>
        public GoPutAttribute(string requestLine) : base("PUT " + requestLine)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoDeleteAttribute : GoRequestLineAttribute
    {
        /// <inheritdoc/>
        public GoDeleteAttribute(string requestLine) : base("DELETE " + requestLine)
        {
        }
    }

    #endregion Extensions

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class GoHeadersAttribute : Attribute
    {
        public GoHeadersAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class GoBodyAttribute : Attribute
    {
    }
}
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
        public GoRequestLineAttribute()
        {
        }

        public GoRequestLineAttribute(string requestLine)
        {
            RequestLine = requestLine;
        }

        public string RequestLine { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class GoHeadersAttribute : Attribute
    {
        public GoHeadersAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public interface IHeaderProvider
    {
        string Name { get; }
        string Value { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class GoBodyAttribute : Attribute, IHeaderProvider
    {
        public string ContentType { get; set; }

        #region Implementation of IHeaderProvider

        /// <inheritdoc/>
        public string Name { get; } = "Content-Type";

        /// <inheritdoc/>
        public string Value => ContentType ?? "application/json";

        #endregion Implementation of IHeaderProvider
    }

    public interface IGoRequestProvider
    {
        string Value { get; }
        string Method { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class GoRequestAttribute : Attribute, IGoRequestProvider
    {
        public GoRequestAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public string Method { get; set; }
    }

    #region Extensions

    [AttributeUsage(AttributeTargets.Method)]
    public class GoGetAttribute : Attribute, IGoRequestProvider
    {
        /// <inheritdoc/>
        public GoGetAttribute(string value)
        {
            Value = value;
        }

        #region Implementation of IGoRequestProvider

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public string Method { get; } = "GET";

        #endregion Implementation of IGoRequestProvider
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoPostAttribute : Attribute, IGoRequestProvider
    {
        /// <inheritdoc/>
        public GoPostAttribute(string value)
        {
            Value = value;
        }

        #region Implementation of IGoRequestProvider

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public string Method { get; } = "POST";

        #endregion Implementation of IGoRequestProvider
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoPutAttribute : Attribute, IGoRequestProvider
    {
        /// <inheritdoc/>
        public GoPutAttribute(string value)
        {
            Value = value;
        }

        #region Implementation of IGoRequestProvider

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public string Method { get; } = "PUT";

        #endregion Implementation of IGoRequestProvider
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoDeleteAttribute : Attribute, IGoRequestProvider
    {
        /// <inheritdoc/>
        public GoDeleteAttribute(string value)
        {
            Value = value;
        }

        #region Implementation of IGoRequestProvider

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public string Method { get; } = "DELETE";

        #endregion Implementation of IGoRequestProvider
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GoHeadAttribute : Attribute, IGoRequestProvider
    {
        /// <inheritdoc/>
        public GoHeadAttribute(string value)
        {
            Value = value;
        }

        #region Implementation of IGoRequestProvider

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public string Method { get; } = "HEAD";

        #endregion Implementation of IGoRequestProvider
    }

    #endregion Extensions

    /*public interface IGoParameterProvider
    {
        string Name { get; set; }
        ParameterPosition Position { get; }
    }

    public class GoParamAttribute : Attribute, IGoParameterProvider
    {
        public GoParamAttribute(string name)
        {
            Name = name;
        }

        #region Implementation of IGoParameterProvider

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public ParameterPosition Position { get; } = ParameterPosition.Query;

        #endregion Implementation of IGoParameterProvider
    }

    public class GoHeaderAttribute : Attribute, IGoParameterProvider
    {
        public GoHeaderAttribute(string name)
        {
            Name = name;
        }

        #region Implementation of IGoParameterProvider

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public ParameterPosition Position { get; } = ParameterPosition.Header;

        #endregion Implementation of IGoParameterProvider
    }

    public enum ParameterPosition
    {
        Query,
        Header,
    }*/
}
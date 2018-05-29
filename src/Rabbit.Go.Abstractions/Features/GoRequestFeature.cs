using Rabbit.Go.Features;
using System.IO;

namespace Rabbit.Go.Abstractions.Features
{
    public class GoRequestFeature : IGoRequestFeature
    {
        public GoRequestFeature()
        {
            Headers = new HeaderDictionary();
            Body = Stream.Null;
            Scheme = string.Empty;
            Method = string.Empty;
            Path = string.Empty;
            QueryString = string.Empty;
            Host = string.Empty;
            Port = 0;
        }

        #region Implementation of IGoRequestFeature

        /// <inheritdoc/>
        public string Scheme { get; set; }

        /// <inheritdoc/>
        public string Host { get; set; }

        /// <inheritdoc/>
        public string Method { get; set; }

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public string QueryString { get; set; }

        /// <inheritdoc/>
        public IHeaderDictionary Headers { get; set; }

        /// <inheritdoc/>
        public int? Port { get; set; }

        /// <inheritdoc/>
        public Stream Body { get; set; }

        #endregion Implementation of IGoRequestFeature
    }
}
using Rabbit.Go.Features;
using System.IO;

namespace Rabbit.Go.Abstractions.Features
{
    public class GoResponseFeature : IGoResponseFeature
    {
        public GoResponseFeature()
        {
            StatusCode = 200;
            Headers = new HeaderDictionary();
            Body = Stream.Null;
        }

        #region Implementation of IGoResponseFeature

        /// <inheritdoc/>
        public int StatusCode { get; set; }

        /// <inheritdoc/>
        public string ReasonPhrase { get; set; }

        /// <inheritdoc/>
        public IHeaderDictionary Headers { get; set; }

        /// <inheritdoc/>
        public Stream Body { get; set; }

        #endregion Implementation of IGoResponseFeature
    }
}
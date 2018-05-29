using System;
using System.Threading.Tasks;
using Rabbit.Go.Codec;

namespace Rabbit.Go.Core
{
    public interface IGoFeature
    {
        IEncoder Encoder { get; set; }
        IDecoder Decoder { get; set; }
        Type RequestType { get; set; }
        object RequestInstance { get; set; }
        Type ResponseType { get; set; }
        object ResponseInstance { get; set; }
    }

    public class GoFeature : IGoFeature
    {
        #region Implementation of IGoFeature

        /// <inheritdoc />
        public IEncoder Encoder { get; set; }

        /// <inheritdoc />
        public IDecoder Decoder { get; set; }

        /// <inheritdoc/>
        public Type RequestType { get; set; }

        /// <inheritdoc/>
        public object RequestInstance { get; set; }

        /// <inheritdoc/>
        public Type ResponseType { get; set; }

        /// <inheritdoc/>
        public object ResponseInstance { get; set; }

        #endregion Implementation of IGoFeature
    }

/*
    public interface IParameterSetter
    {
        Task SetAsync(GoContext goContext, ParameterDescriptor descriptor, object value);
    }
    public class a
    {
*/

}
using Rabbit.Go.Codec;
using System;

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
        RequestOptions RequestOptions { get; set; }
    }

    public class GoFeature : IGoFeature
    {
        public GoFeature()
        {
            RequestOptions = new RequestOptions();
        }

        #region Implementation of IGoFeature

        /// <inheritdoc/>
        public IEncoder Encoder { get; set; }

        /// <inheritdoc/>
        public IDecoder Decoder { get; set; }

        /// <inheritdoc/>
        public Type RequestType { get; set; }

        /// <inheritdoc/>
        public object RequestInstance { get; set; }

        /// <inheritdoc/>
        public Type ResponseType { get; set; }

        /// <inheritdoc/>
        public object ResponseInstance { get; set; }

        /// <inheritdoc/>
        public RequestOptions RequestOptions { get; set; }

        #endregion Implementation of IGoFeature
    }

    public class RequestOptions
    {
        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public uint Timeout { get; set; } = 30;

        /// <summary>
        /// 重试次数
        /// </summary>
        public uint Retryer { get; set; } = 3;
    }
}
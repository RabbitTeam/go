using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rabbit.Go.Codec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Go.DingTalk.Codec
{
    public class DingTalkCodec
    {
        public static IEncoder Encoder { get; } = new DingTalkEncoder();
        public static IDecoder Decoder { get; } = new DingTalkDecoder();

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private class DingTalkDecoder : IDecoder
        {
            #region Implementation of IDecoder

            /// <inheritdoc/>
            public async Task<object> DecodeAsync(GoResponse response, Type type)
            {
                using (var streamReader = new StreamReader(response.Body))
                {
                    var json = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject(json, type, SerializerSettings);
                }
            }

            #endregion Implementation of IDecoder
        }

        private class DingTalkEncoder : IEncoder
        {
            #region Implementation of IEncoder

            /// <inheritdoc/>
            public Task EncodeAsync(object instance, Type type, GoRequest request)
            {
                if (!(instance is DingTalkMessage dingTalkMessage))
                    return Task.CompletedTask;

                var messageType = dingTalkMessage.Type.ToString();

                messageType = messageType[0].ToString().ToLower() + messageType.Substring(1);
                var propertyName = messageType;

                var dictionary = new
                    Dictionary<string, object>
                    {
                        {"msgtype",messageType },
                        {propertyName, instance}
                    };

                if (instance is IAtMessage atMessage)
                {
                    dictionary["at"] = new
                    {
                        atMobiles = atMessage.AtMobiles,
                        isAtAll = atMessage.IsAtAll
                    };
                }

                request.Headers["Content-Type"] = "application/json";
                var json = JsonConvert.SerializeObject(dictionary, SerializerSettings);
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));

                return Task.CompletedTask;
            }

            #endregion Implementation of IEncoder
        }
    }
}
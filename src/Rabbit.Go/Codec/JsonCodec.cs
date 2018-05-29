using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Go.Codec
{
    public class JsonEncoder : IEncoder
    {
        public static JsonEncoder Instance { get; } = new JsonEncoder();

        #region Implementation of IEncoder

        /// <inheritdoc/>
        public Task EncodeAsync(object instance, Type type, GoRequest request)
        {
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(instance)));
            return Task.CompletedTask;
        }

        #endregion Implementation of IEncoder
    }

    public class JsonDecoder : IDecoder
    {
        public static JsonDecoder Instance { get; } = new JsonDecoder();

        #region Implementation of IDecoder

        /// <inheritdoc/>
        public async Task<object> DecodeAsync(GoResponse response, Type type)
        {
            var reader = new StreamReader(response.Body);
            var content = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject(content, type);
        }

        #endregion Implementation of IDecoder
    }
}
/*using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.Go.Codec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Go.GitHub
{
    public class GistCodecAttribute : Attribute, ICodec, IDecoder
    {
        private readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new GitHubContractResolver()
        });

        public GistCodecAttribute()
        {
            Decoder = this;
        }

        #region Implementation of ICodec

        public IEncoder Encoder { get; } = null;
        public IDecoder Decoder { get; }

        #endregion Implementation of ICodec

        #region Implementation of IDecoder

        public async Task<object> DecodeAsync(GoResponse response, Type type)
        {
            using (var streamReader = new StreamReader(response.Content))
            {
                var json = await streamReader.ReadToEndAsync();
                var array = JArray.Parse(json);

                var models = new List<GistModel>(array.Count);
                foreach (var item in array)
                {
                    var model = item.ToObject<GistModel>(_serializer);

                    model.Files = ((JObject)item.SelectToken("files")).Properties()
                        .Select(t => t.Value.ToObject<GistFileModel>(_serializer)).ToArray();

                    models.Add(model);
                }

                return models.ToArray();
            }
        }

        #endregion Implementation of IDecoder
    }
}*/
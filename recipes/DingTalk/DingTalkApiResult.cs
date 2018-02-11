using Newtonsoft.Json;

namespace Rabbit.Go.DingTalk
{
    public struct DingTalkApiResult
    {
        public bool Success => Code == 0;

        [JsonProperty("errcode")]
        public int Code { get; set; }

        [JsonProperty("errmsg")]
        public string Message { get; set; }
    }
}
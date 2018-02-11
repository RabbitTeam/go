using Newtonsoft.Json;

namespace Rabbit.Go.DingTalk
{
    public abstract class DingTalkMessage
    {
        [JsonIgnore]
        public abstract MessageType Type { get; }
    }
}
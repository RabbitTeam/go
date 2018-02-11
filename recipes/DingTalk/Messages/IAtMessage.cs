using Newtonsoft.Json;

namespace Rabbit.Go.DingTalk
{
    public interface IAtMessage
    {
        [JsonIgnore]
        string[] AtMobiles { get; set; }

        [JsonIgnore]
        bool IsAtAll { get; set; }
    }
}
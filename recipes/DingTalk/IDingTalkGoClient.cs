using System.Threading.Tasks;

namespace Rabbit.Go.DingTalk
{
    public interface IDingTalkGoClient
    {
        [GoPost("/robot/send?access_token={accessToken}")]
        Task<DingTalkApiResult> SendAsync([GoBody]DingTalkMessage message, string accessToken);
    }
}
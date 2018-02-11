using Microsoft.Extensions.DependencyInjection;
using Rabbit.DingTalk.Go;
using System.Threading.Tasks;
using Xunit;

namespace Rabbit.Go.DingTalk.Test
{
    public class DingTalkGoClientTest
    {
        private readonly IDingTalkGoClient _dingTalkGoClient;

        public DingTalkGoClientTest()
        {
            _dingTalkGoClient = new ServiceCollection()
                .AddOptions()
                .AddLogging()
                .AddGo()
                .AddDingTalkGoClient("xxxxxxxxxx")
                .BuildServiceProvider()
                .GetRequiredService<IDingTalkGoClient>();
        }

        [Fact]
        public async Task SendTextTest()
        {
            var result = await _dingTalkGoClient.SendTextAsync("hello world");
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendLinkTest()
        {
            var result = await _dingTalkGoClient.SendLinkAsync(new LinkMessage("link message", "content",
                "http://bing.com", "https://cn.bing.com/az/hprichbg/rb/TeRewaRewa_ZH-CN9356115127_1920x1080.jpg"));
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendMarkdownTest()
        {
            var result = await _dingTalkGoClient.SendMarkdownAsync("markdown message", "# title");
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendActionCardTest()
        {
            var result = await _dingTalkGoClient.SendActionCardAsync(new ActionCardMessage("title", "content")
            {
                Buttons =
                {
                    new ActionCardMessage.ActionCardButton("btn1", "http://bing.com"),
                    new ActionCardMessage.ActionCardButton("btn2", "http://bing.com"),
                    new ActionCardMessage.ActionCardButton("btn3", "http://bing.com")
                },
                BtnOrientation = Orientation.Vertical,
                HideAvatar = false
            });
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendSingleActionCardTest()
        {
            var result = await _dingTalkGoClient.SendSingleActionCardAsync(new SingleActionCardMessage("title", "content", "singleTitle", "https://cn.bing.com/az/hprichbg/rb/TeRewaRewa_ZH-CN9356115127_1920x1080.jpg")
            {
                BtnOrientation = Orientation.Vertical,
                HideAvatar = false
            });
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendFeedCardTest()
        {
            var result = await _dingTalkGoClient.SendFeedCardAsync(new FeedCardMessage
            {
                Items =
                {
                    new FeedCardMessage.FeedCardItem("title1", "http://bing.com",
                        "https://cn.bing.com/az/hprichbg/rb/TeRewaRewa_ZH-CN9356115127_1920x1080.jpg"),
                    new FeedCardMessage.FeedCardItem("title2", "http://bing.com",
                        "https://cn.bing.com/az/hprichbg/rb/TeRewaRewa_ZH-CN9356115127_1920x1080.jpg")
                }
            });
            Assert.True(result.Success);
        }
    }
}
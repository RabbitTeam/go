using Linq2Rest.Provider;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.Go;
using Rabbit.Go.Builder;
using Rabbit.Go.Builder.Internal;
using Rabbit.Go.Codec;
using Rabbit.Go.Core;
using Rabbit.Go.Core.Builder;
using Rabbit.Go.Core.Reflective;
using Rabbit.Go.Http;
using Rabbit.Go.Internal;
using Rabbit.Go.Linq2Rest;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rabbit.Go.DingTalk;
using Rabbit.Go.DingTalk.Codec;
using HttpMethod = System.Net.Http.HttpMethod;

namespace ConsoleApp
{
    public class SpeechModel
    {
        public long Id { get; set; }

        /// <summary>
        /// 关联id，如果是评论则是BookId，如果是回复则是评论id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 评论者Id
        /// </summary>
        public long SenderId { get; set; }

        /// <summary>
        /// 用户来源 0：畅读，1：一起读书
        /// </summary>
        public int UserSource { get; set; }

        /// <summary>
        /// 评论者昵称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 是否使用自定义头像
        /// </summary>
        public int HeadId { get; set; }

        /*/// <summary>
        /// 评论者的经验值 </summary>
        public int SenderExp { get; set; }

        public int SenderLv { get; set; }*/

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime SendTime { get; set; }

        public int SupportCount { get; set; }

        /// <summary>
        /// 评论鉴别 0,1：正常评论 2：垃圾评论
        /// </summary>
        public int Classify { get; set; }

        public string Classification { get; set; }
    }

    public class CommentModel : SpeechModel
    {
        /// <summary>
        /// 评论章节ID
        /// </summary>
        public long ChapterId { get; set; }

        public int ReplyCount { get; set; }
    }

    public class BookCommentModel : CommentModel
    {
        public bool IsTop { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
        public long ExCommentId { get; set; }

        /// <summary>
        /// 评论来源 0：小说 1：有声
        /// </summary>
        public int CommentSource { get; set; }
    }

    public class MyClass
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public interface ITestClient
    {
        [GoPut("/MiniProgramFormId/{appId}/{openId}/{formId}")]
        Task PutAsync(string appId, string openId, string formId, [GoBody]MyClass body);
        /*        [GoGet("/accesstoken/{appId}")]
                Task<MyClass> GetAsync(string appId);


                [GoGet("/book/{bookId}/commentary")]
                IQueryable<BookCommentModel> GetQueryable(long bookId);*/

        [GoRequest("/{query}",Method = "get")]
        Task Get(string query);
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            /*            var services = new ServiceCollection()
                            .AddSingleton<ITemplateParser, TemplateParser>()
                            .AddSingleton<IParameterExpanderLocator, ParameterExpanderLocator>()
                            .BuildServiceProvider();
                        var app = new GoApplicationBuilder(services);

                        app
                            .UseMiddleware<ReflectiveMiddleware>()
                            .UseMiddleware<Linq2RestMiddleware>()
                            .UseMiddleware<CodecMiddleware>()
                            .UseMiddleware<HttpRequestMiddleware>();

                        var invoker = app.Build();

                        var goBuilder = new GoBuilder(invoker)
                            .Encoder(DingTalkCodec.Encoder)
                            .Decoder(DingTalkCodec.Decoder);*/

            var services=new ServiceCollection()
                .AddDingTalkGoClient("049f81883f1c4896fc7d65423d8608607c31d6c922c0c1ff5f4453dc3a81cd04")
                .BuildServiceProvider();

            var dingTalkGoClient = services.GetRequiredService<IDingTalkGoClient>();

            dingTalkGoClient
                .SendTextAsync("123")
                .GetAwaiter().GetResult();
            return;

            //            https://oapi.dingtalk.com/robot/send?access_token=049f81883f1c4896fc7d65423d8608607c31d6c922c0c1ff5f4453dc3a81cd04

            //cartoon 7703
            //wechat 7707
            /*var testClient = goBuilder.Target<ITestClient>("http://192.168.100.150:7707");

            testClient.PutAsync("appId","openId","formId",new MyClass
            {
                access_token = "123",
                expires_in = 100
            }).GetAwaiter().GetResult();*/

//            testClient.Get("$take=10").GetAwaiter().GetResult();

/*            var queryable = testClient.GetQueryable(32087072);

            var t = queryable.Take(10).ExecuteAsync().GetAwaiter().GetResult();
            t = queryable.Take(5).ExecuteAsync().GetAwaiter().GetResult();

            var tqueryable = queryable.Skip(5);
            tqueryable = tqueryable.Take(10);
            t = tqueryable.ExecuteAsync().GetAwaiter().GetResult();

            t = queryable.Where(i => i.IsTop).ExecuteAsync().GetAwaiter().GetResult();*/

            // Console.WriteLine(JsonConvert.SerializeObject(testClient.GetAsync("wx3a1ba93c7fc5b2af").GetAwaiter().GetResult()));
            // testClient.PutAsync("appId", "openId", "formId", new MyClass { access_token = "token",
            // expires_in = 100 }).GetAwaiter().GetResult();
        }
    }
}
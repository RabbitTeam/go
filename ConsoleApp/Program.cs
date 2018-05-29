using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rabbit.Go;
using Rabbit.Go.Builder;
using Rabbit.Go.Builder.Internal;
using Rabbit.Go.Core;
using Rabbit.Go.Core.Builder;
using Rabbit.Go.Core.Codec;
using Rabbit.Go.Core.Reflective;
using Rabbit.Go.Http;
using Rabbit.Go.Internal;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class MyClass
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public interface ITestClient
    {
        [GoGet("/accesstoken/{appId}")]
        Task<MyClass> GetAsync([GoParameter("appId")]string appId);

        [GoHeaders("Content-Type:application/json")]
        [GoPut("/MiniProgramFormId/{appId}/{openId}/{formId}")]
        Task PutAsync([GoParameter("appId")]string appId, [GoParameter("openId")]string openId, [GoParameter("formId")]string formId, MyClass body);
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddSingleton<ITemplateParser, TemplateParser>()
                .AddSingleton<IParameterExpanderLocator, ParameterExpanderLocator>()
                .AddSingleton<ToStringParameterExpander>()
                .BuildServiceProvider();
            var app = new GoApplicationBuilder(services);

            app
                .UseMiddleware<ReflectiveMiddleware>()
                .UseMiddleware<CodecMiddleware>()
                .UseMiddleware<HttpRequestMiddleware>();

            var invoker = app.Build();

            var codec = new JsonCodec();
            var goBuilder = new GoBuilder(invoker)
                .Encoder(codec.Encoder)
                .Decoder(codec.Decoder);

            var testClient = goBuilder.Target<ITestClient>("http://192.168.100.150:7707");

            Console.WriteLine(JsonConvert.SerializeObject(testClient.GetAsync("wx3a1ba93c7fc5b2af").GetAwaiter().GetResult()));
            testClient.PutAsync("appId", "openId", "formId", new MyClass { access_token = "token", expires_in = 100 }).GetAwaiter().GetResult();
        }
    }
}
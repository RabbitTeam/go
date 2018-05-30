using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rabbit.Go;
using Rabbit.Go.Builder;
using Rabbit.Go.Builder.Internal;
using Rabbit.Go.Codec;
using Rabbit.Go.Core;
using Rabbit.Go.Core.Builder;
using Rabbit.Go.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Client
{
    internal class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection()
                .AddGo()
                .BuildServiceProvider();

            var app = new GoApplicationBuilder(services)
                .UseMiddleware<ReflectiveMiddleware>()
                .Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (HttpRequestException)
                    {
                    }

                    if (context.Response.StatusCode == 404)
                    {
                        context.Features.Get<IGoFeature>().ResponseInstance = null;
                    }
                })
                .UseMiddleware<CodecMiddleware>()
                .UseMiddleware<SignatureMiddleware>()
                .UseMiddleware<HttpRequestMiddleware>()
                .Build();
            var userGoClient = new GoBuilder(app)
                .Encoder(JsonEncoder.Instance)
                .Decoder(JsonDecoder.Instance)
                .Target<IUserGoClient>("http://localhost:34753");

            {
                var user = await userGoClient.GetAsync(1);
                Console.WriteLine(JsonConvert.SerializeObject(user));
            }

            {
                await userGoClient.DeleteAsync(1);

                var user = await userGoClient.GetAsync(1);
                Console.WriteLine(user);
            }

            {
                var id = await userGoClient.PostAsync(new PostUserModel
                {
                    Age = 25,
                    Password = "123456",
                    UserName = "user"
                });

                Console.WriteLine(id);
                var user = await userGoClient.GetAsync(id);
                Console.WriteLine(JsonConvert.SerializeObject(user));
            }

            {
                await userGoClient.PutAsync(5, new PutUserModel
                {
                    Password = "654321"
                });
                var user = await userGoClient.GetAsync(5);
                Console.WriteLine(JsonConvert.SerializeObject(user));
            }

            {
                var users = await userGoClient.GetUsersAsync(new UserFilter
                {
                    MinAge = 10,
                    MaxAge = 24
                });

                Console.WriteLine(JsonConvert.SerializeObject(users, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                }));
            }
        }
    }
}
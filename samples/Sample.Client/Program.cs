using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Sample.Client
{
    internal class Program
    {
        private static async Task Main()
        {
            var userGoClient = new ServiceCollection()
                .AddOptions()
                .AddGoClient()
                .BuildServiceProvider()
                .GetService<IUserGoClient>();

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
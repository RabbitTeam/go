# Rabbit Go
一个设计为通用的强类型客户端。  
目前只提供了Http的支持  
**NuGet:** https://www.nuget.org/packages/Rabbit.Go.Core
# 1. 一个简单的示例
## 1.1 声明
```
[Go("http://api.com/user")]
[DefaultHeader("access_token", "xxxxxxxxxx")]
[DefaultQuery("project", "samples")]
public interface IUserGoClient
{
    [GoGet("/{id}")]
    Task<UserModel> GetAsync(long id);

    [GoDelete("/{id}")]
    Task DeleteAsync(long id);

    [GoPost]
    Task<long> PostAsync([GoBody]PostUserModel model);

    [GoPut("/{id}")]
    Task PutAsync(long id, [GoBody]PutUserModel model);
}

public class UserModel
{
    [KeyName("user_name")]
    public string UserName { get; set; }

    [CustomFormatter(typeof(PasswordFormatter))]
    public string Password { get; set; }

    public int? Age { get; set; }

    [PropertyIgnore]
    public bool IsAdult { get; set; }
}

public class PasswordFormatter : IKeyValueFormatter
{
    #region Implementation of IKeyValueFormatter

    public Task FormatAsync(KeyValueFormatterContext context)
    {
        var password = context.Model?.ToString();
        if (!string.IsNullOrEmpty(password))
            password = Encoding.UTF8.GetString(Convert.FromBase64String(password));
        context.Result[context.BinderModelName] = password;

        return Task.CompletedTask;
    }

    #endregion Implementation of IKeyValueFormatter
}

public class PostUserModel : UserModel { }

public class PutUserModel : UserModel { }
```
## 1.2 调用
```
var services = new ServiceCollection()
    .AddOptions()
    .AddLogging()
    .AddGoClient()
    .BuildServiceProvider();

var userGoClient = services.GetService<IUserGoClient>();

var user = await userGoClient.GetAsync(1);

await userGoClient.PutAsync(1, new PutUserModel
{
    Age = 20
});
```
# 特性
* 丰富的拦截器
* 全局请求拦截器和模型公约拦截器
* 良好的抽象与实现拆分，扩展非常容易
* 支持url模板
* 可扩展的请求程序（默认 HttpGoClient）
* 不只为Http而设计（针对Grpc、dubbo等其它服务提供者进行了扩展考虑）
# 文档
待补充
# 下一步
1. 在不使用DI的情况下使用
2. 提供服务发现、重试、限流、断路器的支持（集成 Rabbit Cloud）
3. 提供对Grpc的调用支持
4. 提供对dubbo的调用支持

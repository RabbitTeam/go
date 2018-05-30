using Newtonsoft.Json;
using Rabbit.Go;
using System.Threading.Tasks;

namespace Sample.Client
{
    /*    public class SignatureInterceptorAttribute : RequestInterceptorAttribute
        {
            #region Overrides of RequestInterceptorAttribute

            public override void OnRequestExecuting(RequestExecutingContext context)
            {
                var request = context.GoContext.Request;
                var query = request.Query;

                if (!query.Any())
                    return;

                var content = string.Join(string.Empty, query.OrderBy(i => i.Key).Select(i => i.Value.ToString()));
                request.Query("sign", Convert.ToBase64String(Encoding.UTF8.GetBytes(content)));
            }

            #endregion Overrides of RequestInterceptorAttribute
        }

        public class UserCodecAttribute : Attribute, ICodec, IEncoder, IDecoder
        {
            #region Implementation of ICodec

            public IEncoder Encoder => this;
            public IDecoder Decoder => this;

            #endregion Implementation of ICodec

            #region Implementation of IEncoder

            public Task EncodeAsync(object instance, Type type, GoRequest request)
            {
                if (instance is UserModel user && !string.IsNullOrEmpty(user.Password))
                {
                    user.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Password));

                    request.Body(JsonConvert.SerializeObject(user));
                }

                return Task.CompletedTask;
            }

            #endregion Implementation of IEncoder

            #region Implementation of IDecoder

            public async Task<object> DecodeAsync(GoResponse response, Type type)
            {
                using (var reader = new StreamReader(response.Content))
                {
                    var json = await reader.ReadToEndAsync();

                    if (string.IsNullOrEmpty(json))
                        return null;

                    var result = JsonConvert.DeserializeObject(json, type);

                    string DecodePassword(string password)
                    {
                        return string.IsNullOrEmpty(password) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(password));
                    }

                    switch (result)
                    {
                        case UserModel user:
                            user.Password = DecodePassword(user.Password);
                            break;

                        case IEnumerable<UserModel> users:
                            foreach (var user in users)
                            {
                                user.Password = DecodePassword(user.Password);
                            }

                            break;
                    }

                    return result;
                }
            }

            #endregion Implementation of IDecoder
        }*/

    //    [Go("http://localhost:5000/api/user")]
    [GoRequest("/api/user")]
    public interface IUserGoClient
    {
        [GoGet("/{id}")]
        Task<UserModel> GetAsync(long id);

        [GoGet("/")]
        Task<UserModel[]> GetUsersAsync(UserFilter filter);

        [GoDelete("/{id}")]
        Task DeleteAsync(long id);

        [GoPost("/")]
        Task<long> PostAsync([GoBody]PostUserModel model);

        [GoPut("/{id}")]
        Task PutAsync(long id, [GoBody]PutUserModel model);
    }

    public class UserFilter
    {
        public string UserNameKeyword { get; set; }
        public uint? MinAge { get; set; }
        public uint? MaxAge { get; set; }
    }

    public class UserModel
    {
        private const uint AdultAge = 18;

        public string UserName { get; set; }
        public string Password { get; set; }
        public int? Age { get; set; }

        [JsonIgnore]
        public bool IsAdult => Age.HasValue && Age >= AdultAge;
    }

    public class PostUserModel : UserModel { }

    public class PutUserModel : UserModel { }
}
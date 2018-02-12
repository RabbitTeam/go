namespace Sample.AspNetCore.Server.Model
{
    public class UserFilter
    {
        public string UserNameKeyword { get; set; }
        public uint? MinAge { get; set; }
        public uint? MaxAge { get; set; }
    }

    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? Age { get; set; }
    }
}
using System.IO;

namespace Rabbit.Go.Features
{
    public interface IGoRequestFeature
    {
        string Scheme { get; set; }
        string Host { get; set; }
        string Method { get; set; }
        string Path { get; set; }
        string QueryString { get; set; }
        IHeaderDictionary Headers { get; set; }
        int? Port { get; set; }
        Stream Body { get; set; }
    }
}
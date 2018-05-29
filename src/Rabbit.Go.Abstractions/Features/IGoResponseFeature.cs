using System.IO;

namespace Rabbit.Go.Features
{
    public interface IGoResponseFeature
    {
        int StatusCode { get; set; }
        string ReasonPhrase { get; set; }
        IHeaderDictionary Headers { get; set; }
        Stream Body { get; set; }
    }
}
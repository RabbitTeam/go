using Newtonsoft.Json;
using Rabbit.Go.Interceptors;
using System;
using System.Threading.Tasks;

namespace Rabbit.Go.GitHub
{
    [Go("https://api.github.com"), GistCodec]
    [DefaultHeader("User-Agent", "Awesome-Octocat-App")]
    public interface IGitHubGoClient
    {
        [GoGet("/users/{userName}/gists")]
        Task<GistModel[]> GetGistsAsync(string userName);
    }

    public class GistModel
    {
        public string Url { get; set; }
        public string Id { get; set; }

        [JsonIgnore]
        public GistFileModel[] Files { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Description { get; set; }

        [JsonProperty("comments")]
        public int CommentCount { get; set; }

        public bool Truncated { get; set; }
    }

    public class GistFileModel
    {
        public string Filename { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public string RawUrl { get; set; }

        public int Size { get; set; }
    }
}
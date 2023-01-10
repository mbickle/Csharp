using TwitterStream.Interfaces;

namespace TwitterStream.Data
{
    public class Tweet : ITweet
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Hashtags { get; set; }
    }
}

using TwitterStream.Data.Models;

namespace TwitterStream.Interfaces
{
    public interface ITweetStore
    {
        Task AddOrUpdateTweet(Tweet tweet);
        Task AddOrUpdateHashtag(IEnumerable<string> hashtag, long tweetId);
        Task<List<Hashtag>> GetTopHashtags(int count);
        Task<int> GetTweetCount();
    }
}

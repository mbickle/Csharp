using System.Collections.Concurrent;
using System.Text.Json;
using TwitterStream.Interfaces;

namespace TwitterStream.Data
{
    public class TweetStore : ITweetStore
    {
        private static int tweetsCount;
        private static readonly ConcurrentDictionary<string, int> topHashTags = new ConcurrentDictionary<string, int>();

        /// <summary>Adds the tweet.</summary>
        /// <param name="tweet">The tweet.</param>
        public void Add(ITweet tweet)
        {
            Interlocked.Increment(ref tweetsCount);
            if (tweet.Hashtags != null)
            {
                var hashTags = JsonSerializer.Deserialize<List<string>>(tweet.Hashtags);
                if (hashTags != null)
                {
                    for (var i = 0; i < hashTags.Count; i++)
                    {
                        topHashTags.AddOrUpdate(hashTags[i], 0, (key, count) => count + 1);
                    }
                }
            }
        }

        /// <summary>Gets the top hashtags.</summary>
        /// <param name="count">The count.</param>
        /// <returns>
        ///   List of Top Hashtags
        /// </returns>
        public List<string> GetTopHashtags(int count)
        {
            return topHashTags.OrderByDescending(o => o.Value).Take(count).Select(s => s.Key).ToList();
        }

        /// <summary>Gets the count.</summary>
        /// <returns>
        ///   Count of Tweets
        /// </returns>
        public int GetCount()
        {
            return tweetsCount;
        }
    }
}
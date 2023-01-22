using Microsoft.EntityFrameworkCore;
using TwitterStream.Data;
using TwitterStream.Data.Models;
using TwitterStream.Interfaces;

namespace TwitterStream.Service
{
    public class Store : ITweetStore
    {
        private TwitterStreamDbContext dbContext;

        public Store(TwitterStreamDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>Adds the tweet.</summary>
        /// <param name="tweet">The tweet.</param>
        public async Task AddOrUpdateTweet(Tweet tweet)
        {
            if (tweet != null)
            {
                var tweetData = new Tweet()
                {
                    Id = tweet.Id,
                    Content = tweet.Content,
                };

                if (dbContext.Tweets.Any(t => t.Id == tweet.Id))
                {
                    dbContext.Tweets.Update(tweetData);
                }
                else
                {
                    await dbContext.AddAsync(tweetData);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        /// <summary>Adds the or update hashtag.</summary>
        /// <param name="hashtags">The hashtags.</param>
        /// <param name="tweetId">The tweet identifier.</param>
        public async Task AddOrUpdateHashtag(IEnumerable<string> hashtags, long tweetId)
        {
            foreach (var hashtag in hashtags)
            {
                if (!string.IsNullOrWhiteSpace(hashtag))
                {
                    var tag = await dbContext.Hashtags.FirstOrDefaultAsync(t => t.Content == hashtag);

                    if (tag == null)
                    {
                        // First time seeing this tag so need to add.
                        var newtag = new Hashtag
                        {
                            Id = Guid.NewGuid(),
                            TweetId = tweetId,
                            Count = 1,
                            Content = hashtag
                        };

                        await dbContext.Hashtags.AddAsync(newtag);
                    }
                    else
                    {
                        // Tag already exists so let's increment the count.
                        tag.Count++;
                        dbContext.Hashtags.Update(tag);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>Gets the top hashtags.</summary>
        /// <param name="count">The count.</param>
        /// <returns>
        ///   List of Top Hashtags
        /// </returns>
        public async Task<List<Hashtag>> GetTopHashtags(int count)
        {
            return await dbContext.Hashtags
                .AsNoTracking()
                .OrderByDescending(o => o.Count)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>Gets the count.</summary>
        /// <returns>
        ///   Count of Tweets
        /// </returns>
        public async Task<int> GetTweetCount()
        {
            return await dbContext.Tweets
                .AsNoTracking()
                .CountAsync();
        }
    }
}

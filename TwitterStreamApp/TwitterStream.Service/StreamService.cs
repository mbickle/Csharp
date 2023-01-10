using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Polly;
using TwitterStream.Core;
using TwitterStream.Data;
using TwitterStream.Interfaces;

namespace TwitterStream.Service
{
    public class StreamService : ITwitterStreamService
    {
        private readonly ITweetStore store;
        private readonly HttpClient client;
        private readonly ILogger<ITwitterStreamService> logger;
        private readonly ITwitterStreamAppConfiguration configuration;

        /// <summary>Initializes a new instance of the <see cref="StreamService" /> class.</summary>
        /// <param name="store">The store.</param>
        /// <param name="client">The client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public StreamService(
            ITweetStore store, 
            HttpClient client, 
            ILogger<ITwitterStreamService> logger,
            ITwitterStreamAppConfiguration configuration) 
        {
            this.store = store;
            this.client = client;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>Starts the specified cancel token.</summary>
        /// <param name="cancelToken">The cancel token.</param>
        public async Task Start(CancellationTokenSource cancelToken)
        {
            try
            {
                // Retry Policy when Twitter closes a connection.
                // Configured via appsettings
                var retryPolicy = Policy.Handle<OperationCanceledException>()
                    .WaitAndRetryForeverAsync(sleepDurationProvider: _ => TimeSpan.FromSeconds(configuration.RetryWaitSeconds));

                await retryPolicy.ExecuteAsync(async () =>
                {
                    using (var stream = await client.GetStreamAsync(configuration.TwitterUrl, cancelToken.Token))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var line = await reader.ReadLineAsync();

                            while (!string.IsNullOrWhiteSpace(line))
                            {
                                var tweet = DeserializeTweet(line);
                                if (tweet != null)
                                {
                                    Process(tweet);
                                }

                                line = await reader.ReadLineAsync();
                            }

                            logger.LogInformation($"Twitter closed stream, will retry again in {configuration.RetryWaitSeconds} seconds.");
                            throw new OperationCanceledException("Twitter has closed the conneciton.");
                        }
                    }
                });
                
            }
            catch (Exception ex) 
            { 
                cancelToken.Cancel();
                logger.LogError($"Ran into a problem reading Twitter Stream: {ex}");
                throw;
            }
        }

        /// <summary>Processes the specified tweet.</summary>
        /// <param name="tweet">The tweet.</param>
        public void Process(ITweetModel tweet)
        {
            if (!string.IsNullOrEmpty(tweet.Content))
            {
                var hashTags = GetValidHashtags(tweet.Content);
                var tweetResult = new Tweet
                {
                    Id = tweet.TweetId,
                    Content = tweet.Content,
                    Hashtags = JsonSerializer.Serialize(hashTags)
                };

                store.Add(tweetResult);
            }
        }

        /// <summary>Deserializes the tweet.</summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="InvalidDataException">Unable to deserialize tweet. {input}</exception>
        public TweetModel DeserializeTweet(string input)
        {
            Validation.EnsureNotNullOrWhiteSpace(input);

            try
            {
                var tweet = JsonSerializer.Deserialize<TweetDataDto>(input);
                if (tweet == null 
                    || tweet.data== null
                    || tweet.data.id == null
                    || tweet.data.text == null)
                {
                    throw new InvalidDataException($"Unable to deserialize tweet. {input}");
                }

                return new TweetModel(tweet.data.id, tweet.data.text);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error deserializing Tweet: {ex.Message}");
                throw;
            }
        }

        /// <summary>Gets the valid hashtags.</summary>
        /// <param name="input">The input.</param>
        /// <returns>list of valid hashtags</returns>
        /// <remarks>https://stackoverflow.com/questions/36895543/which-characters-are-allowed-in-hashtags</remarks>
        public List<string> GetValidHashtags(string input)
        {
            Validation.EnsureNotNullOrWhiteSpace(input);

            const string HASHTAG_LETTERS = @"\p{L}\p{M}";
            const string HASHTAG_NUMERALS = @"\p{Nd}";
            const string HASHTAG_SPECIAL_CHARS = @"_\u200c\u200d\ua67e\u05be\u05f3\u05f4\uff5e\u301c\u309b\u309c\u30a0\u30fb\u3003\u0f0b\u0f0c\u00b7";
            const string HASHTAG_LETTERS_NUMERALS = HASHTAG_LETTERS + HASHTAG_NUMERALS + HASHTAG_SPECIAL_CHARS;
            const string HASHTAG_LETTERS_NUMERALS_SET = "[" + HASHTAG_LETTERS_NUMERALS + "]";
            const string HASHTAG_LETTERS_SET = "[" + HASHTAG_LETTERS + "]";

            return Regex.Matches(
                input, 
                "(^|[^&" + HASHTAG_LETTERS_NUMERALS + @"])(#|\uFF03)(?!\uFE0F|\u20E3)(" + HASHTAG_LETTERS_NUMERALS_SET + "*" + HASHTAG_LETTERS_SET + HASHTAG_LETTERS_NUMERALS_SET + "*)", 
                RegexOptions.IgnoreCase)
                .OfType<Match>()
                .Select(x => x.Value.Trim()).ToList();
        }
    }
}
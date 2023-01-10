using Microsoft.Extensions.Configuration;
using TwitterStream.Interfaces;

namespace TwitterStream.Core
{
    public class TwitterStreamAppConfiguration : ITwitterStreamAppConfiguration
    {
        /// <summary>
        /// Authorization token
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// Retry Wait In seconds
        /// </summary>
        public int RetryWaitSeconds { get; set; }

        /// <summary>
        /// Refresh seconds
        /// </summary>
        public int RefreshSeconds { get; set; }

        /// <summary>
        /// Twitter Url
        /// </summary>
        public string TwitterUrl { get; set; }

        /// <summary>Initializes a new instance of the <see cref="TwitterStreamAppConfiguration" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public TwitterStreamAppConfiguration(IConfiguration configuration) 
        {
            BearerToken = configuration["BearerToken"];
            RetryWaitSeconds = int.Parse(configuration["RetryWaitSeconds"]);
            RefreshSeconds = int.Parse(configuration["RefreshSeconds"]);
            TwitterUrl = configuration["TwitterUrl"];
        }
    }
}

using Microsoft.Extensions.Logging;
using TwitterStream.Core;
using TwitterStream.Interfaces;

namespace TwitterStream.Reporting
{
    public class ConsoleReport : IReport
    {
        private readonly ITweetStore store;
        private readonly ILogger<ConsoleReport> logger;
        private readonly ITwitterStreamAppConfiguration configuration;

        /// <summary>Initializes a new instance of the <see cref="ConsoleReport" /> class.</summary>
        /// <param name="store">The store.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public ConsoleReport(
            ITweetStore store, 
            ILogger<ConsoleReport> logger, 
            ITwitterStreamAppConfiguration configuration)
        {
            this.store = store;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>Reports the specified cancellation token.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public Task Report(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("");
                logger.LogInformation($"Tweet Count: {store.GetCount()}");
                logger.LogInformation($"Top 10 Hashtags: ");

                var hashtags = store.GetTopHashtags(10);

                foreach (var tag in hashtags)
                {
                    logger.LogInformation($"\t{tag}");
                }

                // Refresh stats every second
                var refreshTime = TimeSpan.FromSeconds(configuration.RefreshSeconds);
                logger.LogInformation("");
                logger.LogInformation($"Waiting {refreshTime} seconds to refresh...");
                Thread.Sleep(TimeSpan.FromSeconds(configuration.RefreshSeconds));
            }

            return Task.CompletedTask;
        }
    }
}
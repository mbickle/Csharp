using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using TwitterStream.Data;
using TwitterStream.Data.Models;
using TwitterStream.Interfaces;
using TwitterStream.Service;

namespace TwitterStreamApp.UnitTests
{
    [TestClass]
    public class TwitterStreamDataTests
    {
        private TwitterStreamDbContext dbContext;
        private DbContextOptionsBuilder<TwitterStreamDbContext> dbContextOptionsBuilder;

        /// <summary>
        /// Mocking TwitterStream.Store()
        /// </summary>
        /// <returns></returns>
        private (Store, Mock<ITweetStore>) CreateTwitterStreamStore()
        {
            Mock<ITweetStore> storeMock = new();
            storeMock.SetReturnsDefault<Guid>(Guid.NewGuid());

            return (new Store(dbContext), storeMock);
        }

        /// <summary>
        /// Mocking TwitterStreamService()
        /// </summary>
        /// <returns></returns>
        private (StreamService, Mock<ITweetStore>) CreateTwitterStreamService()        
        {
            Mock<ILogger<StreamService>> loggerMock = new();
            Mock<ITweetStore> storeMock = new();
            storeMock.SetReturnsDefault<Guid>(Guid.NewGuid());
            Mock<HttpClient> httpClientMock = new();
            Mock<ITwitterStreamAppConfiguration> twitterStreamAppConfigurationMock = new();

            return (new StreamService(
                storeMock.Object, 
                httpClientMock.Object, 
                loggerMock.Object, 
                twitterStreamAppConfigurationMock.Object), storeMock);
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            Trace.WriteLine("Trace.TestInitialize");
            Trace.WriteLine($"{TestContext.TestName}");

            dbContextOptionsBuilder = new DbContextOptionsBuilder<TwitterStreamDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging(true);

            dbContext = new TwitterStreamDbContext(dbContextOptionsBuilder.Options);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
            Trace.WriteLine("Trace.TestCleanup");
        }

        [TestMethod]
        public async Task StoreAdd_Valid()
        {
            var (handlerStore, store) = CreateTwitterStreamStore();
            var (handlerService, service) = CreateTwitterStreamService();
            var tweet = new Tweet()
            {
                Id = 1234,
                Content = "Hello World #helloworld"
            };

            await handlerStore.AddOrUpdateTweet(tweet);

            var hashtags = handlerService.GetValidHashtags(tweet.Content);

            await handlerStore.AddOrUpdateHashtag(hashtags, tweet.Id);
            var tweetCount = await handlerStore.GetTweetCount();
            var topHashtags = await handlerStore.GetTopHashtags(1);

            Assert.IsTrue(tweetCount == 1, "Expected count to be 1");
            Assert.IsTrue(topHashtags.Count == 1, "Expected count to be 1");
            Assert.IsTrue(topHashtags.First().Content == "#helloworld", "Expected hashtag not found.");
        }

        [TestMethod]
        public async Task StoreGetTopHashtags_Valid()
        {
            var (handlerStore, store) = CreateTwitterStreamStore();
            var (handlerService, service) = CreateTwitterStreamService();
            var tweet = new Tweet()
            {
                Id = 1234,
                Content = "Hello World #helloworld",
            };

            var hashtags = handlerService.GetValidHashtags(tweet.Content);

            await handlerStore.AddOrUpdateHashtag(hashtags, tweet.Id);
            await handlerStore.AddOrUpdateHashtag(hashtags, tweet.Id);
            var result = await handlerStore.GetTopHashtags(10);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Any(h => h.Content == "#helloworld"), "Expected hashtag not found");
            Assert.IsTrue(result.Any(h => h.Count == 2), "Expected hashtag count == 2");
        }
    }
}

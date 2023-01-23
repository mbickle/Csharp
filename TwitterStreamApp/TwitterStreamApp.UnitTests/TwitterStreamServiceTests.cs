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
    public class TwitterStreamServiceTests
    {
        private TwitterStreamDbContext dbContext;
        private DbContextOptionsBuilder<TwitterStreamDbContext> dbContextOptionsBuilder;

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
        public async Task ProcessTweet_ContentNoHashtag()
        {
            var (handlerService, service) = CreateTwitterStreamService();
            var tweetString = "{\"data\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year https://t.co/UhH0ZER2b4\"}}";
            
            var tweet = handlerService.DeserializeTweet(tweetString);
            Assert.IsNotNull(tweet);
            Assert.IsTrue(tweet.Id == 1605594247092117505, "Unexpected TweetId");

            await handlerService.Process(tweet);
            service.Verify(call => call.AddOrUpdateTweet(It.IsAny<Tweet>()), Times.Exactly(1), "Should be called once.");
            service.Verify(call => call.AddOrUpdateHashtag(It.IsAny<IEnumerable<string>>(), It.IsAny<long>()), Times.Exactly(0), "Should be called zero, no hashtags.");
        }

        [TestMethod]
        public async Task ProcessTweet_ContentHashtag()
        {
            var (handlerService, service) = CreateTwitterStreamService();
            var tweetString = "{\"data\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year #helloworld https://t.co/UhH0ZER2b4\"}}";

            var tweet = handlerService.DeserializeTweet(tweetString);
            Assert.IsNotNull(tweet);
            Assert.IsTrue(tweet.Id == 1605594247092117505, "Unexpected TweetId");

            await handlerService.Process(tweet);
            service.Verify(call => call.AddOrUpdateTweet(It.IsAny<Tweet>()), Times.Exactly(1), "Should be called once.");
            service.Verify(call => call.AddOrUpdateHashtag(It.IsAny<IEnumerable<string>>(), It.IsAny<long>()), Times.Exactly(1), "Should be called zero, no hashtags.");
        }

        [TestMethod]
        public async Task ProcessTweet_NoContent()
        {
            var (handlerService, service) = CreateTwitterStreamService();
            var tweet = new TweetModel(tweetId: 1234, content: "");

            await handlerService.Process(tweet);
            service.Verify(call => call.AddOrUpdateTweet(It.IsAny<Tweet>()), Times.Exactly(0), "Should be called zero.");
        }

        [TestMethod]
        public void GetValidHashtags_Valid()
        {
            var (handler, _) = CreateTwitterStreamService();
            var tweet = new TweetModel(tweetId: 1234, content: "{\"data\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year https://t.co/UhH0ZER2b4 #science\"}}");

            var results = handler.GetValidHashtags(tweet.Content);
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Contains("#science"), "Expected #science to be in list of hashtags.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetValidHashtags_ExpectedException()
        {
            var (handler, _) = CreateTwitterStreamService();

            var results = handler.GetValidHashtags(null);
        }

        [TestMethod]
        public void DeserializeTweet_Valid()
        {
            var tweet = "{\"data\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year https://t.co/UhH0ZER2b4 #science\"}}";
            var (handler, _) = CreateTwitterStreamService();

            var results = handler.DeserializeTweet(tweet);
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Content);
            Assert.IsNotNull(results.Id);
            Assert.IsTrue(results.Content.Contains("Winter solstice"));
            Assert.IsTrue(results.Id == 1605594247092117505);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void DeserializeTweet_InvalidDataException()
        {
            var tweet = "{\"DATA\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year https://t.co/UhH0ZER2b4 #science\"}}";
            var (handler, _) = CreateTwitterStreamService();

            var results = handler.DeserializeTweet(tweet);
        }
    }
}
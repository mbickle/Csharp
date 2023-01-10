using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using TwitterStream.Core;
using TwitterStream.Interfaces;
using TwitterStream.Service;

namespace TwitterStreamApp.UnitTests
{
    [TestClass]
    public class TwitterStreamServiceTests
    {
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

            return (new StreamService(storeMock.Object, httpClientMock.Object, loggerMock.Object, twitterStreamAppConfigurationMock.Object), storeMock);
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
        public void ProcessTweet_Content()
        {
            var (handler, store) = CreateTwitterStreamService();
            var tweet = new TweetModel(tweetId: Guid.NewGuid().ToString(), content: "{\"data\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year https://t.co/UhH0ZER2b4\"}}");

            handler.Process(tweet);
            store.Verify(call => call.Add(It.IsAny<ITweet>()), Times.Exactly(1), "Should be called once.");
        }

        [TestMethod]
        public void ProcessTweet_NoContent()
        {
            var (handler, store) = CreateTwitterStreamService();
            var tweet = new TweetModel(tweetId: Guid.NewGuid().ToString(), content: "");

            handler.Process(tweet);
            store.Verify(call => call.Add(It.IsAny<ITweet>()), Times.Exactly(0), "Should be called zero.");
        }

        [TestMethod]
        public void GetValidHashtags_Valid()
        {
            var (handler, _) = CreateTwitterStreamService();
            var tweet = new TweetModel(tweetId: Guid.NewGuid().ToString(), content: "{\"data\":{\"edit_history_tweet_ids\":[\"1605594247092117505\"],\"id\":\"1605594247092117505\",\"text\":\"RT @ST0NEHENGE: Winter solstice: The science behind the shortest day of the year https://t.co/UhH0ZER2b4 #science\"}}");

            var results = handler.GetValidHashtags(tweet.Content);
            Assert.IsTrue(results.Count > 0);
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
            Assert.IsNotNull(results.TweetId);
            Assert.IsTrue(results.Content.Contains("Winter solstice"));
            Assert.IsTrue(results.TweetId == "1605594247092117505");
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
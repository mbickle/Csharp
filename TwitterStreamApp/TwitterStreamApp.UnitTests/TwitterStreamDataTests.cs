using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TwitterStream.Data;
using TwitterStream.Interfaces;
using TwitterStream.Service;

namespace TwitterStreamApp.UnitTests
{
    [TestClass]
    public class TwitterStreamDataTests
    {
        /// <summary>
        /// Mocking TwitterStreamData()
        /// </summary>
        /// <returns></returns>
        private (TweetStore, Mock<ITweetStore>) CreateTwitterStreamStore()
        {
            Mock<ITweetStore> storeMock = new();
            storeMock.SetReturnsDefault<Guid>(Guid.NewGuid());

            return (new TweetStore(), storeMock);
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
        public void StoreAdd_Valid()
        {
            var (handler, store) = CreateTwitterStreamStore();
            var tweet = new Tweet()
            {
                Id = "1234",
                Content = "Hello World",
                Hashtags = JsonSerializer.Serialize(new List<string> { "#helloworld" })
            };

            handler.Add(tweet);
            var count = handler.GetCount();
            var hashtags = handler.GetTopHashtags(1);
            Assert.IsTrue(count == 1, "Expected count to be 1");
            Assert.IsTrue(hashtags.Count == 1, "Expected count to be 1");
            Assert.IsTrue(hashtags.First() == "#helloworld", "Expected hashtag not found.");
        }

        [TestMethod]
        public void StoreGetTopHashtags_Valid()
        {
            var (handler, store) = CreateTwitterStreamStore();
            var tweet = new Tweet()
            {
                Id = "1234",
                Content = "Hello World",
                Hashtags = JsonSerializer.Serialize(new List<string> { "#helloworld" })
            };

            handler.Add(tweet);
            var result = handler.GetTopHashtags(10);
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.Contains("#helloworld"), "Expected hashtag not found");
        }
    }
}

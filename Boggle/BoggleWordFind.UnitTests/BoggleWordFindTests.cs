using System.Diagnostics;
using System.Linq;
using BoggleWordFind.Interfaces;

namespace BoggleWordFind.UnitTests
{
    [TestClass]
    public class BoggleWordFindTests
    {
        private BoggleTestable boggleTestable;
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Trace.WriteLine(TestContext.TestName);
            boggleTestable = new BoggleTestable();
        }


        [TestMethod]
        public void SetValidWordSource_Valid()
        {
            var words = new List<string>() { "bar", "car", "care", "dare", "ace", "ok" };
            Assert.IsNull(boggleTestable.WordDictionary);
            boggleTestable.SetValidWordSource(words);
            Assert.IsNotNull(boggleTestable.WordDictionary);
            Assert.IsTrue(boggleTestable.WordDictionary.Count > 0, "Expected to find something in dictionary.");
            Assert.IsTrue(boggleTestable.WordDictionary.ContainsKey('a'), "Dictionary should contain A key.");
            Assert.IsTrue(!boggleTestable.WordDictionary.ContainsKey('z'), "Dictionary shouldn't contain Z key as it's not part of words.");

            var result = new List<string>();
            boggleTestable.WordDictionary.TryGetValue('o', out result);
            Assert.IsNull(result, "O should not be found as 'ok' is not at least 3 chars.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetValidWordSource_Invalid_Exception()
        {
            var words = new List<string>() { "bar", "car", "care", "dare", "ace", "ok" };
            Assert.IsNull(boggleTestable.WordDictionary);
            boggleTestable.SetValidWordSource(null);
        }

        [TestMethod]
        public void SetBoggleBoardState_Valid()
        {
            const int width = 3;
            const int height = 3;

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'a', 'c', 'e' };
            contents[1] = new char[width] { 'a', 'c', 'e' };
            contents[2] = new char[width] { 'a', 'c', 'e' };

            boggleTestable.SetBoggleBoardState(width, height, contents);

            Assert.IsNotNull(boggleTestable.Board, "Board should not be null");
            Assert.IsTrue(boggleTestable.Board.Length == (width * height), "Board should match width * height");
            Assert.IsTrue(boggleTestable.Board[0, 0] == 'a', "Board[0,0] should be 'a'");
            Assert.IsTrue(boggleTestable.Board[2, 2] == 'e', "Board[2,2] should be 'e'");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBoggleBoardState_Invalid_Exception()
        {
            const int width = 3;
            const int height = 3;

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'a', 'c', 'e' };
            contents[1] = new char[width] { 'a', 'c', 'e' };
            contents[2] = new char[width] { 'a', 'c', 'e' };

            boggleTestable.SetBoggleBoardState(width, height, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetBoggleBoardState_Invalid_Contents_NotLetter_Exception()
        {
            const int width = 3;
            const int height = 3;

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'a', 'c', 'e' };
            contents[1] = new char[width] { 'a', 'c', 'e' };
            contents[2] = new char[width] { 'a', 'c', '1' };

            boggleTestable.SetBoggleBoardState(width, height, contents);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetBoggleBoardState_Invalid_Contents_LessThanHeight_Exception()
        {
            const int width = 3;
            const int height = 3;

            char[][] contents = new char[2][];
            contents[0] = new char[width] { 'a', 'c', 'e' };
            contents[1] = new char[width] { 'a', 'c', 'e' };

            boggleTestable.SetBoggleBoardState(width, height, contents);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetBoggleBoardState_Invalid_Contents_LessThanWidth_Exception()
        {
            const int width = 3;
            const int height = 3;

            char[][] contents = new char[2][];
            contents[0] = new char[2] { 'a', 'c' };
            contents[1] = new char[2] { 'a', 'c' };

            boggleTestable.SetBoggleBoardState(width, height, contents);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetBoggleBoardState_Invalid_Contents_Null_Exception()
        {
            const int width = 3;
            const int height = 3;

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'a', 'c', 'e'};
            contents[1] = new char[width] { 'a', 'c', 'e' };

            boggleTestable.SetBoggleBoardState(width, height, contents);
        }

        [TestMethod]
        public void GetFoundWords_Found_Success()
        {
            const int width = 3;
            const int height = 3;
            var words = new List<string>() { "hot", "car", "care", "hoot", "ace", "ace", "to", "too", "chocolate", "chocolates" };

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'c', 'h', 'o' };
            contents[1] = new char[width] { 'l', 'o', 'c' };
            contents[2] = new char[width] { 'a', 't', 'e' };
            
            boggleTestable.SetValidWordSource(words);
            boggleTestable.SetBoggleBoardState(width, height, contents);
            var results = boggleTestable.GetFoundWords();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() == 4, $"Expected to find 4 words, found {results.Count()}");
            Assert.IsTrue(results.Contains("chocolate"), "Expected to find 'chocolate'");
            Assert.IsTrue(results.Contains("hoot"), "Expected to find 'hoot'");
            Assert.IsTrue(results.Contains("too"), "Expected to find 'too'");
        }

        [TestMethod]
        public void GetFoundWords_Found_Large_Success()
        {
            const int width = 25;
            const int height = 25;
            var expectedCount = 3694;

            var words = File.ReadAllLines(@"Files\words.txt");
            string chars = "abcdefghijklmnopqrstuvwxyz";
            char[][] contents = new char[height][];

            for (int i = 0; i < height; i++)
            {
                contents[i] = Utilities.GenerateRandomCharArray(chars, width, i);
            }            

            boggleTestable.SetValidWordSource(words);
            boggleTestable.SetBoggleBoardState(width, height, contents);
            var results = boggleTestable.GetFoundWords();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() == expectedCount, $"Expected to find {expectedCount} words, found {results.Count()}");
        }

        [TestMethod]
        public void GetFoundWords_NotFound_Success()
        {
            const int width = 3;
            const int height = 3;
            var words = new List<string>() { "hat", "car", "care", "hoots", "ace", "to", "chocolates" };

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'c', 'h', 'o' };
            contents[1] = new char[width] { 'l', 'o', 'c' };
            contents[2] = new char[width] { 'a', 't', 'e' };

            boggleTestable.SetValidWordSource(words);
            boggleTestable.SetBoggleBoardState(width, height, contents);
            var results = boggleTestable.GetFoundWords();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() == 0, $"Expected to find 0 words, found {results.Count()}");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetFoundWords_WordDictionary_Exception()
        {
            const int width = 3;
            const int height = 3;
            var words = new List<string>() { "hot", "car", "care", "hoot", "ace", "ace", "to", "too", "chocolate", "chocolates" };

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'c', 'h', 'o' };
            contents[1] = new char[width] { 'l', 'o', 'c' };
            contents[2] = new char[width] { 'a', 't', 'e' };

            boggleTestable.SetBoggleBoardState(width, height, contents);
            var results = boggleTestable.GetFoundWords();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetFoundWords_Board_Exception()
        {
            const int width = 3;
            const int height = 3;
            var words = new List<string>() { "hot", "car", "care", "hoot", "ace", "ace", "to", "too", "chocolate", "chocolates" };

            char[][] contents = new char[height][];
            contents[0] = new char[width] { 'c', 'h', 'o' };
            contents[1] = new char[width] { 'l', 'o', 'c' };
            contents[2] = new char[width] { 'a', 't', 'e' };

            boggleTestable.SetValidWordSource(words);
            var results = boggleTestable.GetFoundWords();
        }
    }
}
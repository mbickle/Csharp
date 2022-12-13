namespace BoggleWordFind
{
    public class Trie
    {
        private TrieNode root;

        /// <summary>Initializes a new instance of the <see cref="Trie" /> class.</summary>
        public Trie()
        {
            root = new TrieNode();
        }

        /// <summary>Inserts the specified word.</summary>
        /// <param name="word">The word.</param>
        public void Insert(string word)
        {
            var node = root;
            foreach (var c in word)
            {
                if (!node.ContainsKey(c))
                {
                    node.Add(c, new TrieNode());
                }
                node = node.Get(c);
            }

            node.SetComplete();
        }

        /// <summary>Searches the prefix.</summary>
        /// <param name="word">The word.</param>
        /// <returns>
        ///   TrieNode
        /// </returns>
        public TrieNode SearchPrefix(string word)
        {
            var node = root;
            foreach (var c in word)
            {
                if (node.ContainsKey(char.ToUpper(c)))
                {
                    node = node.Get(char.ToUpper(c));
                }
                else
                {
                    return null;
                }
            }

            return node;
        }

        /// <summary>Searches the specified word.</summary>
        /// <param name="word">The word.</param>
        /// <returns>
        ///   <c>true</c> if this found; otherwise, <c>false</c>.
        /// </returns>
        public bool Exists(string word)
        {
            var node = SearchPrefix(word);
            return node != null && node.IsComplete();
        }

        /// <summary>Startses the with.</summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        ///  <c>true</c> if this found; otherwise, <c>false</c>.
        /// </returns>
        public bool StartsWith(string prefix)
        {
            var node = SearchPrefix(prefix);
            return node != null;
        }
    }

    public class TrieNode
    {
        private Dictionary<char, TrieNode> children;
        private bool isComplete;
        private int count;

        /// <summary>Initializes a new instance of the <see cref="TrieNode" /> class.</summary>
        public TrieNode()
        {
            children = new Dictionary<char, TrieNode>();
        }

        /// <summary>Determines whether the specified c contains key.</summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if the specified c contains key; otherwise, <c>false</c>
        /// .</returns>
        public bool ContainsKey(char c)
        {
            return children.ContainsKey(c);
        }

        /// <summary>Gets the specified c.</summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   TrieNode
        /// </returns>
        public TrieNode Get(char c)
        {
            return children[c];
        }

        /// <summary>Adds the specified c.</summary>
        /// <param name="c">The c.</param>
        /// <param name="node">The node.</param>
        public void Add(char c, TrieNode node)
        {
            children[c] = node;
        }

        /// <summary>Sets the complete.</summary>
        public void SetComplete()
        {
            isComplete = true;
            ++count;
        }

        /// <summary>Determines whether this instance is complete.</summary>
        /// <returns>
        ///   <c>true</c> if this instance is complete; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComplete()
        {
            return isComplete;
        }

        /// <summary>Counts this instance.</summary>
        /// <returns>
        ///   count
        /// </returns>
        public int Count()
        {
            return count;
        }
    }
}

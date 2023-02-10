namespace BoggleWordFind
{
    public class Trie
    {
        private TrieNode root;

        /// <summary>Gets the root.</summary>
        /// <value>The root.</value>
        public TrieNode Root
        {
            get
            {
                return root;
            }
        }

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


}

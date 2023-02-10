namespace BoggleWordFind
{
    public class TrieNode
    {
        private Dictionary<char, TrieNode> children;
        private bool isComplete;
        private int count;

        /// <summary>Gets the children.</summary>
        /// <value>The children.</value>
        public Dictionary<char, TrieNode> Children
        {
            get
            {
                return children;
            }
        }

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

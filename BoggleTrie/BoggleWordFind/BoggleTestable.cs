using BoggleWordFind.Interfaces;

namespace BoggleWordFind
{
    public class BoggleTestable : IBoggleTestable
    {
        private Trie wordDictionary;
        private char[,] board;
        const int minWordLength = 3;

        /// <summary>Gets the word dictionary.</summary>
        /// <value>The word dictionary.</value>
        public Trie WordDictionary
        {
            get
            {
                return wordDictionary;
            }
        }

        /// <summary>Gets the board.</summary>
        /// <value>The board.</value>
        public char[,] Board
        {
            get
            {
                return board;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="BoggleTestable" /> class.</summary>
        public BoggleTestable()
        {
            wordDictionary = new Trie();
        }

        /// <summary>Sets the valid word source.</summary>
        /// <param name="wordSource">The word source.</param>
        public void SetValidWordSource(IEnumerable<string> wordSource)
        {
            if (wordSource == null)
            {
                throw new ArgumentNullException("wordSource cannot be null.");
            }

            foreach (var word in wordSource)
            {
                if (word.Length >= minWordLength)
                {
                    wordDictionary.Insert(word.ToUpper());
                }
            }
        }

        /// <summary>Sets the state of the boggle board.</summary>
        /// <remarks>Assume the board is square, contents are matches at least width</remarks>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="contents">The contents.</param>
        /// <exception cref="System.ArgumentException">Contents length should equal height.</exception>
        /// <exception cref="System.ArgumentException">Contents are not valid. Must be at least the size of board and contain letters.</exception>
        /// <exception cref="System.ArgumentNullException">Contents cannot be null.</exception>
        public void SetBoggleBoardState(int width, int height, char[][] contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("Contents cannot be null.");
            }

            // Contents should have as many rows as height and contain valid content
            if (!ValidateContents(contents, width, height))
            {
                throw new ArgumentException("Contents are not valid. Must be at least the size of board and contain letters.");
            }

            // Initialize Board
            board = new char[height, width];

            // Populate Board
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    board[i, j] = Char.ToUpper(contents[i][j]);
                }
            }
        }

        /// <summary>Gets the found words.</summary>
        /// <returns>Collection of found words.</returns>
        public IEnumerable<string> GetFoundWords()
        {
            if (Board == null)
            {
                throw new NullReferenceException("Board is null, hasn't been initialized yet.  Must run SetBoggleBoardState.");
            }

            var wordsFound = new List<string>();
            var rowsCount = board.GetLength(0);
            var columnsCount = board.GetLength(1);
            var visited = new bool[rowsCount, columnsCount];
            string word = string.Empty;

            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                for (int columIndex = 0; columIndex < columnsCount; columIndex++)
                {
                    SearchBoard(board, rowIndex, columIndex, 0, word, wordsFound, visited);
                }
            }

            return wordsFound;
        }

        /// <summary>Search board for the words.</summary>
        /// <param name="board">The board.</param>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="currentIndex">Current index.</param>
        /// <param name="word">The word to find.</param>
        /// <param name="wordsFound">Collection of words found.</param>
        /// <param name="visited">The visited row, column.</param>
        private void SearchBoard(char[,] board, int row, int column, int currentIndex, string word, List<string> wordsFound, bool[,] visited)
        {
            var rowsCount = board.GetLength(0);
            var columnsCount = board.GetLength(1);

            // Search if word is valid after we we ensure that min word length has been met
            if (currentIndex >= minWordLength && WordDictionary.Exists(word))
            {
                if (!wordsFound.Contains(word, StringComparer.InvariantCultureIgnoreCase))
                {
                    wordsFound.Add(word);
                }
            }

            // Outside of bounds, nothing left to search
            if (row < 0 || row >= rowsCount || column < 0 || column >= columnsCount)
            {
                return;
            }

            // If we've already visited, we can't use again, so return
            if (visited[row, column])
            {
                return;
            }

            // Mark visited
            visited[row, column] = true;
            word = word + board[row, column];

            // Search
            if (WordDictionary.StartsWith(word))
            {
                // East
                SearchBoard(board, row, column - 1, currentIndex + 1, word, wordsFound, visited);

                // West
                SearchBoard(board, row, column + 1, currentIndex + 1, word, wordsFound, visited);

                // North
                SearchBoard(board, row - 1, column, currentIndex + 1, word, wordsFound, visited);

                // South
                SearchBoard(board, row + 1, column, currentIndex + 1, word, wordsFound, visited);

                // North East
                SearchBoard(board, row - 1, column + 1, currentIndex + 1, word, wordsFound, visited);

                // North West
                SearchBoard(board, row - 1, column - 1, currentIndex + 1, word, wordsFound, visited);

                // South West
                SearchBoard(board, row + 1, column - 1, currentIndex + 1, word, wordsFound, visited);

                // South East
                SearchBoard(board, row + 1, column + 1, currentIndex + 1, word, wordsFound, visited);
            }
            else
            {
                word = string.Empty;
            }

            // Reset visited state after searching
            //word = string.Empty;
            visited[row, column] = false;
        }

        /// <summary>Validates the contents to ensure it meets requirements.</summary>
        /// <remarks>
        ///     Contents should contain at least as many characters per row as width.
        ///     Contenst should only contain Letters
        /// </remarks>
        /// <param name="contents">The contents.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///   true/false
        /// </returns>
        private bool ValidateContents(char[][] contents, int width, int height)
        {
            var valid = true;

            if (contents.Length != height)
            {
                valid = false;
            }
            else
            {
                for (int i = 0; i < height; i++)
                {
                    if (contents[i] == null || contents[i].Length < width)
                    {
                        valid = false;
                        break;
                    }

                    for (int j = 0; j < width; j++)
                    {
                        if (!char.IsLetter(contents[i][j]))
                        {
                            valid = false;
                            break;
                        }
                    }
                }
            }

            return valid;
        }
    }
}

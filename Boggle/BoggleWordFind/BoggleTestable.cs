using System;
using System.Collections.Concurrent;
using BoggleWordFind.Interfaces;

namespace BoggleWordFind
{
    public class BoggleTestable : IBoggleTestable
    {
        private Dictionary<char, List<string>> wordDictionary;
        private List<string> wordList;
        private char[,] board;
        const int minHeight = 3;
        const int minWidth = 3;
        const int minWordLength = 3;

        /// <summary>Gets the word dictionary.</summary>
        /// <value>The word dictionary.</value>
        public Dictionary<char, List<string>> WordDictionary 
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
        }

        /// <summary>Sets the valid word source.</summary>
        /// <param name="wordSource">The word source.</param>
        public void SetValidWordSource(IEnumerable<string> wordSource)
        {
            if (wordSource == null)
            {
                throw new ArgumentNullException("wordSource cannot be null.");
            }

            wordDictionary = new Dictionary<char, List<string>>();

            // Break up the words via starting char to split dictionary up
            foreach (var alpha in Enumerable.Range('a', 'z').Select(i => (char)i).ToArray())
            {
                // Look for words that start with char to add to dictionary.
                // Convert to ToLower to normalize.
                // Only get words that are at least minWordLength.
                var words = wordSource
                    .Where(w => w.ToLowerInvariant().StartsWith(alpha) && w.Trim().Length >= minWordLength)
                    .ToList()
                    .ConvertAll(w => w.ToLowerInvariant().Trim());

                // Only add if we find words.
                if (words.Any())
                {
                    wordDictionary.Add(alpha, words);
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
                    board[i, j] = contents[i][j];
                }
            }
        }

        /// <summary>Gets the found words.</summary>
        /// <returns>Collection of found words.</returns>
        public IEnumerable<string> GetFoundWords()
        {
            if (WordDictionary == null)
            {
                throw new NullReferenceException("Word Dictionary is null, hasn't been initialized yet.  Must run SetValidWordSource.");
            }

            if (Board == null)
            {
                throw new NullReferenceException("Board is null, hasn't been initialized yet.  Must run SetBoggleBoardState.");
            }


            var wordsFound = new List<string>();
            var rowsCount = board.GetLength(0);
            var columnsCount = board.GetLength(1);
            var visited = new bool[rowsCount, columnsCount];

            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                for (int columIndex = 0; columIndex < columnsCount; columIndex++)
                {
                    if (WordDictionary.ContainsKey(board[rowIndex, columIndex]))
                    {
                        var words = new List<string>();
                        WordDictionary.TryGetValue(board[rowIndex, columIndex], out words);
                        if (words != null)
                        {
                            foreach (var word in words)
                            {
                                SearchBoard(board, rowIndex, columIndex, 0, word, wordsFound, visited);
                            }
                        }
                    }
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

            // Search done
            if (currentIndex >= word.Length)
            {
                if (!wordsFound.Contains(word, StringComparer.InvariantCultureIgnoreCase))
                {
                    wordsFound.Add(word);
                }

                return;
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

            // If there isn't a match, return
            if (Char.ToLowerInvariant(board[row, column]).CompareTo(Char.ToLowerInvariant(word[currentIndex])) != 0)
            {
                return;
            }

            // Mark visited
            visited[row, column] = true;
            
            // Search
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

            // Reset visited state after searching
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

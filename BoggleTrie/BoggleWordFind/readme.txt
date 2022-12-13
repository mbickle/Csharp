This is an updated version of previous BoggleWordFind that uses a Trie for the Word Dictionary
Improves the speed of reading in the words
Does a search on words based on characters on board vs. the previous one was a bit more brute force
by taking each of the words in the dictionary and trying to see if they exist in the board.

BoggleWordFind --help 
    Will display parameters.

--contents 
    Must be a string of characters that is at least the length of the board. 
    String will be split up based on size of board.
    Example:  
        Board: 3x3
        String: "abcdefghi" will result in a board of the following:
            a b c
            d e f
            g h i

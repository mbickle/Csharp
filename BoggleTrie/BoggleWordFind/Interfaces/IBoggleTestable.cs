namespace BoggleWordFind.Interfaces
{
    public interface IBoggleTestable
    {
        void SetValidWordSource(IEnumerable<string> wordSource);
        void SetBoggleBoardState(int width, int height, char[][] contents);
        IEnumerable<string> GetFoundWords();
    }
}

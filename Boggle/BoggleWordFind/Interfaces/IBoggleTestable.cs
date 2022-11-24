namespace BoggleWordFind.Interfaces
{
    /// <summary>
    /// You may assume that SetValidWordSource and SetBoggleBOardState will be called before
    /// GetFoundWords is called.
    /// 
    /// The only requirement is that you have a concrete class that implements IBoggleTestable,
    /// and that class can be instantiated with a default constructor.
    /// </summary>
    public interface IBoggleTestable
    {
        void SetValidWordSource(IEnumerable<string> wordSource);
        void SetBoggleBoardState(int width, int height, char[][] contents);
        IEnumerable<string> GetFoundWords();
    }
}

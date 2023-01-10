namespace TwitterStream.Interfaces
{
    public interface ITweetStore
    {
        void Add(ITweet tweet);
        List<string> GetTopHashtags(int count);
        int GetCount();
    }
}

namespace TwitterStream.Interfaces
{
    public interface ITweetModel
    {
        long TweetId { get; set; }
        string Content { get; set; }
    }
}

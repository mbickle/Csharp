namespace TwitterStream.Interfaces
{
    public interface ITweet
    {
        long Id { get; set; }
        string Content { get; set; }
    }
}

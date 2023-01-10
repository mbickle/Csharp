namespace TwitterStream.Interfaces
{
    public interface ITweet
    {
        string Id { get; set; }
        string Content { get; set; }
        string Hashtags { get; set; }
    }
}

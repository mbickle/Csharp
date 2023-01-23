namespace TwitterStream.Interfaces
{
    public interface ITwitterStreamService
    {
        Task Start(CancellationTokenSource cancelToken);
        Task Process(ITweet tweet);
        List<string> GetValidHashtags(string input);
        ITweet DeserializeTweet(string input);
    }
}

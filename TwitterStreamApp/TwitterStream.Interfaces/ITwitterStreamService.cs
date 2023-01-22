namespace TwitterStream.Interfaces
{
    public interface ITwitterStreamService
    {
        Task Start(CancellationTokenSource cancelToken);
        Task Process(ITweetModel tweet);
        List<string> GetValidHashtags(string input);
    }
}

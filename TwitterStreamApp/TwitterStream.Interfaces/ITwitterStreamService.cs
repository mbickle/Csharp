namespace TwitterStream.Interfaces
{
    public interface ITwitterStreamService
    {
        Task Start(CancellationTokenSource cancelToken);
        void Process(ITweetModel tweet);
        List<string> GetValidHashtags(string input);
    }
}

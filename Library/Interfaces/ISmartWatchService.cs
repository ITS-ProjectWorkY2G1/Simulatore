namespace Services.Interfaces
{
    public interface ISmartWatchService
    {
        Task CreateSessionAsync(Guid WatchId, Guid UserId, TimeSpan SessionTime, CancellationToken cancellationToken);
    }
}
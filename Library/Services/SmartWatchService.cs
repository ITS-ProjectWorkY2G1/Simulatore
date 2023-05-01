using Microsoft.EntityFrameworkCore;
using Services.Intefaces;

namespace Services.Services
{
    public class SmartWatchService : ISmartWatchService
    {
        private readonly DbContext _context;
        public SmartWatchService(DbContext context)
        {
            _context = context;
        }
        public async Task CreateSessionAsync(Guid WatchId, Guid UserId, TimeSpan SessionTime, CancellationToken cancellationToken)
        {
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                SessionTime = -new TimeSpan(0, 0, 10);

                //do work
                //_context

                if (SessionTime.TotalSeconds <= 0) break;
            }
        }
    }
}

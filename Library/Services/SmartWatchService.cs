using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Services.Intefaces;

namespace Services.Services
{
    public class SmartWatchService : ISmartWatchService
    {
        private readonly ILogger<SmartWatchService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SmartWatchService(ILogger<SmartWatchService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task CreateSessionAsync(Guid WatchId, Guid UserId, TimeSpan SessionTime, CancellationToken cancellationToken)
        {
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
            WatchContext context = new WatchContext();
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                context = scope.ServiceProvider.GetRequiredService<WatchContext>();
            }
            Random rnd = new Random();

            int latitude = rnd.Next(-89, 91);
            int longitude = rnd.Next(-179, 181);

            Smartwatch smartwatch = new Smartwatch { UserId = UserId, Id = WatchId, SessionId = Guid.NewGuid(), HeartRate = rnd.Next(59, 101), Position = $"{latitude}-{longitude}" };
            Session session = new Session { SessionId = smartwatch.SessionId };

            Coordinate positionFinal = new Coordinate { Latitude = rnd.Next(latitude - 2, latitude + 2), Longitude = rnd.Next(longitude - 2, longitude + 2) };

            Coordinate newPosition = new Coordinate { Latitude = latitude, Longitude = longitude };

            bool direction = true;

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                SessionTime -= new TimeSpan(0, 0, 10);

                smartwatch.Timestamp = DateTime.Now;
                smartwatch.HeartRate = smartwatch.HeartRate + rnd.Next(-5, +5);

                //preso coordinate random per nuova posizione nella piscina ma senza calcolare se segue la linea della corsia
                if (direction)
                {
                    newPosition = new Coordinate { Latitude = newPosition.Latitude + rnd.NextDouble(), Longitude = newPosition.Longitude + rnd.NextDouble() };
                    if (newPosition.Latitude > positionFinal.Latitude || newPosition.Longitude > positionFinal.Longitude) direction = false;
                }
                else
                {
                    newPosition = new Coordinate { Latitude = newPosition.Latitude - rnd.NextDouble(), Longitude = newPosition.Longitude - rnd.NextDouble() };
                    if (newPosition.Latitude < positionFinal.Latitude || newPosition.Longitude < positionFinal.Longitude) direction = true;
                }

                smartwatch.Position = $"{newPosition.Latitude}-{newPosition.Longitude}";

                await context.AddAsync(smartwatch);
                await context.SaveChangesAsync();

                if (SessionTime.TotalSeconds <= 0) break;
            }
        }
    }
}

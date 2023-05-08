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
            IServiceScope scope = _serviceProvider.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<WatchContext>();

            Random rnd = new Random();

            int latitude = rnd.Next(-89, 91);
            int longitude = rnd.Next(-179, 181);

            Smartwatch smartwatch = new Smartwatch { UserId = UserId, Id = WatchId, SessionId = Guid.NewGuid(), HeartRate = rnd.Next(59, 101), Position = $"{latitude}-{longitude}" };
            Session session = new Session { SessionId = smartwatch.SessionId, SessionTime = SessionTime };

            Coordinate positionFinal = new Coordinate { Latitude = rnd.Next(latitude - 2, latitude + 2), Longitude = rnd.Next(longitude - 2, longitude + 2) };

            Coordinate newPosition = new Coordinate { Latitude = latitude, Longitude = longitude };

            bool direction = true;
            List<int> hearthRates = new List<int>();
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                SessionTime -= new TimeSpan(0, 0, 10);

                smartwatch.Timestamp = DateTime.UtcNow;
                smartwatch.HeartRate = smartwatch.HeartRate + rnd.Next(-5, +5);
                hearthRates.Add(smartwatch.HeartRate);
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
                try
                {
                    await context.AddAsync(smartwatch);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                if (SessionTime.TotalSeconds <= 0) break;
            }
            //Save session informations after api finished
            Random rand = new Random();
            session.PoolLength = (short)rand.Next(4, 51);
            session.PoolLaps = (short)rand.Next(2, 5);
            session.AvgHeartRate = Convert.ToInt32(hearthRates.Average());
            session.SessionDistance = rand.Next(5, 100);
            try
            {
                await context.AddAsync(session);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

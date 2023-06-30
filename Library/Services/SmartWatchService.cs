using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            JsonConverter serializer;
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
            WatchContext context = new WatchContext();
            IServiceScope scope = _serviceProvider.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<WatchContext>();
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=cperpwg1;AccountKey=hCqHYjtF4dJdt13i/Ft/w6gAxwINDHdLN7rpeDbh+fdpK5Dhsusm5ppvQpec5Ff/3vtkiCY7lvJH+AStQu6VCg==;EndpointSuffix=core.windows.net";

            QueueClient queueClientSmartWatch = new QueueClient(connectionString, "earth4sport");
            QueueClient queueClientSessions = new QueueClient(connectionString, "earth4sport-sessions");
            queueClientSmartWatch.CreateIfNotExists();
            queueClientSessions.CreateIfNotExists();


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

                //Send to queue smartwatch data
                if (queueClientSmartWatch.Exists())
                {
                    Console.WriteLine($"smartwatch data sent in queue: '{queueClientSmartWatch.Name}'");
                    await queueClientSmartWatch.SendMessageAsync(JsonSerializer.Serialize(smartwatch));
                }
                else
                {
                    Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                }
            }

            if (SessionTime.TotalSeconds <= 0)
            {
                Console.WriteLine("Session too short");
                return;
            }

            Random rand = new Random();
            session.PoolLength = (short)rand.Next(4, 51);
            session.PoolLaps = (short)rand.Next(2, 5);
            session.AvgHeartRate = Convert.ToInt32(hearthRates.Average());
            session.SessionDistance = rand.Next(5, 100);

            if (queueClientSessions.Exists())
            {
                Console.WriteLine($"Session data sent in queue: '{queueClientSessions.Name}'");
                await queueClientSessions.SendMessageAsync(JsonSerializer.Serialize(session));
            }
        }
    }
}

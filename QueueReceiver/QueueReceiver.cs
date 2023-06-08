using System;
using System.Text.Json;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Models;
using Microsoft.Extensions.DependencyInjection;

namespace QueueReceiver
{
    public class QueueReceiver
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public QueueReceiver(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _logger = loggerFactory.CreateLogger<QueueReceiver>();
            _serviceProvider = serviceProvider;
        }

        public async Task LoadDataAsync(Smartwatch data)
        {
            WatchContext context = new();
            IServiceScope scope = _serviceProvider.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<WatchContext>();

            try
            {
                await context.AddAsync(data);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        [Function("QueueReceiver")]
        public void Run([QueueTrigger("earth4sport", Connection = "queue")] string myQueueItem)
        {
            //string jsonstring = Encoding.Unicode.GetString(myQueueItem);

            Smartwatch QueueItem = JsonSerializer.Deserialize<Smartwatch>(myQueueItem);

            QueueItem.Timestamp = DateTime.UtcNow;

            _logger.LogInformation($"C# Queue trigger function processed: {QueueItem.Id}");

            LoadDataAsync(QueueItem).Wait();
        }
    }
}

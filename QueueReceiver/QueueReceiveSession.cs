using System;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;

namespace QueueReceiveSession
{
    public class QueueReceiveSession
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public QueueReceiveSession(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _logger = loggerFactory.CreateLogger<QueueReceiveSession>();
            _serviceProvider = serviceProvider;
        }

        public async Task LoadDataAsync(Session data)
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

        [Function("QueueReceiveSession")]
        public void Run([QueueTrigger("earth4sport", Connection = "queue")] byte[] myQueueItem)
        {
            string jsonstring = Encoding.Unicode.GetString(myQueueItem);

            Session QueueItem = JsonSerializer.Deserialize<Session>(jsonstring);

            LoadDataAsync(QueueItem).Wait();
        }
    }
}

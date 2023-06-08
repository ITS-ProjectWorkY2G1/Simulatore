using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace QueueReceiver
{
    public class QueueReceiveSession
    {
        private readonly ILogger _logger;

        public QueueReceiveSession(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueReceiveSession>();
        }

        [Function("QueueReceiveSession")]
        public void Run([QueueTrigger("earth4sport-sessions", Connection = "queue")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}

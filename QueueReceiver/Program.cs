using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;

var host = new HostBuilder()
    .ConfigureServices(builder =>
    {
        builder.AddDbContext<WatchContext>(opt=>
        {
            opt.UseNpgsql(Environment.GetEnvironmentVariable("PgConnString"));
        });
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();

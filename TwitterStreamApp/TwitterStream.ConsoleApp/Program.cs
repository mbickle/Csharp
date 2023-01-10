using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TwitterStream.Interfaces;
using TwitterStream.Reporting;

namespace TwitterStream.ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Set Console to show unicode characters.
            Console.OutputEncoding = Encoding.Unicode;

            Log.Logger.Information("Starting Application...");

            var host = Setup.Startup();
            var service = host.Services.GetService<ITwitterStreamService>();
            var report = host.Services.GetService<ConsoleReport>();

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                var token = source.Token;

                var tasks = new List<Task>();
                tasks.Add(Task.Run(async () => await service.Start(source)));
                tasks.Add(Task.Run(async () => await report.Report(source.Token)));

                await Task.WhenAll(tasks);
            }

            Log.Logger.Information("Exiting Application...");
        }
    }
}
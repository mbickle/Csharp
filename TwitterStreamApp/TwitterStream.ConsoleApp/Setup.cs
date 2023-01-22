using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TwitterStream.Core;
using TwitterStream.Data;
using TwitterStream.Interfaces;
using TwitterStream.Reporting;
using TwitterStream.Service;

namespace TwitterStream.ConsoleApp
{
    internal class Setup
    {
        /// <summary>Builds the configuration.</summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        internal static void BuildConfig(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        /// <summary>Registers the custom services.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        internal static void RegisterCustomServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            var twitterStreamAppConfiguration = new TwitterStreamAppConfiguration(configuration);

            // Initialize httpclient
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", twitterStreamAppConfiguration.BearerToken);

            services.AddDbContext<TwitterStreamDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "TwitterStream");
            });

            // Singleton
            services.AddSingleton<HttpClient>(httpClient);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<ITwitterStreamAppConfiguration>(twitterStreamAppConfiguration);
            services.AddSingleton<ITweetStore, TwitterStream.Service.Store>();

            // Transient
            services.AddTransient<ITwitterStreamService, StreamService>();
            services.AddTransient<ConsoleReport>();
        }

        /// <summary>Startups this instance.</summary>
        internal static IHost Startup()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()                
                .CreateLogger();

            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    RegisterCustomServices(services, configuration);
                })
                .UseSerilog()
                .Build();
        }
    }
}

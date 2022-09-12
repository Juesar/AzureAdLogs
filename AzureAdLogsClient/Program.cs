using System;
using System.IO;
using System.Threading.Tasks;
using AzureAdLogsClient.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureAdLogsClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var logsClient = host.Services.GetService<ILogsClient>();

            var data = await logsClient.GetAuditLogs();

            Console.ReadLine();
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ILogsClient, LogsClient>();
                });

            return hostBuilder;
        }
    }
}
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
            var csvHelper = host.Services.GetService<ICsvHelper>();
            var fileHelper = host.Services.GetService<IFileHelper>();

            var data = await logsClient.GetAuditLogs();
            var csvText = csvHelper.ObjectToCvsText(data);
            fileHelper.StoreFile(@"D:\test.csv", csvText);

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
                    services.AddTransient<ICsvHelper, CsvHelper>();
                    services.AddTransient<IFileHelper, FileHelper>();
                });

            return hostBuilder;
        }
    }
}
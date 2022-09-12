using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

namespace AzureAdLogsClient.Core
{
    public interface ILogsClient
    {
        Task<IAuditLogRootSignInsCollectionPage?> GetAuditLogs();
    }

    public class LogsClient: ILogsClient
    {
        private GraphServiceClient _graphClient;
        
        public LogsClient(IConfiguration configuration)
        {
            // application should have this api permissions in portal "AuditLog.Read.All", "Directory.Read.All"
            var scopes = new[] {"https://graph.microsoft.com/.default"};
            
            var tenantId =  configuration["TenantId"];
            var clientId = configuration["ClientId"];
            var clientSecret = configuration["ClientSecret"];
            
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            
            var deviceCodeCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            _graphClient = new GraphServiceClient(deviceCodeCredential, scopes);
        }

        public async Task<IAuditLogRootSignInsCollectionPage?> GetAuditLogs()
        {
            var lastDay = new Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var auditLogs = await _graphClient.AuditLogs.SignIns
                .Request()
                .Filter($"createdDateTime gt {lastDay}") // filter date
                .GetAsync();
            return auditLogs;
        }
    }
}
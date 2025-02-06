using Azure.Core.Extensions;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;

namespace SolarPanel_Api
{
    internal static class AzureClientFactoryBuilderExtensions
    {

        public static IAzureClientBuilder<TableServiceClient, TableClientOptions> 
            AddTableServiceClient(this AzureClientFactoryBuilder builder,
            string serviceUriOrConnectionString, bool preferMsi)

        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString,
                UriKind.Absolute, out Uri? serviceUri))
            {
                return builder.AddTableServiceClient(serviceUri);
            }
            else
            {
                return builder.AddTableServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}

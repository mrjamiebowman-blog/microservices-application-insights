using Azure.Messaging.ServiceBus;

namespace MrJB.MS.Common.Extensions
{
    public static class AzureServiceBusMessageExtension
    {
        public static (string rootOperationId, string parentId) GetCorrelationIds(this ServiceBusReceivedMessage message)
        {
            string rootOperationId = string.Empty;
            if (message.ApplicationProperties.TryGetValue("OperationId", out var objRootOperationId))
            {
                rootOperationId = objRootOperationId.ToString();
            }

            string parentId = string.Empty;
            if (message.ApplicationProperties.TryGetValue("", out var objParentId))
            {
                parentId = objParentId.ToString();
            }

            return (rootOperationId, parentId);
        }

        public static void SetCorrelationIds(this ServiceBusMessage message, string rootOperationId, string parentId)
        {
            message.ApplicationProperties.Add("OperationId", rootOperationId);
            message.ApplicationProperties.Add("ParentId", parentId);
        }
    }
}

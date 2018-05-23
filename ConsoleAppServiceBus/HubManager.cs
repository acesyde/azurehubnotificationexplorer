using AutoMapper;
using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleAppServiceBus
{
    public class HubManager
    {
        private readonly NotificationHubClient hub;

        public HubManager(string connectionString, string hubPath)
        {
            hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubPath);
        }

        public async Task<List<Registration>> GetRegistrationsAsync()
        {
            var allRegistrations = new List<RegistrationDescription>();
            CollectionQueryResult<RegistrationDescription> page = null;
            do
            {
                page = await hub.GetAllRegistrationsAsync(page?.ContinuationToken, 0);
                allRegistrations.AddRange(page);
            }
            while (!string.IsNullOrWhiteSpace(page.ContinuationToken));

            return Mapper.Map<List<Registration>>(allRegistrations);
        }
    }
}

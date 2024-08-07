using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CosmosDbSchemaApi.Services
{

    public class CosmosService
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosService(string accountEndpoint, string accountKey)
        {
            _cosmosClient = new CosmosClient(accountEndpoint, accountKey);
        }

        public async Task<List<string>> GetContainersAsync(string databaseName)
        {
            var database = _cosmosClient.GetDatabase(databaseName);
            var containerIterator = database.GetContainerQueryIterator<ContainerProperties>();
            var containers = new List<string>();

            while (containerIterator.HasMoreResults)
            {
                var response = await containerIterator.ReadNextAsync();
                containers.AddRange(response.Select(c => c.Id));
            }

            return containers;
        }

        public async Task<Dictionary<string, List<Dictionary<string, object>>>> GetContainersWithDataAsync(string databaseName)
        {
            var containersData = new Dictionary<string, List<Dictionary<string, object>>>();
            var containerNames = await GetContainersAsync(databaseName);

            foreach (var containerName in containerNames)
            {
                var container = _cosmosClient.GetContainer(databaseName, containerName);
                var items = new List<Dictionary<string, object>>();

                var query = new QueryDefinition("SELECT * FROM c");
                var iterator = container.GetItemQueryIterator<Dictionary<string, object>>(query);

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    items.AddRange(response.ToList());
                }

                containersData.Add(containerName, items);
            }

            return containersData;
        }
    }

}

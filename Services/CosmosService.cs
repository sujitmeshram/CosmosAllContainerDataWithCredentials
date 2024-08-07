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

        public async Task<Dictionary<string, ContainerData>> GetContainersWithDataAndSchemaAsync(string databaseName)
        {
            var containersData = new Dictionary<string, ContainerData>();
            var containerNames = await GetContainersAsync(databaseName);

            foreach (var containerName in containerNames)
            {
                var container = _cosmosClient.GetContainer(databaseName, containerName);
                var items = new List<Dictionary<string, object>>();
                var schema = new Dictionary<string, string>();

                var query = new QueryDefinition("SELECT * FROM c");
                var iterator = container.GetItemQueryIterator<Dictionary<string, object>>(query);

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    items.AddRange(response.ToList());

                    if (schema.Count == 0 && items.Count > 0)
                    {
                        foreach (var key in items[0].Keys)
                        {
                            schema[key] = items[0][key]?.GetType().Name ?? "Unknown";
                        }
                    }
                }

                containersData.Add(containerName, new ContainerData
                {
                    Schema = schema,
                    Data = items
                });
            }

            return containersData;
        }
    }

    public class ContainerData
    {
        public Dictionary<string, string> Schema { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
    }
}
using CosmosDbSchemaApi.Models;
using CosmosDbSchemaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CosmosDbSchemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CosmosController : ControllerBase
    {
        [HttpPost("getContainersWithDataAndSchema")]
        public async Task<IActionResult> GetContainersWithDataAndSchema([FromBody] CosmosCredentials credentials)
        {
            if (credentials == null ||
                string.IsNullOrWhiteSpace(credentials.AccountEndpoint) ||
                string.IsNullOrWhiteSpace(credentials.AccountKey) ||
                string.IsNullOrWhiteSpace(credentials.DatabaseName))
            {
                return BadRequest("Invalid credentials.");
            }

            var cosmosService = new CosmosService(credentials.AccountEndpoint, credentials.AccountKey);
            var containersData = await cosmosService.GetContainersWithDataAndSchemaAsync(credentials.DatabaseName);

            // Define the path to the file where data will be stored in the current working directory
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CosmosData.json");

            // Serialize the containers data to JSON
            var json = JsonConvert.SerializeObject(containersData, Formatting.Indented);

            // Write the JSON to the file
            await System.IO.File.WriteAllTextAsync(filePath, json);

            // Return a success message along with the file path and the JSON data
            return Ok(new
            {
                Message = "Data has been written to file.",
                FilePath = filePath,
                ContainersData = containersData // Include the actual data directly in the response
            });
        }
    
}
}

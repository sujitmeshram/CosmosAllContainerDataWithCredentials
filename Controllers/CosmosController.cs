using CosmosDbSchemaApi.Models;
using CosmosDbSchemaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace CosmosDbSchemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CosmosController : ControllerBase
    {
        [HttpPost("getContainersWithData")]
        public async Task<IActionResult> GetContainersWithData([FromBody] CosmosCredentials credentials)
        {
            if (credentials == null ||
                string.IsNullOrWhiteSpace(credentials.AccountEndpoint) ||
                string.IsNullOrWhiteSpace(credentials.AccountKey) ||
                string.IsNullOrWhiteSpace(credentials.DatabaseName))
            {
                return BadRequest("Invalid credentials.");
            }

            var cosmosService = new CosmosService(credentials.AccountEndpoint, credentials.AccountKey);
            var containersData = await cosmosService.GetContainersWithDataAsync(credentials.DatabaseName);

            return Ok(containersData);
        }
    }
}

using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.Identity.Web.Resource;
using ProgrammezAzureSecurity.Server.Models;
using ProgrammezAzureSecurity.Shared;

namespace ProgrammezAzureSecurity.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class MagazinesController : ControllerBase
{
    private readonly ILogger<MagazinesController> _logger;
    private readonly IOptions<CosmosDbConfigurationOptions> _cosmosDbConfigurationOptions;

    public MagazinesController(ILogger<MagazinesController> logger, IOptions<CosmosDbConfigurationOptions> cosmosDbConfigurationOptions)
    {
        _logger = logger;
        _cosmosDbConfigurationOptions = cosmosDbConfigurationOptions;
    }

    [HttpGet]
    public async Task<IEnumerable<Magazine>> Get()
    {

        var (account, key, databaseName, containerName, partitionKey) = _cosmosDbConfigurationOptions.Value;

#if DEBUG
        using var cosmosClient = new CosmosClient(account,key);
#else
    using var cosmosClient = new CosmosClient(account,
            new DefaultAzureCredential());
#endif


        var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKey);

        var container = cosmosClient.GetContainer(databaseName, containerName);
        
        var query = container.GetItemQueryIterator<Magazine>();
        var result = new List<Magazine>();
        while (query.HasMoreResults)
        {
            var current = await query.ReadNextAsync();
            result.AddRange(current.Resource);
        }

        return result;

    }
}

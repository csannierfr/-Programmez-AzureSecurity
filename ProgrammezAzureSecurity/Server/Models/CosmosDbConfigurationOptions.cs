namespace ProgrammezAzureSecurity.Server.Models;

public class CosmosDbConfigurationOptions
{
    public string Account { get; set; }
    public string Key { get; set; }
    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }

    public string PartitionKey { get; set; }

    public void Deconstruct(out string account, out string key, out string databaseName, out string containerName, out string partitionKey)
    {
        account = Account;
        key = Key;
        databaseName = DatabaseName;
        containerName = ContainerName;
        partitionKey = PartitionKey;
    }
}
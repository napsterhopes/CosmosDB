using Microsoft.Azure.Cosmos;
using System.Configuration;
using System.Diagnostics;

public sealed class Program
{
    private static readonly string DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
    private static readonly string DataCollectionName = ConfigurationManager.AppSettings["CollectionName"];
    private static readonly string _endpointUri = ConfigurationManager.AppSettings["EndPointUrl"];
    private static readonly string _primaryKey = ConfigurationManager.AppSettings["AuthorizationKey"];

    public static async Task Main()
    {


        using CosmosClient client = new CosmosClient(_endpointUri, _primaryKey);
        DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);
        Database targetDatabase = databaseResponse.Database;


        ContainerProperties containerProperties = new()
        {
            Id = DataCollectionName,
            PartitionKeyPath = "/dvdPartitionKey",
            UniqueKeyPolicy =  new()
        };
        Database database = client.GetDatabase(DatabaseName);

        Container container = await database.CreateContainerIfNotExistsAsync(
                    containerProperties, ThroughputProperties.CreateManualThroughput(400)).ConfigureAwait(false);

        Console.ReadKey();
    }

    #region TelemetryClass
    internal class Telemetry
    {
        public string id { get; set; }
        public string tenantSiteId { get; set; }
        public string deviceId { get; set; }
        public long ts { get; set; }
    } 
    #endregion
}



////// List of partition keys, in hierarchical order. You can have up to three levels of keys.
//List<string> subpartitionKeyPaths = new List<string> { "/tenantSiteId", "/enqueuedTime" };
////string subpartitionKeyPaths = "/tenantid/siteid/deviceId";
//// Get reference to database that container will be created in
//Database database = client.GetDatabase(DatabaseName);
////Microsoft.Azure.Cosmos.Container container = database.GetContainer(DataCollectionName);
////// Create container - Subpartitioned by deviceId -> date
//ContainerProperties containerProperties = new ContainerProperties(id: DataCollectionName, partitionKeyPaths: subpartitionKeyPaths);
//Microsoft.Azure.Cosmos.Container container = await database.CreateContainerIfNotExistsAsync(containerProperties, throughput: 400);
////return newcontainer.StatusCode == System.Net.HttpStatusCode.Created;

//#region Create new item
////Telemetry item = new()
////{
////    id = Guid.NewGuid().ToString(),
////    tenantSiteId = "81b4681d-137b-4738-96a2-08d962858d1c" + "$" + "US-1",
////    deviceId = "Giles-7163712301905",
////    ts = DateTimeOffset.Now.ToUnixTimeSeconds() // to epoch
////};

//// Pass in the object and the SDK will automatically extract the full partition key path
////ItemResponse<Telemetry> createResponse = await container.CreateItemAsync(item);
//#endregion


//// Get the full partition key path
////var id = "e171fb40-e6f1-44be-a09b-1ba6261b0ee0";
////var fullPartitionkeyPath = new PartitionKeyBuilder()
////        .Add("TypeA-999973842-T") //deviceid
////        .Add("12072022") //date
////        .Build();
////var itemResponse = await container.ReadItemAsync<dynamic>(id, fullPartitionkeyPath);

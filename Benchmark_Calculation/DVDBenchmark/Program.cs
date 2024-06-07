namespace DocumentDBBenchmark
{
    #region References
    using DVDBenchmark;
    using DVDBenchmark.Model;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Container = Microsoft.Azure.Cosmos.Container;
    #endregion

    /// <summary>
    /// This sample demonstrates how to achieve high performance writes by hierarchial partitioning.
    /// </summary>
    public sealed class Program
    {
        #region Startup Fields
        private static readonly string DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
        private static readonly string DataCollectionName = ConfigurationManager.AppSettings["CollectionName"];
        private static readonly int CollectionThroughput = int.Parse(ConfigurationManager.AppSettings["CollectionThroughput"]);
        private static readonly string endpoint = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string authKey = ConfigurationManager.AppSettings["AuthorizationKey"];

        private static readonly ConnectionPolicy ConnectionPolicy = new ConnectionPolicy
        {
            ConnectionMode = Microsoft.Azure.Documents.Client.ConnectionMode.Direct,
            ConnectionProtocol = Protocol.Tcp,
            RequestTimeout = new TimeSpan(1, 0, 0),
            MaxConnectionLimit = 1000,
            RetryOptions = new RetryOptions
            {
                MaxRetryAttemptsOnThrottledRequests = 10,
                MaxRetryWaitTimeInSeconds = 60
            }
        };

        private static readonly string InstanceId = Dns.GetHostEntry("LocalHost").HostName + Process.GetCurrentProcess().Id;
        private const int MinThreadPoolSize = 100;

        private int pendingTaskCount;
        private long documentsInserted;
        private ConcurrentDictionary<int, double> requestUnitsConsumed = new ConcurrentDictionary<int, double>();
        private DocumentClient client;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        private Program(DocumentClient client)
        {
            this.client = client;
        }
        #endregion

        /// <summary>
        /// Main method for the sample.
        /// </summary>
        /// <param name="args">command line arguments.</param>
        public static void Main(string[] args)
        {
            //PointReadARecordBasedOnDeviceId();

            ThreadPool.SetMinThreads(MinThreadPoolSize, MinThreadPoolSize);

            Console.WriteLine("Summary:");
            Console.WriteLine("--------------------------------------------------------------------- ");
            Console.WriteLine("Endpoint: {0}", endpoint);
            Console.WriteLine("Collection : {0}.{1} at {2} request units per second", DatabaseName, DataCollectionName, ConfigurationManager.AppSettings["CollectionThroughput"]);
            Console.WriteLine("Document Template*: {0}", ConfigurationManager.AppSettings["DocumentTemplateFile"]);
            Console.WriteLine("Degree of parallelism*: {0}", ConfigurationManager.AppSettings["DegreeOfParallelism"]);
            Console.WriteLine("--------------------------------------------------------------------- ");
            Console.WriteLine();

            try
            {
                Console.WriteLine("DVDWriteBenchmark starting...");
                short tenantId = 0;

                WriteDocumentsForATenant(tenantId).Wait();
            }

#if !DEBUG
            catch (Exception e)
            {
                // If the Exception is a DocumentClientException, the "StatusCode" value might help identity 
                // the source of the problem. 
                Console.WriteLine("Samples failed with exception:{0}", e);
            }
#endif

            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }

        private static async void PointReadARecordBasedOnDeviceId()
        {
            //// Define a single-partition query that specifies the full partition key path
            //QueryDefinition query = new QueryDefinition(
            //    "SELECT COUNT(1) FROM c WHERE c.deviceId = @deviceId")
            //    .WithParameter("@deviceId", "Fri-Jado-100073842-T");

            //// Retrieve an iterator for the result set
            //using FeedIterator<RotisserieTelemetryModel> results = container.GetItemQueryIterator<RotisserieTelemetryModel>(query);

            //while (results.HasMoreResults)
            //{
            //    try
            //    {
            //        Microsoft.Azure.Cosmos.FeedResponse<RotisserieTelemetryModel> resultsPage = await results.ReadNextAsync();

            //        Console.ReadKey();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.InnerException);
            //    }
            //}
        }

        #region WriteDocumentsForATenant
        private async static Task WriteDocumentsForATenant(short tenantId)
        {
            //DocumentCollection dataCollection = GetCollectionIfExists(DatabaseName, DataCollectionName);
            //int currentCollectionThroughput = 0;
            //if (bool.Parse(ConfigurationManager.AppSettings["ShouldCleanupOnStart"])) || dataCollection == null)
            //{
            using CosmosClient client = new CosmosClient(endpoint, authKey);
            //Task<DatabaseResponse> databdase = GetDatabaseIfExists(DatabaseName);
            //if (database != null)
            //{
            //    await database.DeleteDatabaseAsync(database.SelfLink);
            //}

            //if (database != null)
            //{
            //    await database.DeleteAsync();
            //}

            //if (databdase.Status == TaskStatus.Created)
            //{
            //    return null;
            //}

            Console.WriteLine("Creating database {0}", DatabaseName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);
            Microsoft.Azure.Cosmos.Database targetDatabase = databaseResponse.Database;

            //database = await client.CreateDatabaseAsync(new Microsoft.Azure.Cosmos.Database { Id = DatabaseName });
            Microsoft.Azure.Cosmos.Database database = client.GetDatabase(DatabaseName);

            Console.WriteLine("Creating collection {0} with {1} RU/s", DataCollectionName, CollectionThroughput);
            //dataCollection = await this.CreatePartitionedCollectionAsync(DatabaseName, DataCollectionName);
            await CreateHierarchialPartitionedCollectionAsync(database, DatabaseName, DataCollectionName);
            //currentCollectionThroughput = CollectionThroughput;
            //}
            //else
            //{
            //    OfferV2 offer = (OfferV2)client.CreateOfferQuery().Where(o => o.ResourceLink == dataCollection.SelfLink).AsEnumerable().FirstOrDefault();
            //    currentCollectionThroughput = offer.Content.OfferThroughput;

            //    Console.WriteLine("Found collection {0} with {1} RU/s", DataCollectionName, currentCollectionThroughput);
            //}

            var siteTasks = new List<Task>();
            for (short siteIndex = 1; siteIndex <= 100; siteIndex++)
            {
                Console.WriteLine("------------------------Starting for site: {0}------------------------", siteIndex);
                using (var clients = new DocumentClient(
                    new Uri(endpoint),
                    authKey,
                    ConnectionPolicy))
                {
                    var program = new Program(clients);
                    //program.RunAsync(siteIndex).Wait();
                    siteTasks.Add(program.WriteDocsForASite(siteIndex));
                    Console.WriteLine("--------------------Completed for site: {0}---------------------------", siteIndex);
                }
            }
            await Task.WhenAll(siteTasks);
        }
        #endregion

        #region WriteDocsForASite
        /// <summary>
        /// Writes documents for a site
        /// </summary>
        /// <returns>a Task object.</returns>
        private async Task WriteDocsForASite(short siteIndex)
        {
            int taskCount;
            int degreeOfParallelism = int.Parse(ConfigurationManager.AppSettings["DegreeOfParallelism"]);

            if (degreeOfParallelism == -1)
            {
                // set TaskCount = 10 for each 10k RUs, minimum 1, maximum 250 // prod
                // for deli-telimetry , default throughput is 400 so 400/1000 = 0.4 , taskCount = 1
                // task will parallelise only when throughput is more than 1000
                // one task per thousand
                // here let's consider maxx while reading it
                taskCount = Math.Max(CollectionThroughput / 1000, 1);
                taskCount = Math.Min(taskCount, 250);
            }
            else
            {
                taskCount = degreeOfParallelism;
            }

            Console.WriteLine("Starting Inserts with {0} tasks", taskCount);


            pendingTaskCount = taskCount;
            var tasks = new List<Task>();
            tasks.Add(this.LogOutputStats());
            //Enable this if you want to read from file
            //File.ReadAllText(ConfigurationManager.AppSettings["Device1"]);
            long numberOfDocumentsToInsert = long.Parse(ConfigurationManager.AppSettings["NumberOfDocumentsToInsert"]) / taskCount;

            #region Insert document for one site having 5 devices
            for (var i = 0; i < taskCount; i++)
            {
                tasks.Add(this.InsertDocument(i, siteIndex, client, numberOfDocumentsToInsert));
            }
            #endregion

            await Task.WhenAll(tasks);

            if (bool.Parse(ConfigurationManager.AppSettings["ShouldCleanupOnFinish"]))
            {
                Console.WriteLine("Deleting Database {0}", DatabaseName);
                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseName));
            }
        }
        #endregion

        #region InsertDocument
        private async Task InsertDocument(int taskId, short siteIndex, DocumentClient clients, long numberOfDocumentsToInsert)
        {
            requestUnitsConsumed[taskId] = 0;
            ItemResponse<FryerTelemetryModel> fryerCollectionResp = null;
            ItemResponse<OvenTelemetryModel> ovenCollectionResp = null;
            ItemResponse<RotisserieTelemetryModel> rotisserieCollectionResp = null;
            ItemResponse<TypeATelemetryModel> typeACollectionResp = null;
            ItemResponse<TypeBTelemetryModel> typeBCollectionResp = null;

            using CosmosClient client = new CosmosClient(endpoint, authKey);
            Container container = client.GetDatabase(DatabaseName).GetContainer(DataCollectionName);
            //string partitionKeyProperty = collection.PartitionKey.Paths[0].Replace("/", "");
            //Dictionary<string, object> newDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(sampleJson);
            for (var i = 0; i < numberOfDocumentsToInsert; i++)
            {
                //newDictionary["id"] = Guid.NewGuid().ToString();
                //newDictionary[partitionKeyProperty] = Guid.NewGuid().ToString();
                try
                {
                    switch (taskId)
                    {
                        case 0:
                            // Pass in the object and the SDK will automatically extract the full partition key path
                            fryerCollectionResp = await container.CreateItemAsync(TelemetryGenerator.GenerateFryerTelemetry(siteIndex));
                            requestUnitsConsumed[taskId] += fryerCollectionResp.RequestCharge;
                            break;
                        case 1:
                            ovenCollectionResp = await container.CreateItemAsync(TelemetryGenerator.GenerateOvenTelemetry(siteIndex));
                            requestUnitsConsumed[taskId] += ovenCollectionResp.RequestCharge;
                            break;
                        case 2:
                            rotisserieCollectionResp = await container.CreateItemAsync(TelemetryGenerator.GenerateRotisserieTelemetry(siteIndex));
                            requestUnitsConsumed[taskId] += rotisserieCollectionResp.RequestCharge;
                            break;
                        case 3:
                            typeACollectionResp = await container.CreateItemAsync(TelemetryGenerator.GenerateTypeATelemetry(siteIndex));
                            requestUnitsConsumed[taskId] += typeACollectionResp.RequestCharge;
                            break;
                        case 4:
                            typeBCollectionResp = await container.CreateItemAsync(TelemetryGenerator.GenerateTypeBTelemetry(siteIndex));
                            requestUnitsConsumed[taskId] += typeBCollectionResp.RequestCharge;
                            break;
                    }
                    //string partition = response.SessionToken.Split(':')[0];
                    //requestUnitsConsumed[taskId] += response.RequestCharge;
                    Interlocked.Increment(ref this.documentsInserted);
                }
                catch (Exception e)
                {
                    if (e is DocumentClientException)
                    {
                        DocumentClientException de = (DocumentClientException)e;
                        if (de.StatusCode != HttpStatusCode.Forbidden)
                        {
                            //Trace.TraceError("Failed to write {0}. Exception was {1}", JsonConvert.SerializeObject(sampleEvent), e);
                            Trace.TraceError("Exception was {0}", e);
                        }
                        else
                        {
                            Interlocked.Increment(ref this.documentsInserted);
                        }
                    }
                }
            }

            Interlocked.Decrement(ref this.pendingTaskCount);
        }
        #endregion

        #region LogOutputStats
        private async Task LogOutputStats()
        {
            long lastCount = 0;
            double lastRequestUnits = 0;
            double lastSeconds = 0;
            double requestUnits = 0;
            double ruPerSecond = 0;
            double ruPerMonth = 0;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (this.pendingTaskCount > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                double seconds = watch.Elapsed.TotalSeconds;

                requestUnits = 0;
                foreach (int taskId in requestUnitsConsumed.Keys)
                {
                    requestUnits += requestUnitsConsumed[taskId];
                }

                long currentCount = this.documentsInserted;
                ruPerSecond = (requestUnits / seconds);
                ruPerMonth = ruPerSecond * 86400 * 30;

                Console.WriteLine("Inserted {0} docs @ {1} writes/s, {2} RU/s ({3}B max monthly 1KB reads)",
                    currentCount,
                    Math.Round(this.documentsInserted / seconds),
                    Math.Round(ruPerSecond),
                    Math.Round(ruPerMonth / (1000 * 1000 * 1000)));

                lastCount = documentsInserted;
                lastSeconds = seconds;
                lastRequestUnits = requestUnits;
            }

            double totalSeconds = watch.Elapsed.TotalSeconds;
            ruPerSecond = (requestUnits / totalSeconds);
            ruPerMonth = ruPerSecond * 86400 * 30;

            Console.WriteLine();
            Console.WriteLine("Summary:");
            Console.WriteLine("--------------------------------------------------------------------- ");
            Console.WriteLine("Inserted {0} docs @ {1} writes/s, {2} RU/s ({3}B max monthly 1KB reads)",
                lastCount,
                Math.Round(this.documentsInserted / watch.Elapsed.TotalSeconds),
                Math.Round(ruPerSecond),
                Math.Round(ruPerMonth / (1000 * 1000 * 1000)));
            Console.WriteLine("--------------------------------------------------------------------- ");
        }
        #endregion

        #region CreatePartitionedCollectionAsync
        /// <summary>
        /// Create a normal partitioned collection.
        /// </summary>
        /// <returns>The created collection.</returns>
        private async Task<DocumentCollection> CreatePartitionedCollectionAsync(string databaseName, string collectionName)
        {
            DocumentCollection existingCollection = GetCollectionIfExists(databaseName, collectionName);

            DocumentCollection collection = new DocumentCollection();
            collection.Id = collectionName;
            collection.PartitionKey.Paths.Add(ConfigurationManager.AppSettings["CollectionPartitionKey"]);

            // Show user cost of running this test
            double estimatedCostPerMonth = 0.06 * CollectionThroughput;
            double estimatedCostPerHour = estimatedCostPerMonth / (24 * 30);
            Console.WriteLine("The collection will cost an estimated ${0} per hour (${1} per month)", Math.Round(estimatedCostPerHour, 2), Math.Round(estimatedCostPerMonth, 2));
            //Console.WriteLine("Press enter to continue ...");
            //Console.ReadLine();

            return await client.CreateDocumentCollectionAsync(
                    UriFactory.CreateDatabaseUri(databaseName),
                    collection,
                    new Microsoft.Azure.Documents.Client.RequestOptions { OfferThroughput = CollectionThroughput });
        }
        #endregion

        #region CreateHierarchialPartitionedCollectionAsync
        private async static Task<Container> CreateHierarchialPartitionedCollectionAsync(Microsoft.Azure.Cosmos.Database database, string databaseName, string collectionName)
        {
            // Show user cost of running this test
            double estimatedCostPerMonth = 0.06 * CollectionThroughput;
            double estimatedCostPerHour = estimatedCostPerMonth / (24 * 30);
            Console.WriteLine("The collection will cost an estimated ${0} per hour (${1} per month)", Math.Round(estimatedCostPerHour, 2), Math.Round(estimatedCostPerMonth, 2));

            List<string> subpartitionKeyPaths = new List<string> { "/deviceId", "/date" };
            ContainerProperties containerProperties = new(id: DataCollectionName, partitionKeyPaths: subpartitionKeyPaths);
            return await database.CreateContainerIfNotExistsAsync(containerProperties, throughput: 400);
        }
        #endregion

        #region GetDatabaseIfExists
        /// <summary>
        /// Get the database if it exists, null if it doesn't
        /// </summary>
        /// <returns>The requested database</returns>
        private Task<DatabaseResponse> GetDatabaseIfExists(string databaseName)
        {
            using CosmosClient client = new CosmosClient(endpoint, authKey);
            return client.CreateDatabaseIfNotExistsAsync(databaseName);
        }
        #endregion

        #region GetCollectionIfExists
        /// <summary>
        /// Get the collection if it exists, null if it doesn't
        /// </summary>
        /// <returns>The requested collection</returns>
        private DocumentCollection GetCollectionIfExists(string databaseName, string collectionName)
        {
            if (GetDatabaseIfExists(databaseName) == null)
            {
                return null;
            }

            return client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(databaseName))
                .Where(c => c.Id == collectionName).AsEnumerable().FirstOrDefault();
        } 
        #endregion
    }
}

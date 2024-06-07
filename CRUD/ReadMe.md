## Walmart.IoT.Platform.Cosmos

- This project is excerpted from the [azure-cosmos-dotnet-repository](https://github.com/IEvangelist/azure-cosmos-dotnet-repository).
- This package wraps the [NuGet: Microsoft.Azure.Cosmos package](https://www.nuget.org/packages/Microsoft.Azure.Cosmos),
exposing a simple dependency-injection enabled `IRepository<T>` interface.
- `IRepositoryFactory` is the base interface exposed for consuming cosmos repository operations.
- Uses [hierarchical partition keys](https://github.com/AzureCosmosDB/HierarchicalPartitionKeysFeedbackGroup) — also known as subpartitioning — in Azure Cosmos DB.

## Getting started

1. Create an Azure Cosmos DB SQL resource. You can use `Azure Cosmos DB Emulator` for `dev` environment.
1. Obtain the resource connection string from the **Keys** blade, be sure to get a connection string and not the key - these are different. The connection string is a compound key and endpoint URL.
1. Call `AddCosmosRepository`:

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddCosmosRepository();
   }
   ```

   The optional `setupAction` allows consumers to manually configure the `RepositoryOptions` object:

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddCosmosRepository(
           options =>
           {
               options.CosmosConnectionString = "<connection string>";
               options.ContainerId = "data-store";
               options.DatabaseId = "samples";
               /*
                   Partition Key is of type IReadOnlyList. In order to comply to the current hierarchical partitioning scheme, maxx levels are set to 3.
               */
               options.PartitionKey = new List<string>()
                                                        {
                                                        "/partition_key_level_1",
                                                        "/partition_key_level_2",
                                                        "/partition_key_level_3"
                                                        };
           });
   }
   ```

Above options are directly set into `DefaultCosmosItemConfigurationProvider` class by `AddOptions` method.

1. Define your object graph, objects must inherit `Item`, for example:

   ```csharp
   using Microsoft.Azure.CosmosRepository;

   public class Person : Item
   {
       public string FirstName { get; set; }
       public string LastName { get; set; }
   }
   ```

1. Ask for an instance of `IRepository<TItem>`, in this case the `TItem` is `Person`:

   ```csharp
   using Microsoft.Azure.CosmosRepository;

   public class Consumer
   {
       readonly IRepository<Person> _repository;

       public Consumer(IRepository<Person> repository) =>
           _repository = repository;

       // Use the repo...
   }
   ```

2. Perform any of the operations on the `_repository` instance, create `Person` records, update them, read them, or delete.

One example is given in `Walmart.IoT.Platform.Cosmos.Consumer.Program.cs` where `RawRepositoryExampleAsync` performs CRUD operations on `FryerTelemetryModel` class.
Alternatively, you can have a service layer which internally performs CRUD , as shown in `ServiceExampleAsync` method.

---

#### Configuration

`RepositoryOptions` class defines various configuration properties.

- `IsAutoResourceCreationIfNotExistsEnabled` :  Creates the required collection and database if not already created. By Default it is set to `true`.
- `OptimizeBandwidth` : `true` (by default) ,the repository SDK reduces networking and CPU load by not sending the resource back over the network and serializing it to the client. This is specific to writes, such as create, update, and delete.

Key | Data type	| Default value
----|-------------|--------------
RepositoryOptions__CosmosConnectionString	| string |	null
RepositoryOptions__AccountEndpoint |string | null
RepositoryOptions__DatabaseId|string|"database"
RepositoryOptions__ContainerId|string|"container"
RepositoryOptions__OptimizeBandwidth|boolean|true
RepositoryOptions__ContainerPerItemType|boolean|false
RepositoryOptions__AllowBulkExecution|boolean|false
RepositoryOptionsSerializationOptionsIgnoreNullValues|boolean|false
RepositoryOptionsSerializationOptionsIndented|boolean|false
RepositoryOptionsSerializationOptionsPropertyNamingPolicy|CosmosPropertyNamingPolicy|CosmosPropertyNaming


#### Example appsettings.json

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "RepositoryOptions": {
    "CosmosConnectionString": "<Your-CosmosDB-ConnectionString>",
    "AccountEndpoint": "<Your-CosmosDB-URI>"
    "DatabaseId": "<Your-CosmosDB-DatabaseName>",
    "ContainerId": "<Your-CosmosDB-ContainerName>",
    "OptimizeBandwidth": true,
    "AllowBulkExecution": true,
    "SerializationOptions": {
      "IgnoreNullValues": true,
     "PropertyNamingPolicy": "CamelCase"
    }
  }
}
```

### Changing underlying DB implementation

- `IRepository` , `IReadOnlyRepository` , `IWriteOnlyRepository` and `IBatchRepository` consist of generic CRUD methods with single/multiple inputs of generic type `<TItem>`.
- In case of change , contract would remain same, but there will be change in underlying logic on the library side.

## Method Signatures

### C

```
ValueTask<TItem> CreateAsync(
TItem value,
CancellationToken cancellationToken = default)
```

```
ValueTask<IEnumerable<TItem>> CreateAsync(
        IEnumerable<TItem> values,
        CancellationToken cancellationToken = default)
```

### R

- Optionally pass a synthetic partition key if the underlying DB implementation is expected to be Azure Cosmos SQL

```
ValueTask<TItem?> TryGetAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
```

- Passing a LINQ predicate

```
ValueTask<IEnumerable<TItem>> GetAsync(
        Expression<Func<TItem, bool>> predicate,
        CancellationToken cancellationToken = default)
```

- Passing a query as string

```
ValueTask<IEnumerable<TItem>> GetByQueryAsync(
        string query,
        CancellationToken cancellationToken = default)
```

```
ValueTask<IEnumerable<TItem>> GetByQueryAsync(
        QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
```

### U

```
ValueTask UpdateAsync(string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        string? partitionKeyValue = null,
        string? etag = default,
        CancellationToken cancellationToken = default)
```

### D

```
ValueTask DeleteAsync(
        string id,
        string? partitionKeyValue = null,
        CancellationToken cancellationToken = default)
```

---

### References

- Learn more about the options available such as Paging, Etag , Querying , etc [here](https://ievangelist.github.io/azure-cosmos-dotnet-repository/1-getting-started/)

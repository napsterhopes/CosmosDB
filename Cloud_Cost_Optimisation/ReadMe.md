# Cloud Cost Optimization

## Context
Azure Cosmos DB is fully managed NoSQL database service that is globally distributed and multi-model. 
Cosmos provides seamless and automatic scaling, low-latency data access, and a flexible and schema-less data model. 
As an application developer, it is essential to understand optimization to improve the performance and cost of Cosmos DB. 
Many application teams are bleeding on incurring costs involved with Cosmos DB due to either incorrect configuration, scalability and data model. 
The scope of this document is provide in depth guiding principles involved in optimizing Cosmos Database with primary focus on Cost and Performance. 

### Design Principles

Following are some of the key factors and best practices learned through operational excellence exercise, and organized in order of the priority. Notice that, I have marked the high priority principles those are critical in Cosmos database, any compromised leads to performance and cost related issues. 

![image](https://github.com/napsterhopes/AZ/assets/12064832/c1d237c3-9c2e-4c48-b437-0f34dd6ad74c)

#### Partitioning Strategy (High Priority)
1. Choose the right partition key to ensure even data distribution and optimal cost to performance.

A partition key has two components: partition key path and the partition key value. For example, consider an item

```
{
    "deviceClass" : "Fryer",
    "deviceId": "223-F"
}
```
if we choose `deviceClass` as the partition key, the following are the two partition key components:

The partition key path (For example: "/deviceClass")
The partition key value (For example: "Fryer"). The partition key value can be of string or numeric types.
Selecting the partition key is a crucial design choice. The partition key should:
Be a property that has a value, which doesn't change. If a property is your partition key, you can't update that property's value.
Should only contain String values. Numbers, GUIDs should ideally be converted into a String.
Have a high cardinality (wide range of possible values)
Spread request unit (RU) consumption and data storage evenly across all logical partitions. This spread ensures even RU consumption and storage distribution across your physical partitions.
Have values that are no larger than 2048 bytes typically, or 101 bytes if large partition keys aren't enabled.

In world of Cosmos DB, across partition access queries incur high costs in request units (RUs) and suffers lower in performance. Let's discuss the use case of Task Management model to understand the impact.

```
{  
    "projectId": "prj234",
    "taskId": 8762,
    "assigneeId": "user123",
    "priority": 1,
    "dueDate": "2023-05-03T10:30:00Z",
    "status": "pending"
}
```

Following partition key can lead to uneven data distribution and reduced performance, as tasks with the same status will be concentrated in the same partition.

```
{  
    "paths": [
        "/status"
    ],
    "kind": "Hash"
}
```

Following partition key ensures that tasks are distributed evenly across partitions based on the project they belong to, improving performance and query efficiency.

```
{  
    "paths": [
        "/projectId"
    ],
    "kind": "Hash"
}
```

Following partition key - a hierarchical pattern can be used if query for Task and given ProjectId, projects and tasks are distributed evenly across partition under Project.

```
{  
    "paths": [
        "/projectId",
        "/taskId"
    ],
    "kind": "Hash"
}
```

> How about dueDate  in query?

The attribute dueDate can be included in query filters, but it would be slightly lower performance when filtering by dueDate since the query may need to scan multiple partitions. Following additional strategies might need to be evaluated --

- Composite partition as shown below, but it could lead to high partition key cardinality and potentially many smaller partitions. 

```
{  
    "paths": [
        "/projectId",
        "/taskId",
        "/dueDate"
    ],
    "kind": "Hash"
}
```

- Synthetic partition as shown below based on combination of the three. This can provide a balance between partition granularity and partition size, hence distributing more evenly.  

```
{  
    "paths": [
        "/partitionKey"
    ],
    "kind": "Hash"
}
```

Partition key would be constructed as part of the model with projectId, taskId, and year YYYY , month MM 

```
{  
    "projectId": "prj234",
    "taskId": 8762,
    "assigneeId": "user123",
    "priority": 1,
    "dueDate": "2023-05-03T10:30:00Z",
    "status": "pending",
    "partitionKey": "prj234-8762-2023-05"
}
```

2. Avoid hot partitions by selecting a partition key with high cardinality and even access patterns.

The hot partitions are primarily caused by fat-partitions, high volume of data to be concentrated on a single partition. In above task management use case, if chosen partition key happens to be dueDate then we would run into this scenario. 

``
{  
    "paths": [
        "/dueDate"
    ],
    "kind": "Hash"
}`
``

3. Monitor partition key usage and adjust as needed based on performance and scalability requirements.

In task management use case, application based on the state of requirements might choose initial partition key as `projectId` - which works well when the number tasks per project is relatively even across all projects. However, as the application grows, certain projects maybe larger number of tasks, leading to hot partitions. In this case, we need to continue to [monitoring](https://github.com/napsterhopes/AZ/tree/main/CosmosDB/Monitoring) and adjusting the partition key, maybe in this case – combination of `projectId` and `taskId` to ensure event distribution of the data across partitions.

Following screenshot from [Microsoft Cosmos Partition Monitoring](https://learn.microsoft.com/en-us/azure/cosmos-db/monitor-normalized-request-units#how-to-identify-if-theres-a-hot-partition), If there's one PartitionKeyRangeId that has significantly higher normalized RU consumption than others (for example, one is consistently at 100%, but others are at 30% or less), this can be a sign of a hot partition.

---

### Indexing (High Priority)

In Azure Cosmos DB, every container has an indexing policy to determine how the container's items should be indexed. The default indexing policy for newly created containers indexes every property of every item and enforces range indexes for any string or number to get good query performance. However, the indexing policy can be customised for specific requirements of each container. There are 2 indexing modes supported by Azure Cosmos DB:

Consistent: The indexing is updated synchronously as items are created, deleted and updated. None: Indexing is disabled. This mode should be used when a container is used as a pure key-value store without the need for secondary indexes. It can also be used to improve the performance of bulk operations. Index Size: The total consumed storage for any cosmos DB is the combination of both the Data size and Index size. The index size depends on the indexing policy. If all the properties are indexed, then the index size can be larger than the data size. Unnecessary indexing on all properties (if not needed) may lead to storage consumption.

1. Use automatic indexing wisely and disable it for properties that don't require indexing.
Keeping all properties indexed (default behavior) would directly contribute to costs in terms of RU, and exponential on update transactions.

```
{  
    "projectId": "prj234",
    "taskId": 8762,
    "assigneeId": "user123",
    "priority": 1,
    "dueDate": "2023-05-03T10:30:00Z",
    "status": "pending"
    "attachments": "url...",
    "notes": "....."
}  
```

Suppose you never query or filter products based on their description or notes . In this case, you can disable indexing for the description and notes  properties to save storage space and reduce indexing overhead.

```
{
  "indexingMode": "consistent",
  "includedPaths": [
    {
      "path": "/*"
    }
  ],
  "excludedPaths": [
    {
      "path": "/attachments/*"
    },
    {
      "path": "/notes/*"
    }
  ]
}
```

3. Leverage composite indexes for complex queries involving multiple properties.

If you need to query tasks based on both **dueDate** and **priority**, you can create a composite index to optimize the performance of such queries.

```
{
  "indexingMode": "consistent",
  "includedPaths": [
    {
      "path": "/*"
    }
  ],
  "compositeIndexes": [
    [
      {
        "path": "/dueDate",
        "order": "ascending"
      },
      {
        "path": "/priority",
        "order": "ascending"
      }
    ]
  ],
   "excludedPaths": [
    {
      "path": "/attachments/*"
    },
    {
      "path": "/notes/*"
    }
  ]
}
```

---

### Query Optimization (High Priority)

1. Optimize query performance by filtering on indexed properties and using the most selective filters first.

Query to retrieve tasks assigned to a specific user with a specific status and due within the next week. Instead of querying tasks by **status** or **dueDate**  first, filter tasks by the most selective filter, in this case, the assigneeId . This reduces the number of tasks that need to be filtered in subsequent steps. Then, further filter by **status** and **dueDate**.

```
{  
    "paths": [
        "/projectId",
        "/assigneeId"
    ],
    "kind": "Hash"
}
```

```
SELECT * FROM tasks t
WHERE t.projectId = 'prj234'
AND t.assigneeId = 'user123'
AND t.status = 'pending'
AND t.dueDate < '2023-06-01'
```

2. Minimize cross-partition queries by designing an efficient partitioning strategy and querying with partition key values.

Query to retrieve tasks from a specific project with a status of `in progress` and partition key in this case happens to be - `projectId`. By querying tasks with the partition key value (`projectId`), we would avoid cross-partition queries, which can be more expensive and slower.

```
SELECT * FROM tasks t
WHERE t.projectId = 'prj234'
AND t.status = 'in progress'
```

3. Use continuation tokens for large result sets and paginate results to improve query performance.

Query to retrieve all tasks from the given project where task count might be in thousands. Here, we need to paginate the result by fetching a smaller number of tasks per requests (e.g. 25 tasks). Cosmos DB provides a continuation token in the response header (`x-ms-continuation`) when there are more results to fetch. When making the next request, include the continuation token in the request header (`x-ms-continuation`) to fetch the next 25 tasks. Repeat the process until all 100 tasks are retrieved or no more continuation tokens are returned.

```
SELECT * FROM tasks t
WHERE t.projectId = 'prj234'
ORDER BY dueDate DESC  
```

### Data Modeling (Medium Priority)

1. Design a denormalized data model to reduce the need for expensive join operations and improve query performance.

Depending on the situations, if document have few writes ops, then denormalization yield better performance. Let's consider the use case of task and assignee. Instead of storing assignee information separately from the task, we can include the assignee's username and full name directly in the post document. This eliminates the need for join operations when retrieving tasks with their assignee information under the assumption of assignee attributes does not change and immutable. 

```
{
  "projectId": "prj234",
  "taskId": 8762,
  "priority": 1,
  "dueDate": "2023-05-03T10:30:00Z",
  "assignee": {
    "userId": "user123",
    "username": "johndoe",
    "fullName": "John Doe"
  }
}
```

2. Use reference documents or materialized views to precompute and store aggregated data or relationships.

In above use case of task management, let's say we have predecessor and successor relationships. Instead of embedding an array of predecessors or successors within the task document, create a separate reference document for each task relationship. This allows for efficient queries to retrieve task's predecessors without loading the entire task document.

```
{
  "projectId": "prj234",
  "taskId": 8762,
  "priority": 1,
  "dueDate": "2023-05-03T10:30:00Z"
}
```

Task Relationship Document

```
{
  "projectId": "prj234",
  "predecessorTaskId": 8762,
  "successorTaskId": 3832
}
```

3. Utilize Time to Live (TTL) to automatically expire and delete old data, reducing storage costs and retaining only relevant information.

The documents that required to be deleted, set a TTL value on each document to automatically expire and delete the data after a specified period. The classic use case would be expired tokens type scenarios. The temporary token has a TTL of 3600 seconds (1 hour) and will be automatically deleted from the database after it expires. Automatic deletes use cost RUs efficiently.

```
{
  "tokenId": "token123",
  "userId": "user123",
  "createdAt": "2023-01-01T00:00:00Z",
  "expiresAt": "2023-01-01T01:00:00Z",
  "ttl": 3600
}
```

---

### Throughput Management (Medium Priority)
1. Provision the right amount of throughput (RU/s) based on your application's performance and latency requirements.

In this case, you need to [analyze the patterns of read and write requests](https://learn.microsoft.com/en-us/azure/cosmos-db/use-metrics) during peak and off-peak hours. By understanding these patterns, you can set the right throughput to avoid over-provisioning or under-provisioning Request Units (RUs).
As initial starting point, you could set the initial throughput to 5000 RU/s and adjust it based on the actual usage patterns observed in your application. Following two screenshots shows two [System tab charts](https://learn.microsoft.com/en-us/azure/cosmos-db/use-metrics). One that shows all metadata requests for an account. The second shows metadata requests throughput consumption from the account's master partition that stores an account's metadata.

2. Use autoscale throughput to automatically adjust RU/s based on workload changes, ensuring optimal resource utilization and cost-efficiency.

In this use case, we can control throughput programmatically. Following code snippet, we set the maximum autoscale throughput to 10,000 RU/s. With this configuration, Cosmos DB will automatically adjust the provisioned throughput for the container based on the current workload, up to a maximum of 10,000 RU/s. This ensures that you only pay for the throughput you need, optimizing resource utilization and cost-efficiency. 

3. Monitor RU consumption and adjust provisioned throughput as needed to avoid throttling or over-provisioning.

![image](https://github.com/napsterhopes/AZ/assets/12064832/b8271385-a45f-45fa-8cdb-d8c7ffe1369e)

---

### Consistency Level (Medium Priority)

1. Choose the appropriate consistency level based on your application's requirements for data consistency, performance, and latency.
Cosmos DB offers following consistency levels as shown in the screenshot below and explained nicely on the [Cosmos DB documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/consistency-levels#guarantees-associated-with-consistency-levels). In task management use case, let's consider Task and Notes as separate documents. For the tasks, consistency of choice could be "Session" because if offers a balance between performance and consistency. App users can read task changes immediately as it is updated, which is important when creating and changing task record. On the other hand, Task Notes could be "Eventual" consistency level to achieve better performance, as immediate consistency is less critical for notes. App users may see slightly out-of-date notes, but this trade-off is accessible for improved read-and-write performance.

![image](https://github.com/napsterhopes/AZ/assets/12064832/e02e69b4-f199-438d-a524-2e60fe2b404a)

2. Use lower consistency levels, such as Session or Eventual, for read-heavy workloads to minimize RU consumption and improve query performance.
As mentioned in above example, Session or Eventual are heavily used for read-heavy workloads and provides most optimal performance. If requirement for stronger consistency guarantees over performance, then utilize higher consistency levels, such as Bounded Staleness or Strong, for critical data and operations  

3. Leverage per-request consistency overrides for specific scenarios.

Cosmos DB provides programatic control for overrides per request basis. The use case in this could be for storing messages, a "Session" consistency level to ensure users can read their own writes. However, for user read receipts, override the default consistency level by specifying "Eventual" consistency on a per-request basis. This provides better performance for read receipts, which are less critical for immediate consistency.

```
FeedOptions feedOptions = new FeedOptions();
feedOptions.setConsistencyLevel(ConsistencyLevel.EVENTUAL);
 
FeedResponse<Document> response = client.readDocuments("dbs/db1/colls/coll1", feedOptions);
```

```
QueryRequestOptions queryRequestOptions = new()
{
    ConsistencyLevel = ConsistencyLevel.Eventual
};
```

---

### Global Distribution (Low Priority)
1. Enable multi-region writes for low-latency, high-throughput read and write operations across all regions.

If application absolutely requires multi-region writes due to performance measures, you can enable multi-region writes to allow users from different regions to write data to their nearest data center. This minimizes write latency and ensures that content created by users is immediately available in their region. On the other hand, the cost in request units (RUs) will go up and some cases, conflict resolution policies required.

2. Use active-active replication to improve availability and failover capabilities in case of regional outages.

Active-Active replication eliminates the need to manually manage regional failover and manages automatically between regions. The configuration can be used with single-region-write option to save on request-units and application performance needs. When failover occurs, Cosmos DB automatically promotes one of the read regions to become the new primary write region. Although active-active replication in Cosmos DB provides several benefits, such as high availability, lower latency, and automatic failover capabilities, there are some known issues and challenges:

- Conflict resolution: Write conflicts can occur when the same data is updated concurrently in different regions. It requires to choose a suitable conflict resolution strategy (Last Writer Wins or custom conflict resolution) to handle these conflicts.
- Data consistency: With multiple regions accepting writes, maintaining data consistency across all regions can be challenging. Cosmos DB offers various consistency levels but apps need to select the one that best meets application's requirements.
- Cost: Multi-master configurations can increase the cost of your Cosmos DB account, as you'll need to provision throughput (RU/s) for all regions to handle both read and write operations. The trade-offs needs to be considered between cost and the benefits of active-active replication.
- Application logic: Implementation may require changes to application logic to direct read and write operations to the appropriate regions. In a single write region setup with active-active capabilities, the application must handle redirecting write operations to the new write region during a failover.
- Latency during failover: While it improves latency in normal operation, during a failover event, write latency may increase temporarily as the system promotes a read region to become the new write region.


3. Use the appropriate conflict resolution policy to handle multi-region write conflicts.
Consider the use case of note-taking app with users editing notes simultaneously from different regions. With multi-region writes enabled, two users may edit the same note simultaneously, causing a write conflict. In this case, we can choose a conflict resolution policy that suits application's requirements. One option is to use Last Writer Wins (LWW), this policy uses a timestamp or a version number to resolve conflicts, ensuring that the latest write is preserved. The policy is suitable when the most recent change is most important.

```
{
  "id": "note1",
  "content": "This is a note.",
  "_ts": 1638553657
}
```

Secondary option is to use Custom conflict resolution, If LWW does not suit application's needs, one can implement a custom conflict resolution logic using stored procedures. This allows you to define how conflicts should be resolved based on your specific requirements, such as merging the changes or prompting the user to resolve the conflict manually. Firstly, create a stored procedure for custom conflict resolution, let's define following JavaScript

```
function resolveConflict(conflict, sourceDocument, targetDocument) {
    if (sourceDocument.timestamp > targetDocument.timestamp) {
        return sourceDocument;
    } else {
        return targetDocument;
    }
}
```

Secondly, upload the stored procedure to your Cosmos DB container using Azure portal, the Azure Cosmos DB SDK, or any other tool that supports Cosmos DB stored procedures. Alternatively, using Java SDK, following code snippet demonstrates the code.

```
import com.azure.cosmos.CosmosContainer;
import com.azure.cosmos.models.CosmosStoredProcedureProperties;
 
public class CustomConflictResolutionExample {
    // ...
 
    private static void createCustomConflictResolutionStoredProcedure(CosmosContainer container) {
        String storedProcedureId = "resolveConflict";
        String storedProcedureBody = "function(conflict, sourceDocument, targetDocument) {"
            + "  if (sourceDocument.timestamp > targetDocument.timestamp) {"
            + "    return sourceDocument;"
            + "  } else {"
            + "    return targetDocument;"
            + "  }"
            + "}";
 
        CosmosStoredProcedureProperties storedProcedureProperties = new CosmosStoredProcedureProperties(storedProcedureId, storedProcedureBody);
        container.getScripts().createStoredProcedure(storedProcedureProperties);
    }
}
```

Finally, configure custom conflict resolution for your Cosmos DB container, set the ConflictResolutionPolicy  to Custom  and specify the stored procedure's ID as the ConflictResolutionProcedure  when creating or updating the container. With this configuration, Cosmos DB will invoke the resolveConflict 

```
import com.azure.cosmos.CosmosContainer;
import com.azure.cosmos.CosmosContainerProperties;
import com.azure.cosmos.models.ConflictResolutionPolicy;
import com.azure.cosmos.models.ConflictResolutionMode;
import com.azure.cosmos.models.ThroughputProperties;
 
public class CustomConflictResolutionExample {
    // ...
 
    public static void main(String[] args) {
        // ...
 
        String databaseId = "exampleDatabase";
        String containerId = "exampleContainer";
        int maxAutoscaleThroughput = 10000;
 
        CosmosDatabase database = cosmosClient.createDatabaseIfNotExists(databaseId).getDatabase();
        CosmosContainerProperties containerProperties = new CosmosContainerProperties(containerId, "/partitionKey");
 
        // Configure custom conflict resolution
        ConflictResolutionPolicy customConflictResolutionPolicy = ConflictResolutionPolicy.createCustomPolicy("/dbs/" + databaseId + "/colls/" + containerId + "/sprocs/resolveConflict");
        containerProperties.setConflictResolutionPolicy(customConflictResolutionPolicy);
 
        ThroughputProperties autoscaleThroughputProperties = ThroughputProperties.createAutoscaledThroughput(maxAutoscaleThroughput);
        CosmosContainer container = database.createContainerIfNotExists(containerProperties, autoscaleThroughputProperties).getContainer();
 
        // Upload the custom conflict resolution stored procedure
        createCustomConflictResolutionStoredProcedure(container);
    }
}
```

---

### Query Optimisation

### Improving Query Performance

Following are the tips to improve query performance in cosmos DB:

- While creating connection, use Direct Mode for better performance. The gateway mode is better while your application runs within a corporate network with strict network rules because it has a single endpoint that can be configured to the firewall for security but,the gateway mode performance will be lower when compared to the direct mode.
- Exclude all properties from indexing (by default it is set to index all properties) and apply indexing only on the properties used in WHERE and ORDER clause. This reduces the RUs spent while querying and improves performance.

```
"includedPaths": [
        {
            "path": "/deviceId/*"
        },
        {
            "path": "/data/telemetry/telemetryTimestamp/?"
        }
    ]
```

---

- **Singelton Connection**: Connect to DB using Singelton approach and keep the connection alive by polling the DB within a specific period of time. This will decrease the connectivity latency.
- Deploy the application in the same region as cosmos. This reduces network latency.
- **Paging**: Cosmos queries, by default, pages the result set into 100 documents at a time. So even if the pageSize requested is 1000, cosmos will return first 100 records and then fetch the next 100 (and so on till 10 calls are made for all 1000 calls) using continuation token. Hence it is recommended to set the maxItemCount as 100 for most of the cases.
- Also, we might get a better performance in the prod environment as it is provisioned to use more RUs.

---

### Decreasing Document size

it is really important to store less document size to have better response times from cosmos (recommend from cosmos is less than 1 MB) and decreasing cost. Following are suggested ways to keep lesser document size

#### Avoid creating detailed readable attribute name, we need to use the mini version. 

- For eg., a simple attribute to store created time stamp currently name as “CreatedAt” or “CreateTimeStamp”.
- We don’t need to store attribute name in readble format as this can incur more space to for storage and retrieval (RUs).
- We can use a mini version of it’s equivalent for eg., “cts” (created time stamp).  – 15 char vs 3 char – when we have million of documents this saving of space is directly proportional.
- It is a small change but with very big value add. When we refer 100’s of attributes in the same document.
- We should be able to model any attribute name with-in 3 char in size, preferably even lesser.
- Showing the actual attribute at the json level can still be taken care at API level. But at DB level we should try to find out all possible options to keep lesser size.

#### Avoid storing deterministic string as values, instead use enum 
- For eg., attribute values like Status of an order, or inventory or similar values where the values are deterministic and static in nature, we don’t need to store complete value as-is
- Instead we can model it to a single char enum, worst case it may end up tow two char depending.
- Avoid usage of String for the attributes related to Yes/No. Instead use Boolean as it is better than using a string.

#### Compression 
- Once above points of #a and #b are followed the document size will be definitely 70% lesser than the actual document.
- Compress the document especially if the document have any unbounded arrays or multiple nested documents.
- Snappy or LZ4 can be used for compression as these are less CPU intensive and we can take advantage of the computing power if required we can add more computing power to scale

---

### References
- https://learn.microsoft.com/en-us/azure/cosmos-db/monitor-normalized-request-units#how-to-identify-if-theres-a-hot-partition
- https://learn.microsoft.com/en-us/azure/cosmos-db/optimize-cost-reads-writes
- https://learn.microsoft.com/en-us/azure/cosmos-db/optimize-cost-throughput
- https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview
- https://www.cloudiqtech.com/optimizing-azure-cosmos-db-performance/
- https://azure.microsoft.com/en-us/blog/a-technical-overview-of-azure-cosmos-db/
- https://learn.microsoft.com/en-us/azure/cosmos-db/use-metrics
- https://learn.microsoft.com/en-us/azure/cosmos-db/monitor-request-unit-usage
- https://learn.microsoft.com/en-us/azure/cosmos-db/consistency-levels
- https://confluence.walmart.com/display/STRATI/Cloud+Optimizer+Quickstart+How+to+Explore+and+Analyze+Cost+Optimization+with+Cloud+Optimizer
- https://confluence.walmart.com/display/IDCEBSTC/Guidelines+and+Best+Practices+-+Azure+Cosmos+DB
- https://dearanil9.medium.com/optimizing-cosmos-database-c7acc036cbd0
- https://willvelida.medium.com/understanding-indexing-in-azure-cosmos-db-62299c351a19
- https://www.cloudiqtech.com/optimizing-azure-cosmos-db-performance/#:~:text=If%20the%20total%20result%20for,data%20transaction%2C%20and%20increase%20performance.
- https://learn.microsoft.com/en-us/azure/cosmos-db/index-policy#includeexclude-strategy




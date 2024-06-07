### [Partitioning and horizontal scaling in Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview)

Azure Cosmos DB uses partitioning to scale individual containers in a database to meet the performance needs of your application. 
In partitioning, the items in a container are divided into distinct subsets called **logical partitions**.
Logical partitions are formed based on the value of a **partition key** that is associated with each **item** in a **container**. 
All the items in a logical partition have the same partition key value.


<img src="https://user-images.githubusercontent.com/12064832/200505398-296a371a-9e58-48b1-b791-5b73364f88e9.png" width="200" />

For example, a container holds items. Each item has a unique value for the `UserID` property. If `UserID` serves as the partition key for the items in the container and there are 1,000 unique `UserID` values, 1,000 logical partitions are created for the container.

In addition to a **partition key** that determines the item's logical partition, each item in a container has an **item ID** (unique within a logical partition). Combining the **partition key** and the **item ID** creates the item's **index**, which uniquely identifies the item.

#### [Logical partitions](https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview#logical-partitions)
A logical partition consists of a set of items that have the same partition key.
A logical partition also defines the scope of database transactions. 
You don't have to worry about deleting a logical partition when the underlying data is deleted.
There is no limit to the number of logical partitions in your container. Each logical partition can store up to **20GB** of data.

Always choose a partition key with a wide range of possible values so that container can be scaled.

#### Physical partitions
A container is scaled by distributing data and throughput across physical partitions. Internally, one or more logical partitions are mapped to a single physical partition. Typically smaller containers have many logical partitions but they only require a single physical partition. Unlike logical partitions, physical partitions are an internal implementation of the system and they are entirely managed by Azure Cosmos DB.
There is no limit to the total number of physical partitions in your container. As your provisioned throughput or data size grows, Azure Cosmos DB will automatically create new physical partitions by splitting existing ones. Physical partition splits do not impact your application's availability.

As a dev, we don't need to worry about physical partitions.

#### Replica set
Each physical partition consists of a set of replicas, also referred to as a `replica set`.

<img src="https://learn.microsoft.com/en-us/azure/cosmos-db/media/partitioning-overview/logical-partitions.png" width="500" />

#### Choosing a partition key
A partition key has two components: partition key path and the partition key value. For example, consider an item `{ "userId" : "Andrew", "worksFor": "Microsoft" }` if you choose "userId" as the partition key, the following are the two partition key components:

- The partition key path (For example: "/userId"). The partition key path accepts alphanumeric and underscore (_) characters. You can also use nested objects by using the standard path notation(/).
- The partition key value (For example: "Andrew"). The partition key value can be of string or numeric types.

Selecting your partition key is a simple but important design choice in Azure Cosmos DB. Once you select your partition key, it is not possible to change it in-place. If you need to change your partition key, you should move your data to a new container with your new desired partition key.

> Partition key for read-heavy containers
For most containers, the above criteria is all you need to consider when picking a partition key. For large read-heavy containers, however, you might want to choose a partition key that appears frequently as a filter in your queries. 

> [Use item ID as the partition key](https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview#use-item-id-as-the-partition-key)
If your container has a property that has a wide range of possible values, it is likely a great partition key choice. One possible example of such a property is the **item ID**. For small read-heavy containers or write-heavy containers of any size, the **item ID** is naturally a great choice for the partition key.



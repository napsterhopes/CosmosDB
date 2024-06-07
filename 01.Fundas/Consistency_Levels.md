#### Consistency Levels
Azure Cosmos DB is a globally distributed database service that provides well-defined levels of consistency for various application types and needs.
Consistency is based on the idea of how distributed databases have limitations and tradeoffs between consistency, availability, and partition tolerance. (CAP)

The **replicas** are identical copies of source data sitting in a different region for either DR purposes or for geo-based distribution of systems across the globe where the application fetches the data from its nearest source to reduce the latency and response time.
Consistency in CosmosDB is considered as the uniformity of the data while it's being replicated across the globe to different regions. It defines how and when the data is copied and committed into a replica.

In addition to that, spreading the cluster in multiple regions makes sure that if one node in a cluster goes down, the other node can handle the requests without any downtime.

> Consistency Models in CosmosDB
As with any feature, there's a steep price you have to pay to be able to leverage that functionality. It is no different with CosmosDB. There is a fundamental tradeoff between the read, availability, latency, and throughput while working with different CosmosDB consistency models.

https://parveensingh.com/cosmosdb-consistency-levels/#:~:text=it%20was%20written.-,Session,with%20the%20same%20session%20token.

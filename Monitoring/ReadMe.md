### [Monitor Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/monitor?tabs=azure-diagnostics)

> [View utilization and performance metrics for Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/insights-overview#view-utilization-and-performance-metrics-for-azure-cosmos-db)

1. Going to [insights](https://learn.microsoft.com/en-us/azure/cosmos-db/insights-overview#overview)

![image](https://github.com/napsterhopes/AZ/assets/12064832/9f77eb4b-8fc6-44a8-b727-5f2ab592b930)

2. Go to requests tab.It shows you the total requests with the distribution of responses that make up those requests:

![image](https://github.com/napsterhopes/AZ/assets/12064832/eef81167-41f6-4ce3-b3fe-eaed86f617fa)

3. Check storage and operations tab.

---

### [View from an Azure Cosmos DB resource](https://learn.microsoft.com/en-us/azure/cosmos-db/insights-overview#view-from-an-azure-cosmos-db-resource)

![image](https://github.com/napsterhopes/AZ/assets/12064832/a871d14e-b29c-4bc3-8fc8-69c3845bc873)

---

### [Normalized RU Consumption and autoscale](https://learn.microsoft.com/en-us/azure/cosmos-db/monitor-normalized-request-units#normalized-ru-consumption-and-autoscale)

The normalized RU consumption metric will show as 100% if at least 1 partition key range uses all its allocated RU/s in any given second in the time interval. One common question that arises is, why is normalized RU consumption at 100%, but Azure Cosmos DB didn't scale the RU/s to the maximum throughput with autoscale?

When you use autoscale, Azure Cosmos DB only scales the RU/s to the maximum throughput when the normalized RU consumption is 100% for a sustained, continuous period of time in a 5 second interval. This is done to ensure the scaling logic is cost friendly to the user, as it ensures that single, momentary spikes to not lead to unnecessary scaling and higher cost. When there are momentary spikes, the system typically scales up to a value higher than the previously scaled to RU/s, but lower than the max RU/s.

**For example, suppose you have a container with autoscale max throughput of 20,000 RU/s (scales between 2000 - 20,000 RU/s) and 2 partition key ranges. Each partition key range can scale between 1000 - 10,000 RU/s. Because autoscale provisions all required resources upfront, you can use up to 20,000 RU/s at any time. Let's say you have an intermittent spike of traffic, where for a single second, the usage of one of the partition key ranges is 10,000 RU/s. For subsequent seconds, the usage goes back down to 1000 RU/s. Because normalized RU consumption metric shows the highest utilization in the time period across all partitions, it will show 100%. However, because the utilization was only 100% for 1 second, autoscale won't automatically scale to the max.

As a result, even though autoscale didn't scale to the maximum, you were still able to use the total RU/s available. To verify your RU/s consumption, you can use the opt-in feature Diagnostic Logs to query for the overall RU/s consumption at a per second level across all partition key ranges.**


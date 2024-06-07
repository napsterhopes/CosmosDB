### Benchmarking

> [Page load time](https://panelbear.com/docs/page-load-time-explained/)

How does a browser load a web page?
When you click on a link, your web browser starts a multi-step process to load all synchronous resources required to show the web page on your screen, this is called the **Page Load Time**. 
The longer this process takes, the longer a visitor must wait to interact with a website.

Once loaded, a website may perform additional asynchronous requests which do not block that initial page load. This is most commonly seen in Single-page Applications (SPAs), in which as the visitor interacts with the site, more resources are loaded and the view is updated in-place.

**Page load time only measures the time it took to initially load a site** - it doesn't measure requests that happen after the initial load (eg. fetch requests or async scripts).

Performance Monitoring measures and aggregates your page load time into various metrics: average, percentiles (p50, p75, p95), and a simplified breakdown of the browser timings.

- 50th Percentile (p50): It indicates that 50% of the page views are accounted for in the value shown. For example a p50 of 2 seconds means that half the users experienced a load time longer than 2 seconds.
- 75th Percentile (p75): It indicates that 75% of the page views are accounted for in the value shown. For example a p75 of 3 seconds means that 75% of the users had a load time of 3 seconds or less (or 25% of the users experienced a load time longer than 3 seconds).
- 95th Percentile (p95): It indicates that 95% of the page views are accounted for in the value shown. For example a p95 of 5 seconds means that 95% of the users had a load time of 5 seconds or less (only 5% of the users experienced a load time longer than 5 seconds).

> Multi Cloud Cross Region Benchmarking

#### Testing Infrastructure
- Cosmos account : Single master account deployed in 3 regions (AZ SCUS, AZ WUS2 and AZ EUS2) with Private Link enabled
- Demo Application : [CosmosSQLCalipers](https://github.com/deepub/CosmosSQLCalipers) (Cosmos SQL API benchmarking utility)
- Application Hosting : Azure, GCP , AWS 

<img src="https://user-images.githubusercontent.com/12064832/207246628-0a3524c8-0d36-4b6c-8aae-ae98d16bc2d3.png" width=500 />

Repeat for other region and sample data.

### Optimizations

#### Resolution to Issue 1: Latency

→ Hosting app and db in the same region OR pointing application to nearest cosmos instance. 

While testing,we have have configured my app to use application region as West US.

→ Using point reads instead of queries allows cosmos SDK to skip the query execution plan.

#### Resolution to Issue 2: Rate Limiting or throttling

Choosing a partition key with more cardinality is the best choice.
Partitioning strategy tells Cosmos DB how you want us to distribute your data among all of the machines that will store the data.
For read heavy scenarios it tells us exactly which machine your data is on, so that we don't have to check all of them.
And then we use the partition key in the filter criteria while querying.
Also , we have used query options like [MaxItemCount](https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.queryrequestoptions.maxitemcount?view=azure-dotnet#microsoft-azure-cosmos-queryrequestoptions-maxitemcount)(allowing pagination),[MaxBufferedItemCount](https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.queryrequestoptions.maxbuffereditemcount?view=azure-dotnet#microsoft-azure-cosmos-queryrequestoptions-maxbuffereditemcount) (using cosmos feature to buffer the results),MaxConcurrency(to allow client system to automatically decide parallel query execution).

#### Optimizing bulk writes:

Let's say we have provisioned a lot of throughput (e.g 10,000 RU/s).

We use bulk executor library to ingest high data into Cosmos DB. 

While inserting the data we observed that we are not able to use complete effective RUs/s.

What we found is to investigate again is our partitioning strategy.

We were using /date and while simulating data we inserted data on the same day. So we were all bottlenecked on the same partition.

Instead we should pick a partition key with high cardinality such as timestamp.

#### Notes:
If we are expecting payload size to increase , we should do compression using wrappers.Cosmos SDK does not do any payload compression when in transit.

#### Introduction

One of the main features of Cosmos DB is being able to replicate data globally and configure the level of consistency. 
Cosmos DB in a single region has an availability SLA of **99.99%**. 
When geo-replication is enabled the SLA improves to **99.999%**. 

##### When you change the level of consistency, 
you should be aware of potential cost implications that it may have. 
For example, moving from the weakest consistency model called `eventual consistency` to the strongest called `strong consistency` can double the number of RUs required 
for reads. In that case, you would need to double the RUs to achieve the same throughput. 

Also be aware that once you enable geo-replication, you can no longer use the strong consistency model. 
The default consistency model of session consistency provides consistent reads and writes in the context of a particular user's session. 
You will likely know if your application can tolerate lower levels of consistency or demands higher consistency.

This Lab Step will show you how easy it is to geo-replicate the data in the stocks database, and illustrates how to configure different levels of consistency.

#### Instructions
1. In the Cosmos DB account blade, click **Replicate data globally:**

![image](https://user-images.githubusercontent.com/12064832/189894724-79ee7148-b909-4a7e-a48e-a2541c42a754.png)

 The account is currently configured to use only a single region. The region appears under **WRITE REGION** because single region databases must be read/write enabled. 
 Geo-replicas can be **read-only** (READ REGIONS) or read/write (WRITE REGION). 
 At the time of writing, only one write region is allowed by default, but multi-master support is being developed to enable multiple write regions and a public preview can be opted into.

2. Find and click on the **East US** region in the map to select it as a read region:

![image](https://user-images.githubusercontent.com/12064832/189896458-8d2ce576-dc97-4121-993c-3879ae6e96af.png)

You can alternatively, click **Add new region** and select the region from the list. In the case of having a write region in the West US, the choice of using East US as a read region is driven by business continuity and disaster recovery reasons (BCDR). 
Azure schedules updates and region recoveries based on [paired regions](https://docs.microsoft.com/en-us/azure/availability-zones/cross-region-replication-azure). 
In the unlikely event of a failed update or a multi-region outage, Azure will prioritize bringing one of the two regions in a pair back online.

 If your resource group was created in a different region, you would choose that region's paired region for the read region for BCDR. It is not a problem to choose East US for the sake of the Lab regardless where the write region is.
 
To perform the same operation using the Azure CLI, you would use the update command with the following arguments:
```
--locations westus=0 eastus=1
--resource-group "put your resource group here"
--name "put your CosmosDB name here"
```

![image](https://user-images.githubusercontent.com/12064832/189897767-2035c072-5d3f-4aff-ba89-873a0b1f28e7.png)

3. Click Save. 

It takes anywhere from a couple of minutes to ten minutes for the replication to complete. Once complete, you will automatically see a checkmark in the region hexagon.

4. Open your Cloud Shell and enter the following command: `az cosmosdb list`

Read through the output and notice a few fields in particular:

- **consistencyPolicy:** Observe **Session** consistency is being used

![image](https://user-images.githubusercontent.com/12064832/189898330-a282d082-44eb-4589-824e-db462ca070a2.png)

- **automaticFailover:** Automatically failing over to a read region if the write region becomes unavailable or is disabled by default

![image](https://user-images.githubusercontent.com/12064832/189898405-97c08b8f-e538-4f8e-831a-1666ce0714a2.png)

- **readLocations:** Two read locations are now available. The **locationName** indicates which region they are, and **failoverPriority** indicates the priority 
of each location to become the new write region in the event of a failover (lower numbers are higher priority).

![image](https://user-images.githubusercontent.com/12064832/189898661-483aa943-c43d-49e4-acca-9c5f9235de6d.png)

5. Enter the following to trigger a manual failover to the East US region:

```
# resetting shell variables in case cloud shell was terminated or refreshed
az cosmosdb list --output table --query '[].{Name: name, ResourceGroup: resourceGroup}'
read account resource_group <<< $(!! | tail -1)
# triggering manual failover
az cosmosdb failover-priority-change --failover-policies EastUS=0 WestUS=1 \
                                     --resource-group $resource_group --name $account
```

Manual failovers are useful for testing the end-to-end availability of applications. You can be confident your applications will continue operating as expected in the event of an Azure triggered failover. Cosmos DB guarantees zero data loss for manual failovers. Automatic failovers can lose data for an amount of time specified in the SLA.

It takes anywhere from one to three minutes for the failover to complete.

6. Connect to the database using the Mongo shell again (edit the Quick start MongoDB Shell command in the Portal to begin with ./mongo as you did in the previous Lab Step).

7. Use the stocks database:  `use stocks`
8. Run the **isMaster** command to see the location of the master (write) replica:  `db.isMaster()`

![image](https://user-images.githubusercontent.com/12064832/189900283-f7e7ecdd-8f79-49ae-ae23-993b42d64961.png)

Notice that the **primary** contains **eastus** in the hostname. This proves that MongoDB is using the East US region for the primary/write region.

8. List all the documents in the collection:  `db.ticker.find({})`

![image](https://user-images.githubusercontent.com/12064832/189901472-48b36186-409b-4411-aa53-196ae0c103fd.png)

All of the data has been preserved through the geo-replication and failover processes.

9. Click **Default Consistency** in the Cosmos DB blade:

![image](https://user-images.githubusercontent.com/12064832/189902420-586cae6c-a752-4acb-aa96-781c2d874499.png)

Read through the information blocks for the [different consistency models](https://docs.microsoft.com/en-us/azure/cosmos-db/consistency-levels) and notice that **STRONG** is disabled since the account is geo-replicated. 
There is no need to change the consistency model, just learn about the options available. 
The consistency can be changed using the Azure CLI update command with the `--default-consistency-level` argument.






                                     


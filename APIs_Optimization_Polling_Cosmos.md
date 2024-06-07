### [Optimization APIs which poll Cosmos](https://www.cloudiqtech.com/optimizing-azure-cosmos-db-performance/)

#### 1. Tune the indexing:

Make sure it is only pointing to properties which you query frequently.

```
Before: 
{
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
        {
            "path": "/*"
        }
    ],
    "excludedPaths": [
        {
            "path": "/\"_etag\"/?" 
        }
    ]
}
```

where `/deviceId` is partition key

```
After :
{
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
        {
            "path": "/deviceId/*"
        }
    ],
    "excludedPaths": [
        {
            "path": "/*" 
        }
    ]
}
```

#### 2. 

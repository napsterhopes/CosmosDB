### Sample data for querying

```
{
    "deviceId": "Giles-7111909211104",
    "metadata": {
        "messageId": "17694171715491491293887186305532032245156609939513",
        "messageType": "device_readings",
        "messageVersion": "1.4",
        "messageTimestamp": "2022-06-29T23:33:17.061Z",
        "manufacturerName": "Giles",
        "manufacturerDeviceModel": "EOF-24/FFLT/24",
        "manufacturerSerialNumber": "7111909211104",
        "manufacturerFamily": null,
        "customerGuid": "L2019-349321",
        "customerName": "Right Fryer Vat",
        "customerDeviceClass": "Fryer"
    },
    "headers": {
        "tenant-id": "81b4681d-137b-4738-96a2-08d962858d1c",
        "source-type": "EdgeModule",
        "site-id": "US-3479",
        "edge-device-id": "US-store-03479-01",
        "edge-module-id": "httpslistenser-giles"
    },
    "telemetry": {
        "dataItemType": "0",
        "inAlarm": "0",
        "errorCode": null,
        "heatingElementSetPoint": "275",
        "tempscale": null,
        "cookTimer1Sec": null,
        "cookTimer2Sec": null,
        "timer1Seconds": "360",
        "timer2Seconds": "480",
        "timer1Direction": "0",
        "timer2Direction": "0",
        "oilProbeTemp": "276.0",
        "cookCycleNumber": "5",
        "cookCyclesSinceFilter": "155",
        "telemetryTimestamp": "2022-06-29T23:33:00Z"
    },
    "error": {
        "errorTimestamp": "2022-06-29T23:33:00Z",
        "code": "0",
        "text": null,
        "index": "0"
    },
    "event": null,
    "id": "Giles-7111909211104_L2019-349321",
    "_rid": "trAXAOAnUHY0AAAAAAAAAA==",
    "_self": "dbs/trAXAA==/colls/trAXAOAnUHY=/docs/trAXAOAnUHY0AAAAAAAAAA==/",
    "_etag": "\"3e00449e-0000-0700-0000-62bce13f0000\"",
    "_attachments": "attachments/",
    "_ts": 1656545599
}
```

#### Query syntax

```
WHERE c.headers["site-id"] = "US-100" AND c.metadata["customerGuid"] = "L2019-536470"
```

```
SELECT * FROM c WHERE c.deviceId = '81b4681d-137b-4738-96a2-08d962858d1c$US-100$httpslistenser-giles$7163701051771' 
and c["data"].telemetry.telemetryTimestamp >= '2023-03-20T01:54:31Z' and c["data"].telemetry.telemetryTimestamp <= '2023-03-21T23:54:31Z' OFFSET 6892 LIMIT 992
```

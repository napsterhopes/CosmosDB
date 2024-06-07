using DVDBenchmark.Model;

namespace DVDBenchmark
{
    internal static class TelemetryGenerator
    {
        #region GenerateFryerTelemetry
        internal static FryerTelemetryModel GenerateFryerTelemetry(short siteIndex)
        {
            var changedEvent = new FryerTelemetryModel()
            {
                //deviceId_date = "Giles-7163712301905" + "_" + DateTime.Now.ToString("MMddyyyy"),
                date = DateTime.Now.ToString("MMddyyyy"),
                deviceId = "Giles-7163712301905",
                metadata = new DVDBenchmark.Model.Metadata()
                {
                    MessageId = "27136333478075765962291745926822057039590103211311",
                    MessageType = "device_readings",
                    MessageVersion = "1.3",
                    MessageTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    Manufacturer = new Manufacturer()
                    {
                        ManufacturerDeviceModel = "EOF-BIB/LT/24/24",
                        ManufacturerName = "Giles",
                        ManufacturerSerialNumber = "7163712301905",
                        ManufacturerFamily = ""
                    },
                    Customer = new Customer()
                    {
                        CustomerName = "Left Fryer Vat",
                        CustomerDeviceClass = "Fryer",
                        CustomerGuid = "2019-002997"
                    }
                },
                header = new DVDBenchmark.Model.Headers()
                {
                    tenantid = "43f4b965-aa85-405a-43d7-08d958688822",
                    sourcetype = "EdgeModule",
                    siteid = string.Format("US-{0}",siteIndex.ToString()),//"US-5260",
                    edgedeviceid = "US-store-05260-01",
                    edgemoduleid = "telemetryreceiver-01"
                },
                type = "Telemetry",
                data = new FryerTelemetryData
                {
                    nod = "1",
                    timer1Direction = "0",
                    controlMode1 = "3",
                    controlMode2 = "0",
                    dataItemType = "0",
                    boardTemp = "78.8",
                    cookCycleNumber = "0",
                    timer1Seconds = "420",
                    addLevelDiff = "0",
                    heatContactorCounter1 = "1",
                    inAlarm = "1",
                    heatContactorCounter2 = "0",
                    net = "1",
                    realTimeSeconds = "213877354",
                    cookCyclesSinceFilter = "0",
                    cooktimeMinutes = "0",
                    heatingElementsOnMinutes = "11",
                    timer2Direction = "0",
                    idleTimeMinutes = "8",
                    addLevelProbeTemp = "0.0",
                    epochTimeSeconds = "217019999",
                    heatingElementTemp = "164.4",
                    uptimeMinutes = "20",
                    heatingElementsIdleMinutes = "0",
                    oilProbeTemp = "118.9",
                    timer2Seconds = "420",
                    heatingElementSetPoint = "350",
                    basketElevatorCycles2 = "1",
                    powerCycleID = "213876154",
                    heatingElementActiveMinutes = "11",
                    basketElevatorCycles1 = "1",
                    rightMenuItemString = "MANUAL",
                    leftMenuNumber = "0",
                    rightMenuNumber = "51",
                    leftMenuItemString = "MANUAL",
                    telemetryTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    localTimestamp = "2021-11-16T13:19:59-06:00"
                },
                id = Guid.NewGuid().ToString(), //modified
                _rid = "x6MMALDeYzwBAAAAAAAAAA==",
                _self = "dbs/x6MMAA==/colls/x6MMALDeYzw=/docs/x6MMALDeYzwBAAAAAAAAAA==/",
                _etag = "\"0f0039e9-0000-0700-0000-619404600000\"",
                _attachments = "attachments/",
                _ts = 1637090400
            };
            return changedEvent; 
            //If you want to return string json
            //return JsonConvert.SerializeObject(changedEvent); 
        }
        #endregion

        #region GenerateOvenTelemetry
        internal static OvenTelemetryModel GenerateOvenTelemetry(short siteIndex)
        {
            var changedEvent = new OvenTelemetryModel()
            {
                //deviceId_date = "Baxter-24-2014949" + "_" + DateTime.Now.ToString("MMddyyyy"),
                date = DateTime.Now.ToString("MMddyyyy"),
                deviceId = "Baxter-24-2014949",
                metadata = new DVDBenchmark.Model.Metadata()
                {
                    MessageId = "11111111111111111111111132893764454459596",
                    MessageType = "device_readings",
                    MessageVersion = "1.3",
                    MessageTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    Manufacturer = new Manufacturer()
                    {
                        ManufacturerDeviceModel = "OV500G2-EE",
                        ManufacturerName = "Baxter",
                        ManufacturerSerialNumber = "24-2014949",
                        ManufacturerFamily = ""
                    },
                    Customer = new Customer()
                    {
                        CustomerName = "Baxter Bakery Oven",
                        CustomerDeviceClass = "Bakery Oven",
                        CustomerGuid = "L2019-345115"
                    }
                },
                header = new DVDBenchmark.Model.Headers()
                {
                    tenantid = "43f4b965-aa85-405a-43d7-08d958688822",
                    sourcetype = "EdgeModule",
                    siteid = string.Format("US-{0}", siteIndex.ToString()),//"US-5260",
                    edgedeviceid = "US-store-05260-01",
                    edgemoduleid = "itwbaxteroven"
                },
                type = "Telemetry",
                data = new OvenTelemetryData
                {
                    heatState = "ON",
                    doorState = "CLOSED",
                    electricityConsumptionCalc = "5",
                    rel_version = "REL_V006",
                    waterConsumptionCalc = "8",
                    overheatTime = "9",
                    serial_nb = "242014949",
                    rotateTime = "948",
                    lightCount = "17854",
                    convecFanTime = "1419",
                    bakingTimeAuto = "461",
                    liftDownTime = "682",
                    errorType = "NONE",
                    fanState = "ON",
                    model = "OV520G2",
                    operatingState = "TRUE",
                    program_total_time = "0",
                    CustomGUID = "",
                    ongoing_cycle = "FALSE",
                    energyType = "GAS",
                    ventState = "CLOSED",
                    bakingTimeManual = "447",
                    ventTime = "259",
                    gasConsumptionCalc = "5",
                    uc_version = "UC_V060",
                    rotateCount = "4720",
                    ventCount = "1318",
                    CustomName = "RACK OVEN",
                    operatingTime = "1783",
                    liftDownCount = "4689",
                    recipeName = "MANUAL",
                    cpuArchitecture = "ESP32-WROOM-32D",
                    heat_count = "30177",
                    steamCount = "2243",
                    cpuSwVersion = "ESP_V021",
                    lightTime = "1989",
                    hoodTime = "1995",
                    liftUpCount = "4768",
                    ovenTempSetPoint = "176.7",
                    hoodState = "ON",
                    manufacturer = "BAXTER",
                    totalStorage = "527",
                    cpuManufacturer = "ESPRESSIF",
                    macAddress = "f0:08:d1:84:3c:39",
                    ovenTempCurrent = "29.7",
                    program_elapsed_time = "0",
                    steamTime = "775",
                    hmi_version = "HMI_V055",
                    remainingServiceDays = "0",
                    liftUpTime = "676",
                    codeNumber = "TBD",
                    steamState = "OFF",
                    rackState = "OFF",
                    osName = "FreeRTOS",
                    numberOfEndOfCookingCycles = "4285",
                    totalMemory = "4091",
                    doorCount = "6940",
                    firmwareVersion = "Fla_V034",
                    hoodCount = "694",
                    convecFanCount = "27911",
                    family = "RACK OVEN",
                    heatTime = "944",
                    telemetryTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified,
                    localTimestamp = "2022-02-14T23:27:25-06:00"
                },
                id = Guid.NewGuid().ToString(), //modified
                _rid = "x6MMALDeYzwXYgQAAAAAAA==",
                _self = "dbs/x6MMAA==/colls/x6MMALDeYzw=/docs/x6MMALDeYzwBAAAAAAAAAA==/",
                _etag = "\"0000cb12-0000-0700-0000-620b39bf0000\"",
                _attachments = "attachments/",
                _ts = "1644902847"
            };
            return changedEvent;
        }
        #endregion

        #region GenerateRotisserieTelemetry
        internal static RotisserieTelemetryModel GenerateRotisserieTelemetry(short siteIndex)
        {
            var changedEvent = new RotisserieTelemetryModel()
            {
                //deviceId_date = "Fri-Jado-100073842-T" + "_" + DateTime.Now.ToString("MMddyyyy"),
                date = DateTime.Now.ToString("MMddyyyy"),
                deviceId = "Fri-Jado-100073842-T",
                metadata = new DVDBenchmark.Model.Metadata()
                {
                    MessageId = "15436a11-d026-45c8-b430-9116fb1391e2",
                    MessageType = "device_readings",
                    MessageVersion = "1.4",
                    MessageTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    Manufacturer = new Manufacturer()
                    {
                        ManufacturerDeviceModel = "FRI-JADO OVEN",
                        ManufacturerName = "Fri-Jado",
                        ManufacturerSerialNumber = "100073842-T",
                        ManufacturerFamily = ""
                    },
                    Customer = new Customer()
                    {
                        CustomerName = "Top Unit",
                        CustomerDeviceClass = "Rotisserie",
                        CustomerGuid = "L2019-345135"
                    }
                },
                header = new DVDBenchmark.Model.Headers()
                {
                    tenantid = "81b4681d-137b-4738-96a2-08d962858d1c",
                    sourcetype = "EdgeModule",
                    siteid = string.Format("US-{0}", siteIndex.ToString()),//"US-5260",
                    edgedeviceid = "US-store-05260-01",
                    edgemoduleid = "frijadooven"
                },
                type = "Telemetry",
                data = new RotisserieTelemetryData
                {
                    ovenTempCurrent = "4.77218534E7",
                    operationMinutes = "11236",
                    telemetryTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    localTimestamp = "2022-06-06T00:12:58-05:00"
                },
                id = Guid.NewGuid().ToString(), //modified
                _rid = "x6MMALDeYzwXYgQAAAAAAA==",
                _self = "dbs/x6MMAA==/colls/x6MMALDeYzw=/docs/x6MMALDeYzwBAAAAAAAAAA==/",
                _etag = "\"0e00e88a-0000-0700-0000-629d8ce10000\"",
                _attachments = "attachments/",
                _ts = "1654492385"
            };
            return changedEvent;
        }
        #endregion

        #region GenerateTypeATelemetry
        internal static TypeATelemetryModel GenerateTypeATelemetry(short siteIndex)
        {
            var changedEvent = new TypeATelemetryModel()
            {
                //deviceId_date = "TypeA-999973842-T" + "_" + DateTime.Now.ToString("MMddyyyy"),
                date = DateTime.Now.ToString("MMddyyyy"),
                deviceId = "TypeA-999973842-T",
                metadata = new DVDBenchmark.Model.Metadata()
                {
                    MessageId = "15436a11-d026-45c8-b430-9116fb1391e2",
                    MessageType = "device_readings",
                    MessageVersion = "1.4",
                    MessageTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    Manufacturer = new Manufacturer()
                    {
                        ManufacturerDeviceModel = "TYPEA OVEN",
                        ManufacturerName = "TypeA",
                        ManufacturerSerialNumber = "999973842-T",
                        ManufacturerFamily = ""
                    },
                    Customer = new Customer()
                    {
                        CustomerName = "Top Unit",
                        CustomerDeviceClass = "TypeA",
                        CustomerGuid = "L2019-345135"
                    }
                },
                header = new DVDBenchmark.Model.Headers()
                {
                    tenantid = "81b4681d-137b-4738-96a2-08d962858d1c",
                    sourcetype = "EdgeModule",
                    siteid = string.Format("US-{0}", siteIndex.ToString()),//"US-5260",
                    edgedeviceid = "US-store-05260-01",
                    edgemoduleid = "typeaoven"
                },
                type = "Telemetry",
                data = new TypeATelemetryData
                {
                    ovenTempCurrent = "4.77218534E7",
                    operationMinutes = "11236",
                    telemetryTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    localTimestamp = "2022-06-06T00:12:58-05:00"
                },
                id = Guid.NewGuid().ToString(), //modified
                _rid = "x6MMALDeYzwXYgQAAAAAAA==",
                _self = "dbs/x6MMAA==/colls/x6MMALDeYzw=/docs/x6MMALDeYzwBAAAAAAAAAA==/",
                _etag = "\"0e00e88a-0000-0700-0000-629d8ce11111\"",
                _attachments = "attachments/",
                _ts = "1654492385"
            };
            return changedEvent;
        }
        #endregion

        #region GenerateTypeBTelemetry
        internal static TypeBTelemetryModel GenerateTypeBTelemetry(short siteIndex)
        {
            var changedEvent = new TypeBTelemetryModel()
            {
                //deviceId_date = "TypeB-9999912301905" + "_" + DateTime.Now.ToString("MMddyyyy"),
                date = DateTime.Now.ToString("MMddyyyy"),
                deviceId = "TypeB-9999912301905",
                metadata = new DVDBenchmark.Model.Metadata()
                {
                    MessageId = "9441232962703822021665269069521950240730852893363",
                    MessageType = "device_readings",
                    MessageVersion = "1.3",
                    MessageTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    Manufacturer = new Manufacturer()
                    {
                        ManufacturerDeviceModel = "EOF-BIB/LT/24/24",
                        ManufacturerName = "TypeB",
                        ManufacturerSerialNumber = "9999912301905",
                        ManufacturerFamily = ""
                    },
                    Customer = new Customer()
                    {
                        CustomerName = "Left Fryer Vat",
                        CustomerDeviceClass = "TypeB",
                        CustomerGuid = "2019-002997"
                    }
                },
                header = new DVDBenchmark.Model.Headers()
                {
                    tenantid = "43f4b965-aa85-405a-43d7-08d958688822",
                    sourcetype = "EdgeModule",
                    siteid = string.Format("US-{0}", siteIndex.ToString()),//"US-5260",
                    edgedeviceid = "US-store-05260-01",
                    edgemoduleid = "telemetryreceiver-01"
                },
                type = "Telemetry",
                typeBdata = new FryerTelemetryData
                {
                    nod = "1",
                    timer1Direction = "0",
                    controlMode1 = "3",
                    controlMode2 = "0",
                    dataItemType = "0",
                    boardTemp = "78.8",
                    cookCycleNumber = "0",
                    timer1Seconds = "420",
                    addLevelDiff = "0",
                    heatContactorCounter1 = "1",
                    inAlarm = "1",
                    heatContactorCounter2 = "0",
                    net = "1",
                    realTimeSeconds = "213877354",
                    cookCyclesSinceFilter = "0",
                    cooktimeMinutes = "0",
                    heatingElementsOnMinutes = "11",
                    timer2Direction = "0",
                    idleTimeMinutes = "8",
                    addLevelProbeTemp = "0.0",
                    epochTimeSeconds = "217019999",
                    heatingElementTemp = "164.4",
                    uptimeMinutes = "20",
                    heatingElementsIdleMinutes = "0",
                    oilProbeTemp = "118.9",
                    timer2Seconds = "420",
                    heatingElementSetPoint = "350",
                    basketElevatorCycles2 = "1",
                    powerCycleID = "213876154",
                    heatingElementActiveMinutes = "11",
                    basketElevatorCycles1 = "1",
                    rightMenuItemString = "MANUAL",
                    leftMenuNumber = "0",
                    rightMenuNumber = "51",
                    leftMenuItemString = "MANUAL",
                    telemetryTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), //modified
                    localTimestamp = "2021-11-16T13:19:59-06:00"
                },
                id = Guid.NewGuid().ToString(), //modified
                _rid = "x6MMALDeYzwBAAAAAAAAAA==",
                _self = "dbs/x6MMAA==/colls/x6MMALDeYzw=/docs/x6MMALDeYzwBAAAAAAAAAA==/",
                _etag = "\"0f0039e9-0000-0700-0000-619404611111\"",
                _attachments = "attachments/",
                _ts = "1637098899"
            };
            return changedEvent;
        } 
        #endregion
    }
}

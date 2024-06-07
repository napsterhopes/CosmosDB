namespace DVDBenchmark.Model
{
    internal class OvenTelemetryModel
    {
        //public string deviceId_date { get; set; }
        public string date { get; set; }
        public string deviceId { get; set; }
        public Metadata metadata { get; set; }
        public Headers header { get; set; }
        public string type { get; set; }
        public OvenTelemetryData data { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public string _ts { get; set; }
    }


    internal class OvenTelemetryData
    {
        public string heatState { get; set; }
        public string doorState { get; set; }
        public string electricityConsumptionCalc { get; set; }
        public string rel_version { get; set; }
        public string waterConsumptionCalc { get; set; }
        public string overheatTime { get; set; }
        public string serial_nb { get; set; }
        public string rotateTime { get; set; }
        public string lightCount { get; set; }
        public string convecFanTime { get; set; }
        public string bakingTimeAuto { get; set; }
        public string liftDownTime { get; set; }
        public string errorType { get; set; }
        public string fanState { get; set; }
        public string model { get; set; }
        public string operatingState { get; set; }
        public string program_total_time { get; set; }
        public string CustomGUID { get; set; }
        public string ongoing_cycle { get; set; }
        public string energyType { get; set; }
        public string ventState { get; set; }
        public string bakingTimeManual { get; set; }
        public string ventTime { get; set; }
        public string gasConsumptionCalc { get; set; }
        public string uc_version { get; set; }
        public string rotateCount { get; set; }
        public string ventCount { get; set; }
        public string CustomName { get; set; }
        public string operatingTime { get; set; }
        public string liftDownCount { get; set; }
        public string recipeName { get; set; }
        public string cpuArchitecture { get; set; }
        public string heat_count { get; set; }
        public string steamCount { get; set; }
        public string cpuSwVersion { get; set; }
        public string lightTime { get; set; }
        public string hoodTime { get; set; }
        public string liftUpCount { get; set; }
        public string ovenTempSetPoint { get; set; }
        public string hoodState { get; set; }
        public string manufacturer { get; set; }
        public string totalStorage { get; set; }
        public string cpuManufacturer { get; set; }
        public string macAddress { get; set; }
        public string ovenTempCurrent { get; set; }
        public string program_elapsed_time { get; set; }
        public string steamTime { get; set; }
        public string hmi_version { get; set; }
        public string remainingServiceDays { get; set; }
        public string liftUpTime { get; set; }
        public string codeNumber { get; set; }
        public string steamState { get; set; }
        public string rackState { get; set; }
        public string osName { get; set; }
        public string numberOfEndOfCookingCycles { get; set; }
        public string totalMemory { get; set; }
        public string doorCount { get; set; }
        public string firmwareVersion { get; set; }
        public string hoodCount { get; set; }
        public string convecFanCount { get; set; }
        public string family { get; set; }
        public string heatTime { get; set; }
        public string telemetryTimestamp { get; set; }
        public string localTimestamp { get; set; }
    }

}

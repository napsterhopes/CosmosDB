namespace DVDBenchmark.Model
{
    internal class RotisserieTelemetryModel
    {
        //public string deviceId_date { get; set; }
        public string date { get; set; }
        public string deviceId { get; set; }
        public Metadata metadata { get; set; }
        public Headers header { get; set; }
        public string type { get; set; }
        public RotisserieTelemetryData data { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public string _ts { get; set; }
    }

    internal class RotisserieTelemetryData
    {
        public string ovenTempCurrent { get; set; }
        public string operationMinutes { get; set; }
        public string telemetryTimestamp { get; set; }
        public string localTimestamp { get; set; }
    }
}

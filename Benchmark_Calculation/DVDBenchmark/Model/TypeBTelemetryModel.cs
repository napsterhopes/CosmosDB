namespace DVDBenchmark.Model
{
    internal class TypeBTelemetryModel
    {
        //public string deviceId_date { get; set; }
        public string date { get; set; }
        public string deviceId { get; set; }
        public Metadata metadata { get; set; }
        public Headers header { get; set; }
        public string type { get; set; }
        public FryerTelemetryData typeBdata { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public string _ts { get; set; }
    }
}

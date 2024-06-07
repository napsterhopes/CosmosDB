namespace DVDBenchmark.Model
{
    internal class Metadata
    {
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public string MessageVersion { get; set; }
        public string MessageTimestamp { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Customer Customer { get; set; }
    }
}

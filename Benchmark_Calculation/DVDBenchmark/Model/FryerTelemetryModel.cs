
namespace DVDBenchmark.Model
{
    internal class FryerTelemetryModel
    {
        public string date { get; set; }
        public string deviceId { get; set; }
        public Metadata metadata { get; set; }
        public Headers header { get; set; }
        public string type { get; set; }
        public FryerTelemetryData data { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }
    }

    internal class FryerTelemetryData
    {
        public string nod { get; set; }
        public string timer1Direction { get; set; }
        public string controlMode1 { get; set; }
        public string controlMode2 { get; set; }
        public string dataItemType { get; set; }
        public string boardTemp { get; set; }
        public string cookCycleNumber { get; set; }
        public string timer1Seconds { get; set; }
        public string addLevelDiff { get; set; }
        public string heatContactorCounter1 { get; set; }
        public string inAlarm { get; set; }
        public string heatContactorCounter2 { get; set; }
        public string net { get; set; }
        public string realTimeSeconds { get; set; }
        public string cookCyclesSinceFilter { get; set; }
        public string cooktimeMinutes { get; set; }
        public string heatingElementsOnMinutes { get; set; }
        public string timer2Direction { get; set; }
        public string idleTimeMinutes { get; set; }
        public string addLevelProbeTemp { get; set; }
        public string epochTimeSeconds { get; set; }
        public string heatingElementTemp { get; set; }
        public string uptimeMinutes { get; set; }
        public string heatingElementsIdleMinutes { get; set; }
        public string oilProbeTemp { get; set; }
        public string timer2Seconds { get; set; }
        public string heatingElementSetPoint { get; set; }
        public string basketElevatorCycles2 { get; set; }
        public string powerCycleID { get; set; }
        public string heatingElementActiveMinutes { get; set; }
        public string basketElevatorCycles1 { get; set; }
        public string rightMenuItemString { get; set; }
        public string leftMenuNumber { get; set; }
        public string rightMenuNumber { get; set; }
        public string leftMenuItemString { get; set; }
        public string telemetryTimestamp { get; set; }
        public string localTimestamp { get; set; }
    }
}

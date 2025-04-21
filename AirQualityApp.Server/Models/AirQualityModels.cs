namespace AirQualityApp.Server.Models
{
    public class AirQualityCityData
    {
        public string City { get; set; }
        public DateTime Date { get; set; }
        public List<AirQualityAeraData> Areas { get; set; }
    }

    public class AirQualityAeraData
    {
        public string Area { get; set; }
        public int AreaId { get; set; }
        public DateTime Date { get; set; }
        public List<AirQualityNodeData> Nodes { get; set; }
    }

    public class AirQualityNodeData
    {
        public string Name { get; set; }

        public AirQuality AirQuality { get; set; }
    }

    public enum AirQualityLevel
    {
        Excellent,
        Good,
        LightlyPolluted,
        ModeratelyPolluted,
        HeavilyPolluted,
        SeverelyPolluted
    }

    public class AirQuality
    {
        public int? PM25 { get; set; }
        public int? PM10 { get; set; }
        public int? O3 { get; set; }
        public int? CO { get; set; }
        public int? SO2 { get; set; }
        public int? NO2 { get; set; }
        public int? AQI { get; set; }
        public AirQualityLevel? Quality { get; set; }
        public string? PrimaryPollutant { get; set; }
    }
}

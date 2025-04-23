namespace AirQualityApp.Shared.Models
{
#nullable disable

    public class AirQualityCityData
    {
        public CityInfo City { get; set; }

        public DateTime Date { get; set; }

        public List<AirQualityAreaData> Areas { get; set; }
    }

    public class AirQualityAreaData
    {
        public AreaInfo Area { get; set; }

        public DateTime Date { get; set; }

        public List<AirQualityNodeData> Nodes { get; set; }
    }

    public class AirQualityNodeData
    {
        public NodeInfo Node { get; set; }

        public AirQuality AirQuality { get; set; }
    }

#nullable restore

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

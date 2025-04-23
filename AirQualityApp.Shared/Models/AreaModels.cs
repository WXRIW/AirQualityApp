namespace AirQualityApp.Shared.Models
{
#nullable disable

    public interface IAreaInfo
    {
        public string Name { get; set; }
    }

    public class CityInfo : IAreaInfo
    {
        public CityInfo() { }

        public CityInfo(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class AreaInfo : IAreaInfo
    {
        public AreaInfo() { }

        public AreaInfo(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class NodeInfo : IAreaInfo
    {
        public NodeInfo() { }
        public NodeInfo(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}

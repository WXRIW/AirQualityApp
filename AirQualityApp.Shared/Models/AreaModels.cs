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

        public CityInfo(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        /// <summary>
        /// 城市名 (拼音，用于传递和存储)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名 (中文名)
        /// </summary>
        public string DisplayName { get; set; }
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

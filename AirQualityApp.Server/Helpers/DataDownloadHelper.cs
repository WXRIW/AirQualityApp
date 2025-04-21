using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AirQualityApp.Server.Models;

namespace AirQualityApp.Server.Helpers
{
    public static class DataDownloadHelper
    {
        private static readonly string SaveFolder = Path.Combine(AppContext.BaseDirectory, "Data");

        public static async Task DownloadLatest()
        {
            var data = await FetchLatestData();
            if (data != null)
            {
                SaveToJson(data);
            }
        }

        private static async Task<AirQualityCityData> FetchLatestData()
        {
            var urlTemplate = "https://link.sthj.sh.gov.cn/aqi/siteAqi/siteSubareaAqi.jsp?groupId={0}";
            var groupIds = new List<int>();
            for (int i = 201; i <= 216; i++) if (i != 207) groupIds.Add(i);

            var httpClient = new HttpClient();
            var cityData = new AirQualityCityData
            {
                City = "上海",
                Areas = new List<AirQualityAeraData>()
            };

            foreach (var groupId in groupIds)
            {
                var url = string.Format(urlTemplate, groupId);
                var html = await httpClient.GetStringAsync(url);

                var timeMatch = Regex.Match(html, @"<span class=""t-time fr"">([^<]+)</span>");
                if (!timeMatch.Success) continue;
                var parsedTime = ParseTimeLabel(timeMatch.Groups[1].Value);
                if (parsedTime == null) continue;

                if (cityData.Date == default)
                {
                    cityData.Date = parsedTime.Value;
                }

                var tableMatch = Regex.Match(html, @"<table id=""siteAqiList""[^>]*?>.*?<tbody>(.*?)</tbody>", RegexOptions.Singleline);
                if (!tableMatch.Success) continue;

                var rowMatches = Regex.Matches(tableMatch.Groups[1].Value, @"<tr.*?>(.*?)</tr>", RegexOptions.Singleline);

                var area = new AirQualityAeraData
                {
                    Area = $"区域{groupId}",
                    Date = parsedTime.Value,
                    Nodes = new List<AirQualityNodeData>()
                };

                foreach (Match row in rowMatches)
                {
                    var cellMatches = Regex.Matches(row.Groups[1].Value, @"<td[^>]*?>(.*?)</td>", RegexOptions.Singleline);
                    if (cellMatches.Count < 10) continue;

                    string Clean(string s) => Regex.Replace(s, "<.*?>", string.Empty).Trim();

                    var qualityStr = Clean(cellMatches[8].Value);
                    if (!Enum.TryParse(qualityStr, out AirQualityLevel quality))
                        quality = AirQualityLevel.未知;

                    var node = new AirQualityNodeData
                    {
                        Name = Clean(cellMatches[0].Value),
                        AirQuality = new AirQuality
                        {
                            PM25 = TryParseInt(Clean(cellMatches[1].Value)),
                            PM10 = TryParseInt(Clean(cellMatches[2].Value)),
                            O3 = TryParseInt(Clean(cellMatches[3].Value)),
                            CO = TryParseFloatToInt(Clean(cellMatches[4].Value)),
                            SO2 = TryParseInt(Clean(cellMatches[5].Value)),
                            NO2 = TryParseInt(Clean(cellMatches[6].Value)),
                            AQI = TryParseInt(Clean(cellMatches[7].Value)),
                            Quality = quality,
                            PrimaryPollutant = Clean(cellMatches[9].Value)
                        }
                    };

                    area.Nodes.Add(node);
                }

                cityData.Areas.Add(area);
            }

            return cityData;
        }

        private static void SaveToJson(AirQualityCityData data)
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            var timestamp = data.Date.ToString("yyyy-MM-dd-HH-mm");
            var filePath = Path.Combine(SaveFolder, $"Shanghai_AirQuality_{timestamp}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 防止中文转 Unicode
            };

            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        private static DateTime? ParseTimeLabel(string input)
        {
            var match = Regex.Match(input, @"(\d{2})月(\d{2})日\s+(\d{2}):(\d{2})");
            if (match.Success)
            {
                int month = int.Parse(match.Groups[1].Value);
                int day = int.Parse(match.Groups[2].Value);
                int hour = int.Parse(match.Groups[3].Value);
                int minute = int.Parse(match.Groups[4].Value);
                var now = DateTime.Now;
                return new DateTime(now.Year, month, day, hour, minute, 0);
            }
            return null;
        }

        private static int TryParseInt(string str) => int.TryParse(str, out int result) ? result : 9999;

        private static int TryParseFloatToInt(string str) => float.TryParse(str, out float f) ? (int)(f * 1000) : 9999;
    }
}
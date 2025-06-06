﻿using AirQualityApp.Shared.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AirQualityApp.Server.Helpers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    public static class DataDownloadHelper
    {
        private static readonly string SaveFolder = Path.Combine(AppContext.BaseDirectory, "Data");

        public static async Task DownloadLatest()
        {
            AirQualityCityData? data = null;
            int maxCount = 10;
            while (maxCount-- > 0)
            {
                try
                {
                    data = await FetchLatestData();
                }
                catch { }
                if (data != null)
                {
                    SaveToJson(data);
                    break;
                }
                else
                {
                    await Task.Delay(1000 * 10); // 等待 10 秒再试
                }
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
                City = new("上海"),
                Areas = [],
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

                var area = new AirQualityAreaData
                {
                    Area = new(groupId, ExtractAreaName(html)),
                    Date = parsedTime.Value,
                    Nodes = [],
                };

                static string ExtractAreaName(string html)
                {
                    var match = Regex.Match(html, "<span class=\"qy-name\">(.*?)</span>");
                    return match.Success ? match.Groups[1].Value.Trim() : "未知区域";
                }

                foreach (Match row in rowMatches)
                {
                    var cellMatches = Regex.Matches(row.Groups[1].Value, @"<td[^>]*?>(.*?)</td>", RegexOptions.Singleline);
                    if (cellMatches.Count < 10) continue;

                    static string Clean(string s) => Regex.Replace(s, "<.*?>", string.Empty).Trim();

                    var qualityStr = Clean(cellMatches[8].Value);
                    qualityStr = qualityStr switch
                    {
                        "优" => "Excellent",
                        "良" => "Good",
                        "轻度污染" => "LightlyPolluted",
                        "中度污染" => "ModeratelyPolluted",
                        "重度污染" => "HeavilyPolluted",
                        "严重污染" => "SeverelyPolluted",
                        _ => "Unknown"
                    };

                    AirQualityLevel? quality = null;
                    if (Enum.TryParse(qualityStr, out AirQualityLevel qualityLevel))
                        quality = qualityLevel;

                    var node = new AirQualityNodeData
                    {
                        Node = new(Clean(cellMatches[0].Value)),
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
                            PrimaryPollutant = string.IsNullOrWhiteSpace(Clean(cellMatches[9].Value)) || Clean(cellMatches[9].Value) == "-" ? null : Clean(cellMatches[9].Value)
                        }
                    };

                    area.Nodes.Add(node);
                }

                cityData.Areas.Add(area);
            }

            return cityData;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1869:Cache and reuse 'JsonSerializerOptions' instances", Justification = "<Pending>")]
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

        private static int? TryParseInt(string str) => int.TryParse(str, out int result) ? result : null;

        private static int? TryParseFloatToInt(string str) => float.TryParse(str, out float f) ? (int)(f * 1000) : null;
    }
}
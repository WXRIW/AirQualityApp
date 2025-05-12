using AirQualityApp.Shared.Models;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AirQualityApp.Server.Helpers
{
    /// <summary>
    /// 包含用于访问和处理空气质量数据文件的静态辅助方法。
    /// </summary>
    public static class AirQualityControllerHelper
    {

        // 数据存储的基础文件夹路径
        private static readonly string SaveFolder = Path.Combine(AppContext.BaseDirectory, "Data");
        // 用于从文件名提取日期时间戳的正则表达式
        private static readonly Regex FileNameRegex = new Regex(@"^Shanghai_AirQuality_(\d{4}-\d{2}-\d{2}-\d{2}-\d{2})\.json$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // JSON 序列化/反序列化选项
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // 忽略属性名大小写
        };

        /// <summary>
        /// 获取指定城市的数据文件存储路径 (目前仅支持上海)。
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <returns>数据存储目录的完整路径</returns>
        /// <exception cref="NotSupportedException">如果城市名称不是 "Shanghai" (忽略大小写)</exception>
        /// <exception cref="IOException">如果创建目录失败。</exception>
        public static string GetDataFolderPath(string cityName)
        {
            // 检查是否是支持的城市（目前仅上海）
            if (cityName.Equals("Shanghai", StringComparison.OrdinalIgnoreCase))
            {
                // 检查目录是否存在，不存在则创建
                if (!Directory.Exists(SaveFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(SaveFolder);
                    }
                    catch (Exception ex)
                    {
                        // 记录或处理目录创建失败的情况
                        Console.Error.WriteLine($"[助手错误] 创建数据目录 '{SaveFolder}' 时出错: {ex.Message}");
                        // 抛出一个更具体的异常或包装原始异常
                        throw new IOException($"创建数据目录失败: {ex.Message}", ex);
                    }
                }
                return SaveFolder;
            }
            // 不支持的城市
            throw new NotSupportedException($"城市 '{cityName}' 当前不受支持。");
        }

        /// <summary>
        /// 从给定的文件名解析出 DateTime 时间戳。
        /// </summary>
        /// <param name="fileName">要解析的文件名 (例如 "Shanghai_AirQuality_2023-10-27-10-00.json")</param>
        /// <returns>解析成功则返回 DateTime 对象，否则返回 null。</returns>
        public static DateTime? ParseTimestampFromFileName(string fileName)
        {
            // 使用正则匹配文件名
            var match = FileNameRegex.Match(Path.GetFileName(fileName));
            // 检查是否匹配成功且捕获组正确
            if (match.Success && match.Groups.Count > 1)
            {
                // 尝试精确解析日期时间字符串
                if (DateTime.TryParseExact(match.Groups[1].Value, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp))
                {
                    return timestamp;
                }
            }
            // 匹配失败或解析失败
            return null;
        }

        /// <summary>
        /// 获取指定城市符合条件的数据文件信息列表（包含文件路径和时间戳）。
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <param name="limitDays">限制返回数据的天数范围（基于文件时间戳的日期部分）。-1 表示返回所有找到的文件。</param>
        /// <returns>按时间戳降序排列的文件信息元组列表 (FilePath, Timestamp)。</returns>
        public static List<(string FilePath, DateTime Timestamp)> GetCityDataFiles(string cityName, int limitDays = -1)
        {
            string dataFolder;
            try
            {
                // 获取数据文件夹路径
                dataFolder = GetDataFolderPath(cityName);
            }
            catch (NotSupportedException) { return []; } // 城市不支持
            catch (IOException) { return []; } // 获取或创建文件夹失败

            List<(string FilePath, DateTime Timestamp)> fileInfos = [];
            try
            {
                // 获取目录下匹配模式的文件
                var files = Directory.GetFiles(dataFolder, $"{cityName}_AirQuality_*.json", SearchOption.TopDirectoryOnly);
                // 遍历文件并解析时间戳
                foreach (var file in files)
                {
                    var timestamp = ParseTimestampFromFileName(file);
                    if (timestamp.HasValue)
                    {
                        fileInfos.Add((file, timestamp.Value));
                    }
                }
            }
            catch (Exception ex) // 捕获列出文件时可能发生的异常 (如目录不存在, IO错误等)
            {
                Console.Error.WriteLine($"[助手错误] 列出城市 '{cityName}' 的文件时出错: {ex.Message}");
                return []; // 出错时返回空列表
            }

            // 按时间戳降序排序 
            fileInfos.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));


            if (limitDays > 0)
            {
                // 计算起始日期阈值
                var thresholdDate = DateTime.Now.Date.AddDays(-(limitDays - 1));
                // 筛选出日期大于等于阈值的文件
                fileInfos = fileInfos.Where(f => f.Timestamp.Date >= thresholdDate).ToList();
            }

            return fileInfos;
        }

        /// <summary>
        /// (同步) 从指定文件路径读取并反序列化城市空气质量数据。
        /// 注意：在 Web API 中使用同步 I/O 会阻塞线程，建议使用异步版本。
        /// </summary>
        /// <param name="filePath">JSON 文件的完整路径。</param>
        /// <returns>反序列化成功则返回 AirQualityCityData 对象，否则返回 null。</returns>
        public static AirQualityCityData? ReadCityData(string filePath)
        {
            try
            {
                // 检查文件是否存在，不存在则返回 null
                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }
                // 同步读取文件内容
                var json = System.IO.File.ReadAllText(filePath);
                // 反序列化 JSON
                return JsonSerializer.Deserialize<AirQualityCityData>(json, _jsonOptions);
            }
            catch (JsonException jsonEx) // JSON 解析错误
            {
                Console.Error.WriteLine($"[助手错误] 文件 '{filePath}' 的 JSON 反序列化错误: {jsonEx.Message}");
                return null;
            }
            catch (IOException ioEx) // 文件读取错误
            {
                Console.Error.WriteLine($"[助手错误] 读取文件 '{filePath}' 时出错: {ioEx.Message}");
                return null;
            }
            catch (Exception ex) // 其他意外错误
            {
                Console.Error.WriteLine($"[助手错误] 读取/解析文件 '{filePath}' 时发生意外错误: {ex.Message}");
                return null;
            }
        }

    }
}

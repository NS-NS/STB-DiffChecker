using DiffCheckerLib;
using DiffCheckerLib.Interface;
using System.Reflection;

namespace DiffCheckerWeb
{
    /// <summary>
    /// ST-Bridgeバージョンごとの読込み・設定・比較エンジンをまとめるクラス
    /// </summary>
    public class VersionEngine
    {
        public required string Version { get; init; }
        public required IImportanceSetting Importance { get; init; }
        public required IToleranceSetting Tolerance { get; init; }
        public required Func<Stream, string, (IST_BRIDGE stb, List<string> errors)> Loader { get; init; }

        public static readonly IReadOnlyList<string> SupportedVersions = ["2.0.1", "2.0.2", "2.1.0"];

        public (IST_BRIDGE stb, List<string> errors) LoadFile(Stream stream)
        {
            return Loader(stream, Importance.GetSchemaContent());
        }

        public static VersionEngine? Create(string version)
        {
            return version switch
            {
                "2.0.1" => new VersionEngine
                {
                    Version = version,
                    Importance = new STB_DiffChecker_201.ImportanceSetting(),
                    Tolerance = new STB_DiffChecker_201.ToleranceSetting(),
                    Loader = (stream, schema) =>
                    {
                        ST_BRIDGE201.ST_BRIDGE stb = XmlValidate.LoadSTBridgeFile(stream, schema, new ST_BRIDGE201.ST_BRIDGE(), out List<string> errors);
                        return (stb, errors);
                    },
                },
                "2.0.2" => new VersionEngine
                {
                    Version = version,
                    Importance = new STB_DiffChecker_202.ImportanceSetting(),
                    Tolerance = new STB_DiffChecker_202.ToleranceSetting(),
                    Loader = (stream, schema) =>
                    {
                        ST_BRIDGE202.ST_BRIDGE stb = XmlValidate.LoadSTBridgeFile(stream, schema, new ST_BRIDGE202.ST_BRIDGE(), out List<string> errors);
                        return (stb, errors);
                    },
                },
                "2.1.0" => new VersionEngine
                {
                    Version = version,
                    Importance = new STB_DiffChecker_210.ImportanceSetting(),
                    Tolerance = new STB_DiffChecker_210.ToleranceSetting(),
                    Loader = (stream, schema) =>
                    {
                        ST_BRIDGE210.ST_BRIDGE stb = XmlValidate.LoadSTBridgeFile(stream, schema, new ST_BRIDGE210.ST_BRIDGE(), out List<string> errors);
                        return (stb, errors);
                    },
                },
                _ => null,
            };
        }

        /// <summary>
        /// タブ名に対応する要素数を取得する（概要表示用、実体はDiffCheckerCoreの共通実装）
        /// </summary>
        public static int CountElements(IST_BRIDGE? stb, string tabName)
        {
            return ElementCounter.Count(stb, tabName);
        }
    }
}

using DiffCheckerLib;
using DiffCheckerLib.Interface;
using System.Text;

namespace STB_DiffChecker
{
    /// <summary>
    /// ST-Bridgeバージョンごとの読込み・設定をまとめるクラス(デスクトップ版)
    /// Web版のVersionEngineと同じ考え方で、ファイルのversion属性から自動選択する
    /// </summary>
    public class DesktopVersionEngine
    {
        public required string Version { get; init; }
        public required IImportanceSetting Importance { get; init; }
        public required Func<IToleranceSetting> ToleranceFactory { get; init; }
        public required Func<IImportanceSetting> ImportanceFactory { get; init; }
        public required Func<string, Encoding, string, (IST_BRIDGE stb, List<string> errors)> Loader { get; init; }

        public IToleranceSetting Tolerance { get; private set; } = null!;

        public static readonly IReadOnlyList<string> SupportedVersions = ["2.0.1", "2.0.2", "2.1.0"];

        public (IST_BRIDGE stb, List<string> errors) LoadFile(string path, Encoding encoding)
        {
            return Loader(path, encoding, Importance.GetSchemaContent());
        }

        public static DesktopVersionEngine? Create(string version)
        {
            DesktopVersionEngine? engine = version switch
            {
                "2.0.1" => new DesktopVersionEngine
                {
                    Version = version,
                    Importance = new STB_DiffChecker_201.ImportanceSetting(),
                    ToleranceFactory = () => new STB_DiffChecker_201.ToleranceSetting(),
                    ImportanceFactory = () => new STB_DiffChecker_201.ImportanceSetting(),
                    Loader = (path, encoding, schema) =>
                    {
                        ST_BRIDGE201.ST_BRIDGE stb = XmlValidate.LoadSTBridgeFile(path, encoding, schema, new ST_BRIDGE201.ST_BRIDGE(), out List<string> errors);
                        return (stb, errors);
                    },
                },
                "2.0.2" => new DesktopVersionEngine
                {
                    Version = version,
                    Importance = new STB_DiffChecker_202.ImportanceSetting(),
                    ToleranceFactory = () => new STB_DiffChecker_202.ToleranceSetting(),
                    ImportanceFactory = () => new STB_DiffChecker_202.ImportanceSetting(),
                    Loader = (path, encoding, schema) =>
                    {
                        ST_BRIDGE202.ST_BRIDGE stb = XmlValidate.LoadSTBridgeFile(path, encoding, schema, new ST_BRIDGE202.ST_BRIDGE(), out List<string> errors);
                        return (stb, errors);
                    },
                },
                "2.1.0" => new DesktopVersionEngine
                {
                    Version = version,
                    Importance = new STB_DiffChecker_210.ImportanceSetting(),
                    ToleranceFactory = () => new STB_DiffChecker_210.ToleranceSetting(),
                    ImportanceFactory = () => new STB_DiffChecker_210.ImportanceSetting(),
                    Loader = (path, encoding, schema) =>
                    {
                        ST_BRIDGE210.ST_BRIDGE stb = XmlValidate.LoadSTBridgeFile(path, encoding, schema, new ST_BRIDGE210.ST_BRIDGE(), out List<string> errors);
                        return (stb, errors);
                    },
                },
                _ => null,
            };

            if (engine != null)
            {
                engine.Tolerance = engine.ToleranceFactory();
            }

            return engine;
        }
    }
}

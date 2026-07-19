using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Text;

// STB-DiffCheckerのヘッドレス動作検証ハーネス
// GUIを起動せずに比較エンジンを実行し、タブ別集計と全レコードのダンプ(回帰比較用)を出力する
//
// 使い方:
//   DiffCheckerHarness <201|202|210> <fileA.stb> <fileB.stb> [許容差(mm)=0] [ダンプ出力パス]
//
// 例:
//   DiffCheckerHarness 202 TestData\FileA.stb TestData\FileB.stb
//   DiffCheckerHarness 210 A.stb B.stb 100 result.tsv

if (args.Length < 3)
{
    Console.Error.WriteLine("使い方: DiffCheckerHarness <201|202|210> <fileA.stb> <fileB.stb> [許容差(mm)=0] [ダンプ出力パス]");
    return 2;
}

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

string mode = args[0];
string pathA = args[1];
string pathB = args[2];
double tolerance = args.Length > 3 ? double.Parse(args[3]) : 0;
string? dumpPath = args.Length > 4 ? args[4] : null;

IImportanceSetting importance;
IToleranceSetting toleranceSetting;
IST_BRIDGE stbA, stbB;
List<string> errorsA, errorsB;

switch (mode)
{
    case "201":
    {
        STB_DiffChecker_201.ImportanceSetting imp = new();
        string schema = imp.GetSchemaContent();
        stbA = XmlValidate.LoadSTBridgeFile(pathA, Encoding.UTF8, schema, new ST_BRIDGE201.ST_BRIDGE(), out errorsA);
        stbB = XmlValidate.LoadSTBridgeFile(pathB, Encoding.UTF8, schema, new ST_BRIDGE201.ST_BRIDGE(), out errorsB);
        importance = imp;
        toleranceSetting = new STB_DiffChecker_201.ToleranceSetting();
        break;
    }
    case "202":
    {
        STB_DiffChecker_202.ImportanceSetting imp = new();
        string schema = imp.GetSchemaContent();
        stbA = XmlValidate.LoadSTBridgeFile(pathA, Encoding.UTF8, schema, new ST_BRIDGE202.ST_BRIDGE(), out errorsA);
        stbB = XmlValidate.LoadSTBridgeFile(pathB, Encoding.UTF8, schema, new ST_BRIDGE202.ST_BRIDGE(), out errorsB);
        importance = imp;
        toleranceSetting = new STB_DiffChecker_202.ToleranceSetting();
        break;
    }
    case "210":
    {
        STB_DiffChecker_210.ImportanceSetting imp = new();
        string schema = imp.GetSchemaContent();
        stbA = XmlValidate.LoadSTBridgeFile(pathA, Encoding.UTF8, schema, new ST_BRIDGE210.ST_BRIDGE(), out errorsA);
        stbB = XmlValidate.LoadSTBridgeFile(pathB, Encoding.UTF8, schema, new ST_BRIDGE210.ST_BRIDGE(), out errorsB);
        importance = imp;
        toleranceSetting = new STB_DiffChecker_210.ToleranceSetting();
        break;
    }
    default:
        Console.Error.WriteLine($"未対応のモード: {mode} (201/202/210のいずれかを指定)");
        return 2;
}

foreach (UserTolerance tol in toleranceSetting.Tolerances())
{
    tol.Node = tolerance;
    tol.Offset = tolerance;
}

Console.WriteLine($"validation errors: A={errorsA.Count} B={errorsB.Count}");
foreach (string e in errorsA.Take(10)) { Console.WriteLine($"  A: {e}"); }
foreach (string e in errorsB.Take(10)) { Console.WriteLine($"  B: {e}"); }

List<string> targetNames = importance.GetTabs().Select(n => n.Item1).ToList();
System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
Dictionary<string, List<Record>> results = ObjectComparer.CompareSTBridgeFiles(
    stbA, stbB, targetNames, importance.UserImportance(), toleranceSetting);
sw.Stop();
Console.WriteLine($"compare time: {sw.ElapsedMilliseconds} ms");
Console.WriteLine($"total records: {results.Values.Sum(r => r.Count)}");

foreach ((string root, string header) in importance.GetTabs())
{
    if (!results.TryGetValue(root, out List<Record>? records)) { continue; }
    int c = records.Count(r => r.Consistency == Consistency.Consistent);
    int a = records.Count(r => r.Consistency == Consistency.AlmostMatch);
    int i = records.Count(r => r.Consistency == Consistency.Inconsistent);
    int n = records.Count(r => r.Consistency is Consistency.Incomparable or Consistency.ElementIncomparable);
    Console.WriteLine($"[{header}] 一致={c} 許容差内={a} 不一致={i} 対象なし={n}");
}

if (!string.IsNullOrEmpty(dumpPath))
{
    StringBuilder sb = new();
    foreach (string tab in results.Keys.OrderBy(k => k, StringComparer.Ordinal))
    {
        foreach (Record r in results[tab])
        {
            sb.AppendLine($"{tab}\t{r.XmlPath}\t{r.Key}\t{r.Item}\t{r.A}\t{r.B}\t{r.Consistency}\t{r.Importance}");
        }
    }
    File.WriteAllText(dumpPath, sb.ToString(), new UTF8Encoding(false));
    Console.WriteLine($"dumped: {dumpPath}");
}

return 0;

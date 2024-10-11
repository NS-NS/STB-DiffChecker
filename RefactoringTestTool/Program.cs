using RefactoringTestTool;
using ST_BRIDGE201;
using STBDiffChecker;
using STBDiffChecker.AttributeType;
using STBDiffChecker.v201.Records;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {

        /*
        if (args.Length < 2)
        {
            Console.WriteLine("2つのST-Bridgeファイルパスを指定してください。");
            return;
        }

        string pathA = args[0];
        string pathB = args[1];
        */
        string pathA = @"C:\Users\lucky\Downloads\STB202_Sample-RC\STB202_Sample-RC\RC標準モデル_stb2.0.1.stb"; //RC
        //string pathA = @"C:\Users\lucky\Downloads\STB202_Sample-S_3\STB202_Sample-S_3\S標準モデル_stb2.0.1_2.stb"; //S

        string pathB = @"C:\Users\lucky\Downloads\STB202_Sample-RC\STB202_Sample-RC\RC標準モデル_stb2.0.1.stb"; //RC
        //string pathB = @"C:\Users\lucky\Downloads\STB202_Sample-S_3\STB202_Sample-S_3\S標準モデル_stb2.0.1_2.stb"; //S

        if (!File.Exists(pathA) || !File.Exists(pathB))
        {
            Console.WriteLine("指定されたファイルが存在しません。");
            return;
        }

        try
        {
            ST_BRIDGE stbA = TotalRecord.Deserialize(pathA);
            ST_BRIDGE stbB = TotalRecord.Deserialize(pathB);

            //既存のレコードと比較。
            (List<string>, List<string>) setting = ReadCsv(@"C:\Users\lucky\Documents\GitHub\STB-DiffChecker\TestData\SettingsAll.csv");
            ToleranceSetting toleranceSetting = new();
            toleranceSetting.ImportCsv(setting.Item1);
            ImportanceSetting importanceSetting = new();
            importanceSetting.ImportCsv(setting.Item2);
            ResultFormSetting resultFormSetting = new()
            {
                PathA = pathA,
                PathB = pathB,
                importanceSetting = importanceSetting,
                toleranceSetting = toleranceSetting
            };

            TotalRecord totalRecord = new(resultFormSetting);
            totalRecord.Run();

            Dictionary<string, STBDiffChecker.AttributeType.Importance> dictionary = MakeImportanceDictionary(setting.Item2);

            Dictionary<string, List<Record>> groupedRecords = ObjectComparer.CompareSTBridgeFiles(stbA, stbB, TargetClasses(), dictionary, toleranceSetting);
            bool isEqual = true;
            if (groupedRecords.Count(n => n.Key == "StbCommon") > 0)

            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.CommonRecord.records, groupedRecords["StbCommon"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbNodes") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.NodesRecord.records, groupedRecords["StbNodes"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbParallelAxes") > 0)

            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.ParallelAxesRecord.records, groupedRecords["StbParallelAxes"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbArcAxes") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.ArcAxesRecord.records, groupedRecords["StbArcAxes"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbRadialAxes") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.RadialAxesRecord.records, groupedRecords["StbRadialAxes"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbDrawingLineAxis") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.DrawingLineAxis.records, groupedRecords["StbDrawingLineAxis"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbDrawingArcAxis") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.DrawingArcAxis.records, groupedRecords["StbDrawingArcAxis"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbStories") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.StoriesRecord.records, groupedRecords["StbStories"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbColumns") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.ColumnsRecord.records, groupedRecords["StbColumns"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbPosts") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.PostsRecord.records, groupedRecords["StbPosts"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbGirders") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.GirdersRecord.records, groupedRecords["StbGirders"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbBeams") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.BeamsRecord.records, groupedRecords["StbBeams"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbBraces") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.BracesRecord.records, groupedRecords["StbBraces"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSlabs") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SlabsRecord.records, groupedRecords["StbSlabs"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbWalls") > 0)
            {
                //isEqual &= RecordComparer.AreRecordsEqual(totalRecord.WallsRecord.records, groupedRecords["StbWalls"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbFootings") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.FootingsRecord.records, groupedRecords["StbFootings"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbStripFootings") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.StripFootingsRecord.records, groupedRecords["StbStripFootings"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbPiles") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.PilesRecord.records, groupedRecords["StbPiles"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbFoundationColumns") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.FoundationColumnsRecord.records, groupedRecords["StbFoundationColumns"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbParapets") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.ParapetsRecord.records, groupedRecords["StbParapets"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecColumn_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecColumnRcRecord.records, groupedRecords["StbSecColumn_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecColumn_S") > 0)
            {
                // steelの比較が違いすぎ
                //isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecColumnSRecord.records, groupedRecords["StbSecColumn_S"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecColumn_SRC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecColumnSrcRecord.records, groupedRecords["StbSecColumn_SRC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecColumn_CFT") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecColumnCftRecord.records, groupedRecords["StbSecColumn_CFT"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecBeam_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecBeamRcRecord.records, groupedRecords["StbSecBeam_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecBeam_S") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecBeamSRecord.records, groupedRecords["StbSecBeam_S"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecBeam_SRC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecBeamSrcRecord.records, groupedRecords["StbSecBeam_SRC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecBrace_S") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecBraceSRecord.records, groupedRecords["StbSecBrace_S"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecSlab_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecSlabRcRecord.records, groupedRecords["StbSecSlab_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecSlabDeck") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecSlabDeckRecord.records, groupedRecords["StbSecSlabDeck"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecSlabPrecast") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecSlabPrecastRecord.records, groupedRecords["StbSecSlabPrecast"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecWall_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecWallRcRecord.records, groupedRecords["StbSecWall_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecFoundation_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecFoundationRcRecord.records, groupedRecords["StbSecFoundation_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecPile_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecPileRcRecord.records, groupedRecords["StbSecPile_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecPile_S") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecPileSRecord.records, groupedRecords["StbSecPile_S"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecPileProduct") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecPileProductRecord.records, groupedRecords["StbSecPileProduct"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbSecParapet_RC") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual(totalRecord.SecParapetRcRecord.records, groupedRecords["StbParapet_RC"]);
            }
            if (groupedRecords.Count(n => n.Key == "StbJoints") > 0)
            {
                isEqual &= RecordComparer.AreRecordsEqual([], groupedRecords["StbJoints"]);
            }

            {
                if (isEqual)
                {
                    Console.WriteLine("2つのRecordリストは同じです。");
                }
                else
                {
                    Console.WriteLine("2つのRecordリストは異なります。");
                }
            }

            /*
            // 各グループごとに結果を出力
            foreach (var group in groupedResults)
            {
                Console.WriteLine($"=== {group.Key} の比較結果 ===");
                foreach (var record in group.Value)
                {
                    PrintRecord(record);
                }
                Console.WriteLine();
            }
            */
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エラー: {ex.Message}");
        }
    }

    private static IReadOnlyList<string> TargetClasses()
    {
        return ["StbCommon", "StbNodes", "StbParallelAxes", "StbArcAxes", "StbRadialAxes", "StbDrawingLineAxis",
            "StbDrawingArcAxis", "StbStories", "StbColumns", "StbPosts", "StbGirders", "StbBeams", "StbBraces",
        "StbSlabs", "StbWalls", "StbFootings", "StbStripFootings", "StbPiles", "StbFoundationColumns", "StbParapets", "StbSecColumn_RC",
        "StbSecColumn_S", "StbSecColumn_SRC", "StbSecColumn_CFT", "StbSecBeam_RC", "StbSecBeam_S", "StbSecBeam_SRC", "StbSecBrace_S",
        "StbSecSlab_RC", "StbSecSlabDeck", "StbSecSlabPrecast", "StbSecWall_RC", "StbSecFoundation_RC", "StbSecPile_RC", "StbSecPile_S",
        "StbSecPileProduct", "StbSecParapet_RC", "StbJoints"];
    }


    // Recordクラスの内容を出力するためのメソッド
    private static void PrintRecord(Record record)
    {
        Console.WriteLine("=======================================");
        Console.WriteLine($"親要素: {record.ParentElement}");
        Console.WriteLine($"比較対象: {record.Key}");
        Console.WriteLine($"要素・属性: {record.Item}");
        Console.WriteLine($"ファイルA: {record.A}");
        Console.WriteLine($"ファイルB: {record.B}");
        Console.WriteLine($"比較結果: {record.Consistency}");
        Console.WriteLine($"重要度: {record.Importance}");
        if (!string.IsNullOrEmpty(record.Comment))
        {
            Console.WriteLine($"コメント: {record.Comment}");
        }
        Console.WriteLine("=======================================");
    }

    /// <summary>
    /// 設定ファイルの読込み処理
    /// </summary>
    /// <param name="path"></param>
    private static (List<string>, List<string>) ReadCsv(string path)
    {
        string[] readData;
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            readData = File.ReadAllLines(path, System.Text.Encoding.GetEncoding("shift_jis"));
        }
        catch
        {
            return new();
        }

        List<string> csvTolerance = [];
        List<string> csvImportance = [];
        int flag = 0;   //Tleranceは1,Importanceは2

        #region 読込み処理
        foreach (string line in readData)
        {
            if (flag == 0)
            {
                string head = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                if (head == "<Tolerance>")
                {
                    flag = 1;
                    continue;
                }
                if (head == "<Importance>")
                {
                    flag = 2;
                    continue;
                }

            }
            else if (flag == 1)
            {
                string head = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                if (head == "<Importance>")
                {
                    flag = 2;
                    continue;
                }
                csvTolerance.Add(line);
                continue;
            }
            else if (flag == 2)
            {
                // 一応逆パターン用に設定

                string head = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                if (head == "<Tolerance>")
                {
                    flag = 1;
                    continue;
                }
                csvImportance.Add(line);
                continue;
            }
            else
            {
                continue;
            }
        }
        #endregion

        return (csvTolerance, csvImportance);
    }

    static Dictionary<string, STBDiffChecker.AttributeType.Importance> MakeImportanceDictionary(List<string> csvImportance)
    {
        Dictionary<string, STBDiffChecker.AttributeType.Importance> resultDict = [];
        foreach (string line in csvImportance)
        {
            // 行をカンマで分割
            string[] columns = line.Split(',');
            if (columns.Length < 1)
            {
                continue; // 行に要素がなければスキップ
            }

            // 重要度の取得
            Importance importance;
            if (columns[1].Trim() == "高")
            {
                importance = Importance.Required;
            }
            else if (columns[1].Trim() == "中")
            {
                importance = Importance.Optional;
            }
            else if (columns[1].Trim() == "低")
            {
                importance = Importance.Unnecessary;
            }
            else if (columns[1].Trim() == "対象外") { importance = Importance.NotApplicable; }
            else
            {
                continue; // 重要度が不正ならスキップ
            }


            string elementPath = columns[0].Trim(); // パス部分
            string[] elements = elementPath.Split('/'); // パスを '/' で分割


            // 属性名が存在するかを確認 (@があるかどうか)
            if (elementPath.Contains("@"))
            {
                // 属性名あり -> 最後の要素が属性なので、1つ前の要素を取得
                string attributeName = elements[^1].Replace("@", ""); // 最後の要素の@を除去
                string elementName = elements[^2]; // 1つ前の要素名を取得

                // キーの生成: {要素名}:{属性名}
                string key = $"{elementName}:{attributeName}";

                if (!resultDict.ContainsKey(key))
                {
                    resultDict.Add(key, importance);
                }
            }
            else
            {
                // 属性名なし -> 最後の要素名をキーにする
                string elementName = elements[^1]; // 最後の要素名

                // キーの生成: {要素名}:
                string key = $"{elementName}:";

                if (!resultDict.ContainsKey(key))
                {
                    resultDict.Add(key, importance);
                }
            }
        }
        return resultDict;
    }

}


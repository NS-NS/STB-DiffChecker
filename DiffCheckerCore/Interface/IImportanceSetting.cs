using DiffCheckerLib.Enum;
using System;
using System.Collections.Generic;
using System.Data;

#nullable enable annotations

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// ST_BRIDGEの要素に重要度を設定するためのクラス
    /// </summary>
    public interface IImportanceSetting
    {
        /// <summary>
        /// 同梱している重要度プリセット(設計段階プロファイル)の名前一覧
        /// </summary>
        public static readonly IReadOnlyList<string> PresetNames = ["S2", "S4"];

        public string GetSchemaContent();

        /// <summary>
        /// 同梱プリセットCSV(設定ファイルと同形式)の内容を取得する
        /// </summary>
        /// <returns>プリセットCSVの全文。該当バージョンに同梱が無ければnull</returns>
        public string? GetPresetCsv(string name);

        /// <summary>
        /// タブにまとめる情報を取得する
        /// </summary>
        /// <returns>ルートにするST_BRIDGEの要素名、ヘッダー名</returns>
        public abstract IReadOnlyList<(string, string)> GetTabs();

        /// <summary>
        /// XMLパスとユーザー設定の重要度の紐づけ用Dictionary
        /// </summary>
        public abstract Dictionary<string, Importance> UserImportance();

        /// <summary>
        /// 表示用にDictionaryのXMLパス順序を保持
        /// </summary>
        public abstract IReadOnlyList<string> OrderedImportance();

        /// <summary>
        /// 比較結果からテーブルを作成する
        /// </summary>
        public DataTable CreateTable()
        {
            DataTable importanceTable = new();
            DataColumn name = importanceTable.Columns.Add("StbName", typeof(string));
            name.ReadOnly = true;
            DataColumn importance = importanceTable.Columns.Add("Importance", typeof(string));
            importance.ReadOnly = true;

            foreach (string path in OrderedImportance())
            {
                _ = importanceTable.Rows.Add(path, UserImportance()[path].ToJapanese());
            }
            return importanceTable;
        }

        /// <summary>
        /// csvを読み込む
        /// </summary>
        /// <returns>未対応だったXMLパスの一覧</returns>
        public List<string> ImportCsv(IReadOnlyList<string> csv)
        {
            List<string> unknownPaths = new();
            foreach (string line in csv)
            {
                string[] split = line.Split(',');
                try
                {
                    if (UserImportance().ContainsKey(split[0]))
                    {
                        if (split[1].Trim() == Importance.Required.ToJapanese())
                        {
                            UserImportance()[split[0]] = Importance.Required;
                            continue;
                        }
                        else if (split[1].Trim() == Importance.Optional.ToJapanese())
                        {
                            UserImportance()[split[0]] = Importance.Optional;
                            continue;
                        }
                        if (split[1].Trim() == Importance.Unnecessary.ToJapanese())
                        {
                            UserImportance()[split[0]] = Importance.Unnecessary;
                            continue;
                        }
                        if (split[1].Trim() == Importance.NotApplicable.ToJapanese())
                        {
                            UserImportance()[split[0]] = Importance.NotApplicable;
                            continue;
                        }
                    }
                    unknownPaths.Add(split[0]);
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }
            return unknownPaths;
        }

    }
}
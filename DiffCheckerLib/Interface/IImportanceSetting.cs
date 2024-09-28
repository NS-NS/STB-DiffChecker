using DiffCheckerLib.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// ST_BRIDGEの要素に重要度を設定するためのクラス
    /// </summary>
    public interface IImportanceSetting
    {
        public string GetSchemaContent();

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
        public void ImportCsv(IReadOnlyList<string> csv)
        {
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
                    _ = MessageBox.Show($"未対応の{split[0]}があります。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }
        }

    }
}
using DiffCheckerLib.Setting;
using System;
using System.Collections.Generic;
using System.Data;

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// 許容差の設定を行うインターフェース
    /// バージョンごとに許容差が必要な要素が異なることを想定
    /// </summary>
    public interface IToleranceSetting
    {
        /// <summary>
        /// 許容差リストを取得する
        /// </summary>
        List<UserTolerance> Tolerances();

        /// <summary>
        /// 許容差表示用のテーブルを作成する
        /// </summary>
        public DataTable CreateTable()
        {
            DataTable toleranceTable = new();
            DataColumn name = toleranceTable.Columns.Add("Name", typeof(string));
            name.ReadOnly = true;
            _ = toleranceTable.Columns.Add("Node", typeof(double));
            _ = toleranceTable.Columns.Add("Offset", typeof(double));
            foreach (UserTolerance tolerance in Tolerances())
            {
                _ = toleranceTable.Rows.Add(tolerance.Name, tolerance.Node, tolerance.Offset);
            }

            return toleranceTable;
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
                    foreach (UserTolerance tolerance in Tolerances())
                    {
                        if (tolerance.Name == split[0])
                        {
                            tolerance.Node = double.Parse(split[1]);
                            tolerance.Offset = double.Parse(split[2]);
                            break;
                        }
                    }
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }
        }

    }
}

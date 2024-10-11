using DiffCheckerLib.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace DiffCheckerLib
{
    public class Summary : RecordTab
    {
        public static new readonly string Header = "概要";
        public Summary() : base(Header)
        {
            dateTime = DateTime.Now;
        }

        public void SetRow()
        {
            this.dataTable = this.CreateSummaryTable();

            //DataGridの定義
            DataGrid dataGrid = new()
            {
                Name = Header,
                CanUserAddRows = false,
                CanUserSortColumns = false,
                ItemsSource = this.dataTable.DefaultView
            };
            dataGrid.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.Content = dataGrid;
        }

        public DateTime dateTime;
        public static readonly List<string> HeaderText =
            [
            "項目",
            "要素数(ファイルA)",
            "要素数(ファイルB)",
            Consistency.Consistent.ToJapanese(),
            Consistency.AlmostMatch.ToJapanese(),
            Consistency.Inconsistent.ToJapanese(),
            Consistency.Incomparable.ToJapanese(),
            Consistency.ElementIncomparable.ToJapanese()
            ];

        public List<List<string>> Rows = [];

        public List<string> CollectResult(string headear, int countA, int countB, List<Record> records)
        {
            return [
                headear,
                countA.ToString(),
                countB.ToString(),
                records.Count(n => n.Consistency == Consistency.Consistent).ToString(),
                records.Count(n => n.Consistency == Consistency.AlmostMatch).ToString(),
                records.Count(n => n.Consistency == Consistency.Inconsistent).ToString(),
                records.Count(n => n.Consistency == Consistency.Incomparable).ToString(),
                records.Count(n => n.Consistency == Consistency.ElementIncomparable).ToString()
            ];
        }

        /// <summary>
        /// SummaryをDataTableへ出力
        /// </summary>
        private DataTable CreateSummaryTable()
        {
            DataTable table = new();
            foreach (string header in Summary.HeaderText)
            {
                DataColumn column = table.Columns.Add(header, typeof(string));
                column.ReadOnly = true;
            }

            foreach (List<string> row in this.Rows)
            {
                _ = table.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7]);
            }

            return table;
        }

    }
}

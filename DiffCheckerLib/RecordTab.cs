using DiffCheckerLib.Enum;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace DiffCheckerLib
{
    public class RecordTab : TabItem
    {
        public DataTable dataTable { get; protected set; }
        public List<Record> records { get; private set; }
        public void SetRecord(List<DiffCheckerLib.Record> record)
        {
            if (record == null)
            {
                return;
            }
            records = record;
            dataTable = CreateTable();

            //DataGridの定義
            DataGrid dataGrid = new()
            {
                CanUserAddRows = false,
                CanUserSortColumns = false,
                ItemsSource = this.dataTable.DefaultView
            };
            dataGrid.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.Content = dataGrid;
        }
        public RecordTab(string headerName)
        {
            Header = headerName;
        }

        protected void dataGrid_AutoGeneratingColumn(object s, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Record.JapaneseParentElement:
                    e.Column.Header = Record.JapaneseParentElement;
                    break;
                case Record.JapaneseKey:
                    e.Column.Header = Record.JapaneseKey;
                    break;
                case Record.JapaneseItem:
                    e.Column.Header = Record.JapaneseItem;
                    break;
                case Record.JapaneseA:
                    e.Column.Header = Record.JapaneseA;
                    break;
                case Record.JapaneseB:
                    e.Column.Header = Record.JapaneseB;
                    break;
                case Record.JapaneseConsistency:
                    e.Column.Header = Record.JapaneseConsistency;
                    break;
                case Record.JapaneseImportance:
                    e.Column.Header = Record.JapaneseImportance;
                    break;
                case Record.JapaneseComment:
                    e.Column.Header = Record.JapaneseComment;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
            }
        }

        /// <summary>
        /// 比較結果(Record)をDataTableへ出力
        /// </summary>
        private DataTable CreateTable()
        {
            DataTable table = new();
            DataColumn column = table.Columns.Add(Record.JapaneseParentElement, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(Record.JapaneseKey, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(Record.JapaneseItem, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(Record.JapaneseA, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(Record.JapaneseB, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(Record.JapaneseConsistency, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(Record.JapaneseImportance, typeof(string));
            column.ReadOnly = true;
            _ = table.Columns.Add(Record.JapaneseComment, typeof(string));

            foreach (Record gridRow in records)
            {
                _ = table.Rows.Add(gridRow.XmlPath, gridRow.Key, gridRow.Item, gridRow.A, gridRow.B, gridRow.Consistency.ToJapanese(), gridRow.Importance.ToJapanese(), gridRow.Comment);
            }

            return table;
        }

    }
}
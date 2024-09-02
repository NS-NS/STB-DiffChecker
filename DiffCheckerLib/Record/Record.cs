using STBDiffChecker.AttributeType;
using STBDiffChecker.Enum;
using System;
using System.Collections.Generic;
using System.Data;

namespace STBDiffChecker
{
    public class Record
    {

        public const string JapaneseParentElement = "親要素";
        public const string JapaneseKey = "比較対象を決定した情報";
        public const string JapaneseItem = "要素・属性";
        public const string JapaneseA = "ファイルA";
        public const string JapaneseB = "ファイルB";
        public const string JapaneseConsistency = "比較結果";
        public const string JapaneseImportance = "重要度";
        public const string JapaneseComment = "コメント";
        internal string ParentElement { get; }
        internal string Key { get; }
        internal string Item { get; }
        internal string A { get; }
        internal string B { get; }
        internal Consistency Consistency { get; }
        internal Importance Importance { get; }
        internal string Comment;

        internal Record(string parentElement, IReadOnlyList<string> key, string item, string a, string b,
            Consistency consistency, Importance importance)
        {
            this.ParentElement = parentElement;
            if (key == null)
            {
                this.Key = String.Empty;
            }
            else if (key.Count == 1)
            {
                this.Key = key[0];
            }
            else
            {
                this.Key = String.Join(",  ", key);
            }

            this.Item = item;
            this.A = a;
            this.B = b;
            this.Consistency = consistency;
            this.Importance = importance;
        }

        internal Record(string parentElement, IReadOnlyList<string> key, string item, string a, string b,
            Consistency consistency, Importance importance, string comment) : this(parentElement, key, item, a, b, consistency, importance)
        {
            this.Comment = comment;
        }

        /// <summary>
        /// 比較結果(Record)をDataTableへ出力
        /// </summary>
        public static DataTable CreateTable( List<Record> records )
        {
            DataTable table = new DataTable();
            var column = table.Columns.Add(JapaneseParentElement, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(JapaneseKey, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(JapaneseItem, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(JapaneseA, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(JapaneseB, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(JapaneseConsistency, typeof(string));
            column.ReadOnly = true;
            column = table.Columns.Add(JapaneseImportance, typeof(string));
            column.ReadOnly = true;
            table.Columns.Add(JapaneseComment, typeof(string));

            foreach (var gridRow in records)
            {
                table.Rows.Add(gridRow.ParentElement, gridRow.Key, gridRow.Item, gridRow.A, gridRow.B, gridRow.Consistency.ToJapanese(), gridRow.Importance.ToJapanese(), gridRow.Comment);
            }

            return table;
        }

        /// <summary>
        /// SummaryをDataTableへ出力
        /// </summary>
        public static DataTable CreateSummaryTable(Summary summary)
        {
            DataTable table = new DataTable();
            foreach(string header in Summary.HeaderText)
            {
                var column = table.Columns.Add(header, typeof(string));
                column.ReadOnly = true;
            }

            foreach (var row in summary.Rows)
            {
                table.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7]);
            }

            return table;
        }
    }
}
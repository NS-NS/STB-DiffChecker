using DiffCheckerLib.Enum;
using System.Collections.Generic;

namespace DiffCheckerLib
{
    /// <summary>
    /// 結果表示用に比較結果を保持するクラス
    /// </summary>
    public class Record
    {
        public const string JapaneseParentElement = "XMLパス";
        public const string JapaneseKey = "比較対象を決定した情報";
        public const string JapaneseItem = "要素・属性";
        public const string JapaneseA = "ファイルA";
        public const string JapaneseB = "ファイルB";
        public const string JapaneseConsistency = "比較結果";
        public const string JapaneseImportance = "重要度";
        public const string JapaneseComment = "コメント";
        public string XmlPath { get; }
        public string Key { get; }
        public string Item { get; }
        public string A { get; }
        public string B { get; }
        public Consistency Consistency { get; }
        public Importance Importance { get; }
        public string Comment;

        public Record(string parentElement, IReadOnlyList<string> key, string item, string a, string b,
            Consistency consistency, Importance importance)
        {
            XmlPath = parentElement;
            Key = key == null ? string.Empty : key.Count == 1 ? key[0] : string.Join(",  ", key);

            Item = item;
            A = a;
            B = b;
            Consistency = consistency;
            Importance = importance;
        }

        internal Record(string parentElement, IReadOnlyList<string> key, string item, string a, string b,
            Consistency consistency, Importance importance, string comment) : this(parentElement, key, item, a, b, consistency, importance)
        {
            Comment = comment;
        }
    }
}
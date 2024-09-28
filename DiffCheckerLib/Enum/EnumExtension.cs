using System;
using System.Collections.Generic;

namespace DiffCheckerLib.Enum
{
    /// <summary>
    ///  Enumの拡張メソッドだったり、日本語のEnum化だったり
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Consistencyを日本語に変換
        /// </summary>
        public static string ToJapanese(this Consistency consistency)
        {
            return ConcistencyDictionary[consistency];
        }

        private static Dictionary<Consistency, string> ConcistencyDictionary = new()
        {
            {Consistency.Consistent, "一致"},
            {Consistency.AlmostMatch, "許容差内" },
            {Consistency.Inconsistent,"不一致" },
            {Consistency.Incomparable,"比較属性なし" },
            {Consistency.ElementIncomparable,"比較要素なし" }
        };

        /// <summary>
        /// Importanceを日本語に変換
        /// </summary>
        public static string ToJapanese(this Importance importance)
        {
            return ImportancDictionary[importance];
        }

        private static Dictionary<Importance, string> ImportancDictionary = new()
        {
            {Importance.Required, "高"},
            {Importance.Optional, "中"},
            {Importance.Unnecessary, "低"},
            {Importance.NotApplicable, "対象外"},
        };

        /// <summary>
        /// 日本語からConsistencyに変換
        /// </summary>
        public static Importance TranslateJapanese(string japanese)
        {
            foreach (KeyValuePair<Importance, string> importance in ImportancDictionary)
            {
                if (importance.Value == japanese)
                {
                    return importance.Key;
                }
            }
            throw new Exception();
        }
    }
}

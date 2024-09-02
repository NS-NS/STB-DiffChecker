using System;
using System.Collections.Generic;
using STBDiffChecker.AttributeType;

namespace STBDiffChecker.Enum
{
    public static class EnumExtension
    {
        private static Dictionary<Consistency, string> ConcistencyDictionary = new Dictionary<Consistency, string>()
        {
            {Consistency.Consistent, "一致"},
            {Consistency.AlmostMatch, "許容差内" },
            {Consistency.Inconsistent,"不一致" },
            {Consistency.Incomparable,"比較属性なし" },
            {Consistency.ElementIncomparable,"比較要素なし" }
        };

        public static string ToJapanese(this Consistency consistency) => ConcistencyDictionary[consistency];

        private static Dictionary<Importance, string> ImportancDictionary = new Dictionary<Importance, string>()
        {
            {Importance.Required, "高"},
            {Importance.Optional, "中"},
            {Importance.Unnecessary, "低"},
            {Importance.NotApplicable, "対象外"},
        };

        public static string ToJapanese(this Importance importance) => ImportancDictionary[importance];

        internal static Importance TranslateJapanese(string japanese)
        {
            foreach(var importance in ImportancDictionary)
            {
                if (importance.Value == japanese)
                    return importance.Key;
            }
            throw new Exception();
        }
    }
}

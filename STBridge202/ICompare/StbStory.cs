using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbStory : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbStory other && name == other.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE stb)
        {
            return new List<string> { $"name={name}" };
        }

        public bool IsSpecial(PropertyInfo info)
        {
            // id_dependence は依存階へのidだが、比較では参照先の階名を突き合わせる
            return info.Name == "id_dependence";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting itoleranceSetting)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);

            string nameA = FindStoryName(valueA?.ToString(), stbA);
            string nameB = FindStoryName(valueB?.ToString(), stbB);

            records.Add(new Record(
                parentElement,
                key,
                "@" + info.Name,
                nameA,
                nameB,
                nameA == nameB ? Consistency.Consistent : Consistency.Inconsistent,
                ObjectComparer.CheckImportance(parentElement, importanceDict)
            ));
        }

        private static string? FindStoryName(string? id, ST_BRIDGE stbridge)
        {
            if (id == null)
            {
                return null;
            }
            StbStory story = stbridge?.StbModel?.StbStories?.FirstOrDefault(n => n.id == id);
            return story?.name;
        }
    }
}

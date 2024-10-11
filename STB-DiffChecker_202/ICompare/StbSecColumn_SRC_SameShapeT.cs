using DiffCheckerLib.Interface;
using System.Collections.Generic;
using System.Reflection;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Setting;
using System.Windows.Shapes;

namespace ST_BRIDGE202
{
    public partial class StbSecColumn_SRC_SameShapeT : IProperty
    {
        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "shape_H" || info.Name == "shape_T";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<DiffCheckerLib.Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            if (info.Name == "shape_H" || info.Name == "shape_T")
            {
                // @shape=>/StbSecSteel
                parentElement += "=>/StbSecSteel";
                List<string> newKey = info.Name == "shape_H" ? new List<string>(key) { $"shape={shape_H}" } : new List<string>(key) { $"shape={shape_T}" };
                StbSecSteelColumn_S_NotSame.CompareSteelShape(parentElement, info, this, istbA, objB, istbB, newKey, records, importanceDict, toleranceSetting);
            }
        }
    }
}
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Shapes;

namespace ST_BRIDGE202
{
    public partial class StbSecColumn_SRC_NotSameShapeCross : IProperty
    {
        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "shape_X" || info.Name == "shape_Y";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<DiffCheckerLib.Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            if (info.Name == "shape_X" || info.Name == "shape_Y")
            {
                // @shape=>/StbSecSteel
                parentElement += "=>/StbSecSteel";
                List<string> newKey = (info.Name == "shape_X") ? new List<string>(key) { $"shape={shape_X}"} : new List<string>(key) {$"shape={shape_Y}"};
                StbSecSteelColumn_S_NotSame.CompareSteelShape(parentElement, info, this, istbA, objB, istbB, newKey, records, importanceDict, toleranceSetting);
            }
        }
    }
}
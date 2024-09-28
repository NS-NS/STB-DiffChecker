using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbSecColumn_SRC_SameShapePipe : IProperty
    {
        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "shape";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<DiffCheckerLib.Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            if (info.Name == "shape")
            {
                // @shape=>/StbSecSteel
                parentElement += "=>/StbSecSteel";
                List<string> newKey = new List<string>(key) { $"shape={shape}" };
                StbSecSteelColumn_S_NotSame.CompareSteelShape(parentElement, info, this, istbA, objB, istbB, newKey, records, importanceDict, toleranceSetting);
            }
        }
    }
}
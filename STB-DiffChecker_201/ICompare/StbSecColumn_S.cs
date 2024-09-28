using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE201
{
    // shapeはkeyのため、StbSecSteelまで比較する
    public partial class StbSecColumn_S : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecColumn_S other && floor == other.floor && name == other.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return new List<string> { $"name={name},floor={floor}" };
        }
    }
}
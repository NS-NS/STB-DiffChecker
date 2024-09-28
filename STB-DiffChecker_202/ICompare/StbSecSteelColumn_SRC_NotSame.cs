using DiffCheckerLib.Interface;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbSecSteelColumn_SRC_NotSame : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecSteelColumn_SRC_NotSame other && pos == other.pos;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"pos={pos}"];
        }
    }
}
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbSecBarParapet_RC_DoubleNet : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            return obj is StbSecBarParapet_RC_DoubleNet other && pos == other.pos;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE stb = istb as ST_BRIDGE;
            return new List<string> { $"pos={pos}" };
        }
    }
}
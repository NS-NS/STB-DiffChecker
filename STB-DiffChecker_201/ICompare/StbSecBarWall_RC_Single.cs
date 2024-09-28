using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE201
{
    public partial class StbSecBarWall_RC_Single : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecBarWall_RC_Single other && pos == other.pos;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return new List<string> { $"pos={pos}" };
        }
    }
}
using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbSecPileProductNodular_PRC : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecPileProductNodular_PRC other && id_order == other.id_order;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return new List<string> { $"id_order={id_order}" };
        }
    }
}
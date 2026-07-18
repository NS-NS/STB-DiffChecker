using DiffCheckerLib.Interface;

namespace ST_BRIDGE210
{
    public partial class StbSecBarArrangementBeam_SRC : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecBarArrangementBeam_SRC other && order == other.order;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"order={order}"];
        }
    }
}
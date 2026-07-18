using DiffCheckerLib.Interface;

namespace ST_BRIDGE210
{
    public partial class StbSecBarSlab_RC_Truss1Way : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecBarSlab_RC_Truss1Way other && pos == other.pos;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"pos={pos}"];
        }
    }
}
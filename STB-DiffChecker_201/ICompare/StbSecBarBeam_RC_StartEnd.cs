using DiffCheckerLib.Interface;

namespace ST_BRIDGE201
{
    public partial class StbSecBarBeam_RC_StartEnd : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecBarBeam_RC_StartEnd other && pos == other.pos;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"pos={pos}"];
        }
    }
}
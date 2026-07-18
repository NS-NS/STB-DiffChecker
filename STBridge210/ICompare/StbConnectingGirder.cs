using DiffCheckerLib.Interface;

namespace ST_BRIDGE210
{
    public partial class StbConnectingGirder : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbConnectingGirder other && pos == other.pos;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"pos={pos}"];
        }
    }
}
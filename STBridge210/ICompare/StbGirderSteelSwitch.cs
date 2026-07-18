using DiffCheckerLib.Interface;

namespace ST_BRIDGE210
{
    public partial class StbGirderSteelSwitch : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbGirderSteelSwitch other && order == other.order;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"order={order}"];
        }
    }
}
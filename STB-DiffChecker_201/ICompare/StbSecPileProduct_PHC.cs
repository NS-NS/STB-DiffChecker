using DiffCheckerLib.Interface;

namespace ST_BRIDGE201
{
    public partial class StbSecPileProduct_PHC : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecPileProduct_PHC other && id_order == other.id_order;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"id_order={id_order}"];
        }
    }
}
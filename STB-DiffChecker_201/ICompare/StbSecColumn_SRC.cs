using DiffCheckerLib.Interface;

namespace ST_BRIDGE201
{
    // shapeはkeyのため、StbSecSteelまで比較する
    public partial class StbSecColumn_SRC : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecColumn_SRC other && floor == other.floor && name == other.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"name={name},floor={floor}"];
        }
    }
}
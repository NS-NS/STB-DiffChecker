using DiffCheckerLib.Interface;

namespace ST_BRIDGE201
{
    public partial class StbSecParapet_RC : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecParapet_RC other && name == other.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"name={name}"];
        }
    }
}
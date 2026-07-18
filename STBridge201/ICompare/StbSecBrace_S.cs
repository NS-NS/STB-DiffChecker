using DiffCheckerLib.Interface;
using System.Collections.Generic;

namespace ST_BRIDGE201
{
    public partial class StbSecBrace_S : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbSecBrace_S other && floor == other.floor && name == other.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"name={name},floor={floor}"];
        }
    }
}
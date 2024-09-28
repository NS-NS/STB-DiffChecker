using DiffCheckerLib.Interface;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbRadialAxes : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbRadialAxes item && group_name == item.group_name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"group_name={group_name}"];
        }
    }
}

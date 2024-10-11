using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbParallelAxes : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbParallelAxes item && group_name == item.group_name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"group_name={group_name}"];
        }
    }
}

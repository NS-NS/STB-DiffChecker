using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbParallelAxis : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbParallelAxis item && name == item.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"name={name}"];
        }
    }
}

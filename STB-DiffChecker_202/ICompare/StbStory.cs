using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbStory : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbStory other && name == other.name;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE stb)
        {
            return new List<string> { $"name={name}" };
        }
    }
}

using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;

namespace ST_BRIDGE202
{
    public partial class StbJointBeamShapeH : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            return obj is StbJointBeamShapeH other && joint_mark == other.joint_mark;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            return [$"joint_mark={joint_mark}"];
        }
    }
}
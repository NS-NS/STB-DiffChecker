using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST_BRIDGE201
{
    public partial class StbNodeId : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            if (!(obj is StbNodeId other))
            {
                return false;
            }

            StbNode nodeA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
            StbNode nodeB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id);

            return Math.Abs(nodeA.X - nodeB.X) < 0.00001 && Math.Abs(nodeA.Y - nodeB.Y) < 0.00001 && Math.Abs(nodeA.Z - nodeB.Z) < 0.00001;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE stb = istb as ST_BRIDGE;
            StbNode node = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
            return new List<string> { $"({node.X},{node.Y},{node.Z})" };
        }
    }
}

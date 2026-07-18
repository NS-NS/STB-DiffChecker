using DiffCheckerLib;
using DiffCheckerLib.Interface;
using STB_DiffChecker_201;

namespace ST_BRIDGE201
{
    public partial class StbWallOffset : ICompare
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            if (obj is not StbWallOffset other)
            {
                return false;
            }

            StbNode nodeA = stbA.FindNode(id_node);
            StbNode nodeB = stbB.FindNode(other.id_node);

            if (nodeA == null || nodeB == null)
            {
                return false;
            }

            return Math.Abs(nodeA.X - nodeB.X) <= Utility.Tolerance &&
                Math.Abs(nodeA.Y - nodeB.Y) <= Utility.Tolerance &&
                Math.Abs(nodeA.Z - nodeB.Z) <= Utility.Tolerance;
        }

        public bool AlmostCompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB, IToleranceSetting itoleranceSetting)
        {
            if (obj is not StbWallOffset other)
            {
                return false;
            }
            ToleranceSetting? toleranceSetting = itoleranceSetting as ToleranceSetting;
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            StbNode nodeA = stbA.FindNode(id_node);
            StbNode nodeB = stbB.FindNode(other.id_node);

            if (nodeA == null || nodeB == null)
            {
                return false;
            }

            return Math.Abs(nodeA.X - nodeB.X) <= toleranceSetting.WallTolerance.Offset &&
                Math.Abs(nodeA.Y - nodeB.Y) <= toleranceSetting.WallTolerance.Offset &&
                Math.Abs(nodeA.Z - nodeB.Z) <= toleranceSetting.WallTolerance.Offset;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE? stb = istb as ST_BRIDGE;
            StbNode node = stb.FindNode(id_node);
            return [$"offset=({node.X},{node.Y},{node.Z})"];
        }
    }
}

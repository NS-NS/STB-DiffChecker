using DiffCheckerLib;
using DiffCheckerLib.Interface;
using STB_DiffChecker_202;

namespace ST_BRIDGE202
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

            StbNode nodeA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);
            StbNode nodeB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node);

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

            StbNode nodeA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);
            StbNode nodeB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node);

            return Math.Abs(nodeA.X - nodeB.X) <= toleranceSetting.WallTolerance.Offset &&
                Math.Abs(nodeA.Y - nodeB.Y) <= toleranceSetting.WallTolerance.Offset &&
                Math.Abs(nodeA.Z - nodeB.Z) <= toleranceSetting.WallTolerance.Offset;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE? stb = istb as ST_BRIDGE;
            StbNode node = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);
            return [$"offset=({node.X},{node.Y},{node.Z})"];
        }
    }
}

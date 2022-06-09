using STBDiffChecker.v201.Records;
using STBridge201;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.AttributeType
{
    public class ReferenceNodeAttribute : AbstractAttribute
    {
        public ReferenceNodeAttribute(string stbName) : base(stbName)
        {

        }

        internal void Compare(StbNode a, StbNode b, double margin, IReadOnlyList<string> key, List<Record> records)
        {
            if (Nodes.CheckAnalysisDistance(a, b, Utility.Tolerance))
                records.Add(new Record(this.ParentElement(), key, this.Item(), Property(a), Property(b),
                    Consistency.Consistent, this.Importance));
            else if (Nodes.CheckAnalysisDistance(a, b, margin))
                records.Add(new Record(this.ParentElement(), key, this.Item(), Property(a), Property(b),
                    Consistency.AlmostMatch, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), Property(a), Property(b),
                    Consistency.Inconsistent, this.Importance));
        }

        internal void Compare(string a, ST_BRIDGE stbridgeA, string b, ST_BRIDGE stbridgeB, IReadOnlyList<string> key,
            List<Record> records)
        {
            Compare(a, stbridgeA, b, stbridgeB, Utility.Tolerance, key, records);
        }

        internal void Compare(string a, ST_BRIDGE stbridgeA, string b, ST_BRIDGE stbridgeB, double margin, IReadOnlyList<string> key, List<Record> records)
        {
            var nodeA = stbridgeA.StbModel.StbNodes.First(n => n.id == a);
            var nodeB = stbridgeB.StbModel.StbNodes.First(n => n.id == b);
            Compare(nodeA, nodeB, margin, key, records);
        }

        private string Property(StbNode nodeA)
        {
            return "id=" + nodeA.id + "(X=" + nodeA.X + ", Y=" + nodeA.Y + ", Z=" + nodeA.Z + ")";
        }

    }
}

using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;
using StbNode = STBridge201.StbNode;

namespace STBDiffChecker.v201.Records
{
    internal static class Nodes
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var nodeA = stBridgeA?.StbModel?.StbNodes;
            var nodeB = stBridgeB?.StbModel?.StbNodes;
            var setB = nodeB != null ? new HashSet<StbNode>(nodeB) : new HashSet<StbNode>();

            if (nodeA != null)
            {
                foreach (var a in nodeA)
                {
                    var key = new List<string> {$"({a.X},{a.Y},{a.Z})" };
                    bool hasItem = false;
                    if (nodeB != null)
                    {
                        foreach (var b in nodeB.Where(n =>
                            Math.Abs(n.X - a.X) < Utility.Tolerance &&
                            Math.Abs(n.Y - a.Y) < Utility.Tolerance &&
                            Math.Abs(n.Z - a.Z) < Utility.Tolerance))
                        {
                            CheckObjects.StbNodeId.Compare(a.id, b.id, key, records);
                            CheckObjects.StbNodeGuid.Compare(a.guid, b.guid, key, records);
                            CheckObjects.StbNodeX.Compare(a.X, b.X, key, records);
                            CheckObjects.StbNodeY.Compare(a.Y, b.Y, key, records);
                            CheckObjects.StbNodeZ.Compare(a.Z, b.Z, key, records);
                            CheckObjects.StbNodeKind.Compare(a.kind.ToString(), b.kind.ToString(), key, records);
                            CheckObjects.StbNodeIdMember.Compare(a.id_member, b.id_member, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }

                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbNode.Compare(nameof(StbNode), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"({b.X},{b.Y},{b.Z})" };
                CheckObjects.StbNode.Compare(null, nameof(StbNode), key, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        internal static bool CheckAnalysisDistance(StbNode a, StbNode b, double margin)
        {
            if (Math.Abs(a.X - b.X) > margin)
                return false;
            if (Math.Abs(a.Y - b.Y) > margin)
                return false;
            if (Math.Abs(a.Z - b.Z) > margin)
                return false;
            double square = Math.Pow(a.X - b.X, 2.0) + Math.Pow(a.Y - b.Y, 2.0) + Math.Pow(a.Z - b.Z, 2.0);
            if (margin > Utility.Tolerance && square > margin * margin)
                return false;

            return true;
        }

    }
}

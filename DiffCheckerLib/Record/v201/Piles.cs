using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbPile = STBridge201.StbPile;

namespace STBDiffChecker.v201.Records
{
    internal static class Piles
    {
        internal static double analysisMargin = 500;

        private class CashPile
        {
            internal readonly StbPile cashPile;
            internal readonly StbNode cashNode;

            internal CashPile(StbPile pile, ST_BRIDGE stBridge)
            {
                cashPile = pile;
                cashNode = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == pile.id);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbPiles;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbPiles;
            
            List<CashPile> cashPiles = new List<CashPile>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashPiles.Add(new CashPile(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashPile>(cashPiles);

            if (membersA != null)
            {
                foreach (var a in membersA)
                {
                    var nodeA = stBridgeA?.StbModel?.StbNodes.First(n => n.id == a.id_node);
                    var key = new List<string>
                    {
                        $"node=({nodeA.X},{nodeA.Y},{nodeA.Z})"
                    };

                    bool hasItem = false;
                    foreach (var b in cashPiles)
                    {
                        if (Nodes.CheckAnalysisDistance(nodeA, b.cashNode, analysisMargin))
                        {
                            if (Nodes.CheckAnalysisDistance(nodeA, b.cashNode, analysisMargin))
                            {
                                CheckObjects.StbPile.AppendConcistentRecord(nameof(StbPile), key, records);
                            }
                            else
                            {
                                CheckObjects.StbPile.AppendConcistentRecord(nameof(StbPile), key, records, true);
                            }

                            ComparePile(stBridgeA, stBridgeB, a, b.cashPile, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbPile.Compare(nameof(StbPile), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"node=({b.cashNode.X},{b.cashNode.Y},{b.cashNode.Z})"
                    };
                    CheckObjects.StbPile.Compare(null, nameof(StbPile), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void ComparePile(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbPile memberA, StbPile memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbPileId.Compare(memberA.id, memberB.id, key, records);
            StbPileGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbPileName.Compare(memberA.name, memberB.name, key, records);
            StbPileIdNode.Compare(memberA.id_node, stBridgeA, memberB.id_node, stBridgeB, analysisMargin, key, records);
            StbPileIdSection.ComparePileSection(memberA.id_section, memberA.kind_structure.ToString(), stBridgeA,
                memberB.id_section, memberB.kind_structure.ToString(), stBridgeB, key, records);
            StbPileOffsetX.Compare(memberA.offset_XSpecified, memberA.offset_X,
                memberB.offset_XSpecified, memberB.offset_X, key, records);
            StbPileOffsetY.Compare(memberA.offset_YSpecified, memberA.offset_Y,
                memberB.offset_YSpecified, memberB.offset_Y, key, records);
            StbPileLevelTop.Compare(memberA.level_topSpecified, memberA.level_top,
                memberB.level_topSpecified, memberB.level_top, key, records);
            StbPileLengthAll.Compare(memberA.length_allSpecified, memberA.length_all,
                memberB.length_allSpecified, memberB.length_all, key, records);
            StbPileLengthHead.Compare(memberA.length_headSpecified, memberA.length_head,
                memberB.length_headSpecified, memberB.length_head, key, records);
            StbPileLengthFoot.Compare(memberA.length_footSpecified, memberA.length_foot,
                memberB.length_footSpecified, memberB.length_foot, key, records);
        }
    }
}

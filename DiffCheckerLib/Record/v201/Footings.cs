using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbFooting = STBridge201.StbFooting;

namespace STBDiffChecker.v201.Records
{
    internal static class Footings
    {
        internal static double analysisMargin = 500;

        private class CashFooting
        {
            internal readonly StbFooting cashFooting;
            internal readonly StbNode cashNode;

            internal CashFooting(StbFooting footing, ST_BRIDGE stBridge)
            {
                cashFooting = footing;
                cashNode = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == footing.id_node);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbFootings;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbFootings;

            List<CashFooting> cashFootings = new List<CashFooting>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashFootings.Add(new CashFooting(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashFooting>(cashFootings);

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
                    foreach (var b in cashFootings)
                    {
                        if (Nodes.CheckAnalysisDistance(nodeA, b.cashNode, analysisMargin))
                        {
                            CompareFooting(stBridgeA, stBridgeB, a, b.cashFooting, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbFooting.Compare(nameof(StbFooting), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"node=({b.cashNode.X},{b.cashNode.Y},{b.cashNode.Z})"
                    };
                    CheckObjects.StbFooting.Compare(null, nameof(StbFooting), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareFooting(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbFooting memberA, StbFooting memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbFootingId.Compare(memberA.id, memberB.id, key, records);
            StbFootingGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbFootingName.Compare(memberA.name, memberB.name, key, records);
            StbFootingIdNode.Compare(memberA.id_node, stBridgeA, memberB.id_node, stBridgeB, analysisMargin, key, records);
            StbFootingRotate.Compare(memberA.rotate, memberB.rotate, key, records);
            StbFootingIdSection.CompareFoundationSection(memberA.id_section, stBridgeA, memberB.id_section, stBridgeB, key, records);
            StbFootingOffsetX.Compare(memberA.offset_XSpecified, memberA.offset_X,
                memberB.offset_XSpecified, memberB.offset_X, key, records);
            StbFootingOffsetY.Compare(memberA.offset_YSpecified, memberA.offset_Y,
                memberB.offset_YSpecified, memberB.offset_Y, key, records);
            StbFootingLevelBottom.Compare(memberA.level_bottomSpecified, memberA.level_bottom,
                memberB.level_bottomSpecified, memberB.level_bottom, key, records);
            StbFootingThicknessAddStartX.Compare(memberA.thickness_add_start_XSpecified, memberA.thickness_add_start_X,
                memberB.thickness_add_start_XSpecified, memberB.thickness_add_start_X, key, records);
            StbFootingThicknessAddEndX.Compare(memberA.thickness_add_end_XSpecified, memberA.thickness_add_end_X,
                memberB.thickness_add_end_XSpecified, memberB.thickness_add_end_X, key, records);
            StbFootingThicknessAddStartY.Compare(memberA.thickness_add_start_YSpecified, memberA.thickness_add_start_Y,
                memberB.thickness_add_start_YSpecified, memberB.thickness_add_start_Y, key, records);
            StbFootingThicknessAddEndY.Compare(memberA.thickness_add_end_YSpecified, memberA.thickness_add_end_Y,
                memberB.thickness_add_end_YSpecified, memberB.thickness_add_end_Y, key, records);
            StbFootingThicknessAddTop.Compare(memberA.thickness_add_topSpecified, memberA.thickness_add_top,
                memberB.thickness_add_topSpecified, memberB.thickness_add_top, key, records);
            StbFootingThicknessAddBottom.Compare(memberA.thickness_add_bottomSpecified, memberA.thickness_add_bottom,
                memberB.thickness_add_bottomSpecified, memberB.thickness_add_bottom, key, records);
        }
    }
}

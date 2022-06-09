using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbBrace = STBridge201.StbBrace;

namespace STBDiffChecker.v201.Records
{
    internal static class Braces
    {
        internal static double analysisMargin = Utility.Tolerance;
        internal static double offsetMargin = Utility.Tolerance;

        private class CashBrace
        {
            internal StbBrace cashBrace;
            internal StbNode cashNodeStart;
            internal StbNode cashNodeEnd;

            internal CashBrace(StbBrace brace, ST_BRIDGE stBridge)
            {
                cashBrace = brace;
                cashNodeStart = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == brace.id_node_start);
                cashNodeEnd = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == brace.id_node_end);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbBraces;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbBraces;

            List<CashBrace> cashBraces = new List<CashBrace>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashBraces.Add(new CashBrace(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashBrace>(cashBraces);

            if (membersA != null)
            {
                foreach (var a in membersA)
                {
                    var startA = stBridgeA?.StbModel?.StbNodes.First(n => n.id == a.id_node_start);
                    var endA = stBridgeA?.StbModel?.StbNodes.First(n => n.id == a.id_node_end);
                    var key = new List<string>
                    {
                        $"start=({startA.X},{startA.Y},{startA.Z})",
                        $"end=({endA.X},{endA.Y},{endA.Z})"
                    };

                    bool hasItem = false;
                    foreach (var b in cashBraces)
                    {
                        if (Nodes.CheckAnalysisDistance(startA, b.cashNodeStart, analysisMargin) && Nodes.CheckAnalysisDistance(endA, b.cashNodeEnd, analysisMargin))
                        {
                            CompareBrace(stBridgeA, stBridgeB, a, b.cashBrace, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbBrace.Compare(nameof(StbBrace), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"start=({b.cashNodeStart.X},{b.cashNodeStart.Y},{b.cashNodeStart.Z})",
                        $"end=({b.cashNodeEnd.X},{b.cashNodeEnd.Y},{b.cashNodeEnd.Z})"
                    };
                    CheckObjects.StbBrace.Compare(null, nameof(StbBrace), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareBrace(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbBrace memberA, StbBrace memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbBraceId.Compare(memberA.id, memberB.id, key, records);
            StbBraceGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbBraceName.Compare(memberA.name, memberB.name, key, records);
            StbBraceIdNodeStart.Compare(memberA.id_node_start, stBridgeA, memberB.id_node_start, stBridgeB, analysisMargin, key, records);
            StbBraceIdNodeEnd.Compare(memberA.id_node_end, stBridgeA, memberB.id_node_end, stBridgeB, analysisMargin, key, records);
            StbBraceRotate.Compare(memberA.rotate, memberB.rotate, key, records);
            StbBraceIdSection.CompareBraceSection(memberA.id_section, memberA.kind_structure.ToString(), stBridgeA,
                memberB.id_section, memberB.kind_structure.ToString(), stBridgeB, key, records);
            StbBraceKindStructure.Compare(memberA.kind_structure.ToString(), memberB.kind_structure.ToString(), key, records);
            StbBraceOffsetStartX.Compare(memberA.offset_start_XSpecified, memberA.offset_start_X,
                memberB.offset_start_XSpecified, memberB.offset_start_X, key, records, offsetMargin);
            StbBraceOffsetStartY.Compare(memberA.offset_start_YSpecified, memberA.offset_start_Y,
                memberB.offset_start_YSpecified, memberB.offset_start_Y, key, records, offsetMargin);
            StbBraceOffsetStartZ.Compare(memberA.offset_start_ZSpecified, memberA.offset_start_Z,
                memberB.offset_start_ZSpecified, memberB.offset_start_Z, key, records, offsetMargin);
            StbBraceOffsetEndX.Compare(memberA.offset_end_XSpecified, memberA.offset_end_X,
                memberB.offset_end_XSpecified, memberB.offset_end_X, key, records, offsetMargin);
            StbBraceOffsetEndY.Compare(memberA.offset_end_YSpecified, memberA.offset_end_Y,
                memberB.offset_end_YSpecified, memberB.offset_end_Y, key, records, offsetMargin);
            StbBraceOffsetEndZ.Compare(memberA.offset_end_ZSpecified, memberA.offset_end_Z,
                memberB.offset_end_ZSpecified, memberB.offset_end_Z, key, records, offsetMargin);
            StbBraceConditionStart.Compare(memberA.condition_start.ToString(), memberB.condition_start.ToString(), key, records);
            StbBraceConditionEnd.Compare(memberA.condition_end.ToString(), memberB.condition_end.ToString(), key, records);
            StbBraceFeatureBrace.Compare(memberA.feature_brace.ToString(), memberB.feature_brace.ToString(), key, records);
            StbBraceJointStart.Compare(memberA.joint_startSpecified, memberA.joint_start, memberB.joint_startSpecified,
                memberB.joint_start, key, records);
            StbBraceJointEnd.Compare(memberA.joint_endSpecified, memberA.joint_end,
                memberB.joint_endSpecified, memberB.joint_end, key, records);
            StbBraceKindJointStart.Compare(memberA.kind_joint_start.ToString(), memberB.kind_joint_start.ToString(), key, records);
            StbBraceKindJointEnd.Compare(memberA.kind_joint_end.ToString(), memberB.kind_joint_end.ToString(), key, records);
            StbBraceJointIdStart.Compare(memberA.joint_id_start, stBridgeA, memberB.joint_id_start, stBridgeB, key, records);
            StbBraceJointIdEnd.Compare(memberA.joint_id_end, stBridgeA, memberB.joint_id_end, stBridgeB, key, records);
        }
    }
}

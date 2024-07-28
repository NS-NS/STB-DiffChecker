using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbGirder = STBridge201.StbGirder;

namespace STBDiffChecker.v201.Records
{
    internal static class Girders
    {
        internal static double analysisMargin = Utility.Tolerance;
        internal static double offsetMargin = Utility.Tolerance;

        private class CashGirder
        {
            internal readonly StbGirder cashGirder;
            internal readonly StbNode cashNodeStart;
            internal readonly StbNode cashNodeEnd;

            internal CashGirder(StbGirder girder, ST_BRIDGE stBridge)
            {
                cashGirder = girder;
                cashNodeStart = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == girder.id_node_start);
                cashNodeEnd = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == girder.id_node_end);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbGirders;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbGirders;

            List<CashGirder> cashGirders = new List<CashGirder>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashGirders.Add(new CashGirder(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashGirder>(cashGirders);

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
                    foreach (var b in cashGirders)
                    {
                        if (Nodes.CheckAnalysisDistance(startA, b.cashNodeStart, analysisMargin) && Nodes.CheckAnalysisDistance(endA, b.cashNodeEnd, analysisMargin))
                        {
                            if (Nodes.CheckAnalysisDistance(startA, b.cashNodeStart, Utility.Tolerance) &&
                                Nodes.CheckAnalysisDistance(endA, b.cashNodeEnd, Utility.Tolerance))
                            {
                                CheckObjects.StbGirder.AppendConcistentRecord(nameof(CheckObjects.StbGirder), key, records);
                            }
                            else
                            {
                                CheckObjects.StbGirder.AppendConcistentRecord(nameof(CheckObjects.StbGirder), key, records, true);
                            }
                            CompareGirder(stBridgeA, stBridgeB, a, b.cashGirder, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbGirder.Compare(nameof(StbGirder), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"start=({b.cashNodeStart.X},{b.cashNodeStart.Y},{b.cashNodeStart.Z})",
                        $"end=({b.cashNodeEnd.X},{b.cashNodeEnd.Y},{b.cashNodeEnd.Z})"
                    };
                    CheckObjects.StbGirder.Compare(null, nameof(StbGirder), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareGirder(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbGirder memberA, StbGirder memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbGirderId.Compare(memberA.id, memberB.id, key, records);
            StbGirderGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbGirderName.Compare(memberA.name, memberB.name, key, records);
            StbGirderIdNodeStart.Compare(memberA.id_node_start, stBridgeA, memberB.id_node_start, stBridgeB, analysisMargin, key, records);
            StbGirderIdNodeEnd.Compare(memberA.id_node_end, stBridgeA, memberB.id_node_end, stBridgeB, analysisMargin, key, records);
            StbGirderRotate.Compare(memberA.rotate, memberB.rotate, key, records);
            StbGirderIdSection.CompareBeamSection(memberA.id_section, memberA.kind_structure.ToString(), stBridgeA,
                memberB.id_section, memberB.kind_structure.ToString(), stBridgeB, key, records);
            StbGirderSectionIoStart.Compare(memberA.section_io_startSpecified, memberA.section_io_start.ToString(), memberB.section_io_startSpecified, memberB.section_io_start.ToString(), key, records);
            StbGirderSectionIoEnd.Compare(memberA.section_io_endSpecified, memberA.section_io_end.ToString(), memberB.section_io_endSpecified, memberB.section_io_end.ToString(), key, records);
            StbGirderKindStructure.Compare(memberA.kind_structure.ToString(), memberB.kind_structure.ToString(), key, records);
            StbGirderIsFoundation.Compare(memberA.isFoundation, memberB.isFoundation, key, records);
            StbGirderOffsetStartX.Compare(memberA.offset_start_XSpecified, memberA.offset_start_X,
                memberB.offset_start_XSpecified, memberB.offset_start_X, key, records, offsetMargin);
            StbGirderOffsetStartY.Compare(memberA.offset_start_YSpecified, memberA.offset_start_Y,
                memberB.offset_start_YSpecified, memberB.offset_start_Y, key, records, offsetMargin);
            StbGirderOffsetStartZ.Compare(memberA.offset_start_ZSpecified, memberA.offset_start_Z,
                memberB.offset_start_ZSpecified, memberB.offset_start_Z, key, records, offsetMargin);
            StbGirderOffsetEndX.Compare(memberA.offset_end_XSpecified, memberA.offset_end_X,
                memberB.offset_end_XSpecified, memberB.offset_end_X, key, records, offsetMargin);
            StbGirderOffsetEndY.Compare(memberA.offset_end_YSpecified, memberA.offset_end_Y,
                memberB.offset_end_YSpecified, memberB.offset_end_Y, key, records, offsetMargin);
            StbGirderOffsetEndZ.Compare(memberA.offset_end_ZSpecified, memberA.offset_end_Z,
                memberB.offset_end_ZSpecified, memberB.offset_end_Z, key, records, offsetMargin);
            StbGirderThicknessAddTop.Compare(memberA.thickness_add_topSpecified, memberA.thickness_add_top,
                memberB.thickness_add_topSpecified, memberB.thickness_add_top, key, records);
            StbGirderThicknessAddBottom.Compare(memberA.thickness_add_bottomSpecified, memberA.thickness_add_bottom,
                memberB.thickness_add_bottomSpecified, memberB.thickness_add_bottom, key, records);
            StbGirderThicknessAddRight.Compare(memberA.thickness_add_rightSpecified, memberA.thickness_add_right,
                memberB.thickness_add_rightSpecified, memberB.thickness_add_right, key, records);
            StbGirderThicknessAddLeft.Compare(memberA.thickness_add_leftSpecified, memberA.thickness_add_left,
                memberB.thickness_add_leftSpecified, memberB.thickness_add_left, key, records);
            StbGirderConditionStart.Compare(memberA.condition_start.ToString(), memberB.condition_start.ToString(), key, records);
            StbGirderConditionEnd.Compare(memberA.condition_end.ToString(), memberB.condition_end.ToString(), key, records);
            StbGirderHaunchStart.Compare(memberA.haunch_startSpecified, memberA.haunch_start,
                memberB.haunch_startSpecified, memberB.haunch_start, key, records);
            StbGirderHaunchEnd.Compare(memberA.haunch_endSpecified, memberA.haunch_end,
                memberB.haunch_endSpecified, memberB.haunch_end, key, records);
            StbGirderJointStart.Compare(memberA.joint_startSpecified, memberA.joint_start, memberB.joint_startSpecified,
                memberB.joint_start, key, records);
            StbGirderJointEnd.Compare(memberA.joint_endSpecified, memberA.joint_end,
                memberB.joint_endSpecified, memberB.joint_end, key, records);
            StbGirderKindHaunchStart.Compare(memberA.kind_haunch_start.ToString(), memberB.kind_haunch_start.ToString(), key, records);
            StbGirderKindHaunchEnd.Compare(memberA.kind_haunch_end.ToString(), memberB.kind_haunch_end.ToString(), key, records);
            StbGirderTypeHaunchH.Compare(memberA.type_haunch_H.ToString(), memberB.type_haunch_H.ToString(), key, records);
            StbGirderTypeHaunchV.Compare(memberA.type_haunch_V.ToString(), memberB.type_haunch_V.ToString(), key, records);
            StbGirderKindJointStart.Compare(memberA.kind_joint_start.ToString(), memberB.kind_joint_start.ToString(), key, records);
            StbGirderKindJointEnd.Compare(memberA.kind_joint_end.ToString(), memberB.kind_joint_end.ToString(), key, records);
            StbGirderJointIdStart.Compare(memberA.joint_id_start, stBridgeA, memberB.joint_id_start, stBridgeB, key, records);
            StbGirderJointIdEnd.Compare(memberA.joint_id_end, stBridgeA, memberB.joint_id_end, stBridgeB, key, records);
        }
    }
}

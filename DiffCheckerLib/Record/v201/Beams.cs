using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbBeam = STBridge201.StbBeam;

namespace STBDiffChecker.v201.Records
{
    internal static class Beams
    {
        internal static double analysisMargin = Utility.Tolerance;
        internal static double offsetMargin = Utility.Tolerance;

        private class CashBeam
        {
            internal readonly StbBeam cashBeam;
            internal readonly StbNode cashNodeStart;
            internal readonly StbNode cashNodeEnd;

            internal CashBeam(StbBeam beam, ST_BRIDGE stBridge)
            {
                cashBeam = beam;
                cashNodeStart = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == beam.id_node_start);
                cashNodeEnd = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == beam.id_node_end);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbBeams;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbBeams;

            List<CashBeam> cashBeams = new List<CashBeam>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashBeams.Add(new CashBeam(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashBeam>(cashBeams);

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
                    foreach (var b in cashBeams)
                    {
                        if (Nodes.CheckAnalysisDistance(startA, b.cashNodeStart, analysisMargin) && Nodes.CheckAnalysisDistance(endA, b.cashNodeEnd, analysisMargin))
                        {
                            if (Nodes.CheckAnalysisDistance(startA, b.cashNodeStart, analysisMargin) &&
                                Nodes.CheckAnalysisDistance(endA, b.cashNodeEnd, analysisMargin))
                            {
                                CheckObjects.StbBeam.AppendConcistentRecord(nameof(StbBeam), key, records);
                            }
                            else
                            {
                                CheckObjects.StbBeam.AppendConcistentRecord(nameof(StbBeam), key, records, true);
                            }

                            CompareBeam(stBridgeA, stBridgeB, a, b.cashBeam, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        var key1 = new List<string>(key) { $"id={a.id}" };
                        CheckObjects.StbBeam.Compare(nameof(StbBeam), null, key1, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"start=({b.cashNodeStart.X},{b.cashNodeStart.Y},{b.cashNodeStart.Z})",
                        $"end=({b.cashNodeEnd.X},{b.cashNodeEnd.Y},{b.cashNodeEnd.Z})",
                        $"id={b.cashBeam.id}"
                    };
                    CheckObjects.StbBeam.Compare(null, nameof(StbBeam), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareBeam(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbBeam memberA, StbBeam memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbBeamId.Compare(memberA.id, memberB.id, key, records);
            StbBeamGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbBeamName.Compare(memberA.name, memberB.name, key, records);
            StbBeamIdNodeStart.Compare(memberA.id_node_start, stBridgeA, memberB.id_node_start, stBridgeB, analysisMargin, key, records);
            StbBeamIdNodeEnd.Compare(memberA.id_node_end, stBridgeA, memberB.id_node_end, stBridgeB, analysisMargin, key, records);
            StbBeamRotate.Compare(memberA.rotate, memberB.rotate, key, records);
            StbBeamIdSection.CompareBeamSection(memberA.id_section, memberA.kind_structure.ToString(), stBridgeA,
                memberB.id_section, memberB.kind_structure.ToString(), stBridgeB, key, records);
            StbBeamSectionIoStart.Compare(memberA.section_io_startSpecified, memberA.section_io_start.ToString(), memberB.section_io_startSpecified, memberB.section_io_start.ToString(), key, records);
            StbBeamSectionIoEnd.Compare(memberA.section_io_endSpecified, memberA.section_io_end.ToString(), memberB.section_io_endSpecified, memberB.section_io_end.ToString(), key, records);
            StbBeamKindStructure.Compare(memberA.kind_structure.ToString(), memberB.kind_structure.ToString(), key, records);
            StbBeamIsFoundation.Compare(memberA.isFoundation, memberB.isFoundation, key, records);
            StbBeamOffsetStartX.Compare(memberA.offset_start_XSpecified, memberA.offset_start_X,
                memberB.offset_start_XSpecified, memberB.offset_start_X, key, records, offsetMargin);
            StbBeamOffsetStartY.Compare(memberA.offset_start_YSpecified, memberA.offset_start_Y,
                memberB.offset_start_YSpecified, memberB.offset_start_Y, key, records, offsetMargin);
            StbBeamOffsetStartZ.Compare(memberA.offset_start_ZSpecified, memberA.offset_start_Z,
                memberB.offset_start_ZSpecified, memberB.offset_start_Z, key, records, offsetMargin);
            StbBeamOffsetEndX.Compare(memberA.offset_end_XSpecified, memberA.offset_end_X,
                memberB.offset_end_XSpecified, memberB.offset_end_X, key, records, offsetMargin);
            StbBeamOffsetEndY.Compare(memberA.offset_end_YSpecified, memberA.offset_end_Y,
                memberB.offset_end_YSpecified, memberB.offset_end_Y, key, records, offsetMargin);
            StbBeamOffsetEndZ.Compare(memberA.offset_end_ZSpecified, memberA.offset_end_Z,
                memberB.offset_end_ZSpecified, memberB.offset_end_Z, key, records, offsetMargin);
            StbBeamThicknessAddTop.Compare(memberA.thickness_add_topSpecified, memberA.thickness_add_top,
                memberB.thickness_add_topSpecified, memberB.thickness_add_top, key, records);
            StbBeamThicknessAddBottom.Compare(memberA.thickness_add_bottomSpecified, memberA.thickness_add_bottom,
                memberB.thickness_add_bottomSpecified, memberB.thickness_add_bottom, key, records);
            StbBeamThicknessAddRight.Compare(memberA.thickness_add_rightSpecified, memberA.thickness_add_right,
                memberB.thickness_add_rightSpecified, memberB.thickness_add_right, key, records);
            StbBeamThicknessAddLeft.Compare(memberA.thickness_add_leftSpecified, memberA.thickness_add_left,
                memberB.thickness_add_leftSpecified, memberB.thickness_add_left, key, records);
            StbBeamConditionStart.Compare(memberA.condition_start.ToString(), memberB.condition_start.ToString(), key, records);
            StbBeamConditionEnd.Compare(memberA.condition_end.ToString(), memberB.condition_end.ToString(), key, records);
            StbBeamHaunchStart.Compare(memberA.haunch_startSpecified, memberA.haunch_start,
                memberB.haunch_startSpecified, memberB.haunch_start, key, records);
            StbBeamHaunchEnd.Compare(memberA.haunch_endSpecified, memberA.haunch_end,
                memberB.haunch_endSpecified, memberB.haunch_end, key, records);
            StbBeamJointStart.Compare(memberA.joint_startSpecified, memberA.joint_start, memberB.joint_startSpecified,
                memberB.joint_start, key, records);
            StbBeamJointEnd.Compare(memberA.joint_endSpecified, memberA.joint_end,
                memberB.joint_endSpecified, memberB.joint_end, key, records);
            StbBeamKindHaunchStart.Compare(memberA.kind_haunch_start.ToString(), memberB.kind_haunch_start.ToString(), key, records);
            StbBeamKindHaunchEnd.Compare(memberA.kind_haunch_end.ToString(), memberB.kind_haunch_end.ToString(), key, records);
            StbBeamTypeHaunchH.Compare(memberA.type_haunch_H.ToString(), memberB.type_haunch_H.ToString(), key, records);
            StbBeamTypeHaunchV.Compare(memberA.type_haunch_V.ToString(), memberB.type_haunch_V.ToString(), key, records);
            StbBeamKindJointStart.Compare(memberA.kind_joint_start.ToString(), memberB.kind_joint_start.ToString(), key, records);
            StbBeamKindJointEnd.Compare(memberA.kind_joint_end.ToString(), memberB.kind_joint_end.ToString(), key, records);
            StbBeamJointIdStart.Compare(memberA.joint_id_start, stBridgeA, memberB.joint_id_start, stBridgeB, key, records);
            StbBeamJointIdEnd.Compare(memberA.joint_id_end, stBridgeA, memberB.joint_id_end, stBridgeB, key, records);
        }
    }
}

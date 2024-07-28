using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbParapet = STBridge201.StbParapet;

namespace STBDiffChecker.v201.Records
{
    internal static class Parapets
    {
        internal static double analysisMargin = 500;

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbParapets;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbParapets;
            var setB = membersB != null ? new HashSet<StbParapet>(membersB) : new HashSet<StbParapet>();

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
                    if (membersB != null)
                    {
                        foreach (var b in membersB)
                        {
                            var startB = stBridgeB?.StbModel?.StbNodes.First(n => n.id == b.id_node_start);
                            var endB = stBridgeB?.StbModel?.StbNodes.First(n => n.id == b.id_node_end);
                            if (Nodes.CheckAnalysisDistance(startA, startB, analysisMargin) && Nodes.CheckAnalysisDistance(endA, endB, analysisMargin))
                            {
                                if (Nodes.CheckAnalysisDistance(startA, startB, Utility.Tolerance) &&
                                    Nodes.CheckAnalysisDistance(endA, endB, Utility.Tolerance))
                                {
                                    CheckObjects.StbParapet.AppendConcistentRecord(nameof(StbParapet), key, records);
                                }
                                else
                                {
                                    CheckObjects.StbParapet.AppendConcistentRecord(nameof(StbParapet), key, records, true);
                                }
                                CompareParapet(stBridgeA, stBridgeB, a, b, key, records);
                                setB.Remove(b);
                                hasItem = true;
                            }
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbParapet.Compare(nameof(StbParapet), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var start = stBridgeB?.StbModel?.StbNodes.First(n => n.id == b.id_node_start);
                    var end = stBridgeB?.StbModel?.StbNodes.First(n => n.id == b.id_node_end);
                    var key = new List<string>
                    {
                        $"start=({start.X},{start.Y},{start.Z})",
                        $"end=({end.X},{end.Y},{end.Z})"
                    };
                    CheckObjects.StbParapet.Compare(null, nameof(StbParapet), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareParapet(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbParapet memberA, StbParapet memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbParapetId.Compare(memberA.id, memberB.id, key, records);
            StbParapetGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbParapetName.Compare(memberA.name, memberB.name, key, records);
            StbParapetIdNodeStart.Compare(memberA.id_node_start, stBridgeA, memberB.id_node_start, stBridgeB, analysisMargin, key, records);
            StbParapetIdNodeEnd.Compare(memberA.id_node_end, stBridgeA, memberB.id_node_end, stBridgeB, analysisMargin, key, records);
            StbParapetIdSection.CompareParapetSection(memberA.id_section, stBridgeA, memberB.id_section, stBridgeB, key, records);
            StbParapetKindStructure.Compare(memberA.kind_structure.ToString(), memberB.kind_structure.ToString(), key, records);
            StbParapetKindLayout.Compare(memberA.kind_layout.ToString(), memberB.kind_layout.ToString(), key, records);
            CheckObjects.StbParapetDirection.Compare(memberA.directionSpecified, memberA.direction.ToString(),
                memberB.directionSpecified, memberB.direction.ToString(), key, records);
            StbParapetOffset.Compare(memberA.offsetSpecified, memberA.offset,
                memberB.offsetSpecified, memberB.offset, key, records);
            StbParapetLevel.Compare(memberA.levelSpecified, memberA.level,
                memberB.levelSpecified, memberB.level, key, records);
        }
    }
}

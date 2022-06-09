using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbStripFooting = STBridge201.StbStripFooting;

namespace STBDiffChecker.v201.Records
{
    internal static class StripFootings
    {
        internal static double analysisMargin = 500;

        private class CashStripFooting
        {
            internal readonly StbStripFooting cashStripFooting;
            internal readonly StbNode cashNodeStart;
            internal readonly StbNode cashNodeEnd;

            internal CashStripFooting(StbStripFooting footing, ST_BRIDGE stBridge)
            {
                cashStripFooting = footing;
                cashNodeStart = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == footing.id_node_start);
                cashNodeEnd = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == footing.id_node_end);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbStripFootings;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbStripFootings;
            List<CashStripFooting> cashStripFootings = new List<CashStripFooting>();

            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashStripFootings.Add(new CashStripFooting(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashStripFooting>(cashStripFootings);

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
                    foreach (var b in cashStripFootings)
                    {
                        if (Nodes.CheckAnalysisDistance(startA, b.cashNodeStart, analysisMargin) && Nodes.CheckAnalysisDistance(endA, b.cashNodeEnd, analysisMargin))
                        {
                            CompareStripFooting(stBridgeA, stBridgeB, a, b.cashStripFooting, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbStripFooting.Compare(nameof(StbStripFooting), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"start=({b.cashNodeStart.X},{b.cashNodeStart.Y},{b.cashNodeStart.Z})",
                        $"end=({b.cashNodeEnd.X},{b.cashNodeEnd.Y},{b.cashNodeEnd.Z})"
                    };
                    CheckObjects.StbStripFooting.Compare(null, nameof(StbStripFooting), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareStripFooting(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbStripFooting memberA, StbStripFooting memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbStripFootingId.Compare(memberA.id, memberB.id, key, records);
            StbStripFootingGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbStripFootingName.Compare(memberA.name, memberB.name, key, records);
            StbStripFootingIdNodeStart.Compare(memberA.id_node_start, stBridgeA, memberB.id_node_start, stBridgeB, analysisMargin, key, records);
            StbStripFootingIdNodeEnd.Compare(memberA.id_node_end, stBridgeA, memberB.id_node_end, stBridgeB, analysisMargin, key, records);
            StbStripFootingIdSection.CompareFoundationSection(memberA.id_section, stBridgeA, memberB.id_section, stBridgeB, key, records);
            StbStripFootingKindStructure.Compare(memberA.kind_structure.ToString(), memberB.kind_structure.ToString(), key, records);
            StbStripFootingLevel.Compare(memberA.levelSpecified, memberA.level,
                memberB.levelSpecified, memberB.level, key, records);
            StbStripFootingOffset.Compare(memberA.offsetSpecified, memberA.offset,
                memberB.offsetSpecified, memberB.offset, key, records);
            StbStripFootingLengthExStart.Compare(memberA.length_ex_startSpecified, memberA.length_ex_start,
                memberB.length_ex_startSpecified, memberB.length_ex_start, key, records);
            StbStripFootingLengthExEnd.Compare(memberA.length_ex_endSpecified, memberA.length_ex_end,
                memberB.length_ex_endSpecified, memberB.length_ex_end, key, records);
        }
    }
}

using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbFoundationColumn = STBridge201.StbFoundationColumn;

namespace STBDiffChecker.v201.Records
{
    internal static class FoundationColumns
    {
        internal static double analysisMargin = 500;

        private class CashFoundationColumn
        {
            internal readonly StbFoundationColumn cashFoundationColumn;
            internal readonly StbNode cashNode;

            internal CashFoundationColumn(StbFoundationColumn foundationColumn, ST_BRIDGE stBridge)
            {
                cashFoundationColumn = foundationColumn;
                cashNode = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == foundationColumn.id_node);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbFoundationColumns;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbFoundationColumns;

            List<CashFoundationColumn> cashFoundationColumns = new List<CashFoundationColumn>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashFoundationColumns.Add(new CashFoundationColumn(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashFoundationColumn>(cashFoundationColumns);

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
                    foreach (var b in cashFoundationColumns)
                    {
                        if (Nodes.CheckAnalysisDistance(nodeA, b.cashNode, analysisMargin))
                        {
                            if (Nodes.CheckAnalysisDistance(nodeA, b.cashNode, Utility.Tolerance))
                            {
                                CheckObjects.StbFoundationColumn.AppendConcistentRecord(nameof(CheckObjects.StbFoundationColumn), key, records);
                            }
                            else
                            {
                                CheckObjects.StbFoundationColumn.AppendConcistentRecord(nameof(CheckObjects.StbFoundationColumn), key, records, true);
                            }
                            CompareFoundationColumn(stBridgeA, stBridgeB, a, b.cashFoundationColumn, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbFoundationColumn.Compare(nameof(StbFoundationColumn), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"node=({b.cashNode.X},{b.cashNode.Y},{b.cashNode.Z})"
                    };
                    CheckObjects.StbFoundationColumn.Compare(null, nameof(StbFoundationColumn), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareFoundationColumn(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbFoundationColumn memberA, StbFoundationColumn memberB, IReadOnlyList<string> key, List<Record> records)
        {
            StbFoundationColumnId.Compare(memberA.id, memberB.id, key, records);
            StbFoundationColumnGuid.Compare(memberA.guid, memberB.guid, key, records);
            StbFoundationColumnName.Compare(memberA.name, memberB.name, key, records);
            StbFoundationColumnIdNode.Compare(memberA.id_node, stBridgeA, memberB.id_node, stBridgeB, analysisMargin, key, records);
            StbFoundationColumnRotate.Compare(memberA.rotate, memberB.rotate, key, records);
            StbFoundationColumnOffsetZ.Compare(memberA.offset_ZSpecified, memberA.offset_Z,
                memberB.offset_ZSpecified, memberB.offset_Z, key, records);
            StbFoundationColumnKindStructure.Compare(memberA.kind_structure, memberB.kind_structure, key, records);
            StbFoundationColumnIdSectionFD.CompareFoundationColumnSection(memberA.id_section_FD,  stBridgeA, memberB.id_section_FD, stBridgeB, key, records);
            StbFoundationColumnLengthFD.Compare(memberA.length_FDSpecified, memberA.length_FD,
                memberB.length_FDSpecified, memberB.length_FD, key, records);
            StbFoundationColumnOffsetFDX.Compare(memberA.offset_FD_XSpecified, memberA.offset_FD_X,
                memberB.offset_FD_XSpecified, memberB.offset_FD_X, key, records);
            StbFoundationColumnOffsetFDY.Compare(memberA.offset_FD_YSpecified, memberA.offset_FD_Y,
                memberB.offset_FD_YSpecified, memberB.offset_FD_Y, key, records);
            StbFoundationColumnThicknessAddFDStartX.Compare(memberA.thickness_add_FD_start_XSpecified, memberA.thickness_add_FD_start_X,
                memberB.thickness_add_FD_start_XSpecified, memberB.thickness_add_FD_start_X, key, records);
            StbFoundationColumnThicknessAddFDEndX.Compare(memberA.thickness_add_FD_end_XSpecified, memberA.thickness_add_FD_end_X,
                memberB.thickness_add_FD_end_XSpecified, memberB.thickness_add_FD_end_X, key, records);
            StbFoundationColumnThicknessAddFDStartY.Compare(memberA.thickness_add_FD_start_YSpecified, memberA.thickness_add_FD_start_Y,
                memberB.thickness_add_FD_start_YSpecified, memberB.thickness_add_FD_start_Y, key, records);
            StbFoundationColumnThicknessAddFDEndY.Compare(memberA.thickness_add_FD_end_YSpecified, memberA.thickness_add_FD_end_Y,
                memberB.thickness_add_FD_end_YSpecified, memberB.thickness_add_FD_end_Y, key, records);

            StbFoundationColumnIdSectionWR.CompareFoundationColumnSection(memberA.id_section_WR, stBridgeA, memberB.id_section_WR, stBridgeB, key, records);
            StbFoundationColumnLengthWR.Compare(memberA.length_WRSpecified, memberA.length_WR,
                memberB.length_WRSpecified, memberB.length_WR, key, records);
            StbFoundationColumnOffsetWRX.Compare(memberA.offset_WR_XSpecified, memberA.offset_WR_X,
                memberB.offset_WR_XSpecified, memberB.offset_WR_X, key, records);
            StbFoundationColumnOffsetWRY.Compare(memberA.offset_WR_YSpecified, memberA.offset_WR_Y,
                memberB.offset_WR_YSpecified, memberB.offset_WR_Y, key, records);
            StbFoundationColumnThicknessAddWRStartX.Compare(memberA.thickness_add_WR_start_XSpecified, memberA.thickness_add_WR_start_X,
                memberB.thickness_add_WR_start_XSpecified, memberB.thickness_add_WR_start_X, key, records);
            StbFoundationColumnThicknessAddWREndX.Compare(memberA.thickness_add_WR_end_XSpecified, memberA.thickness_add_WR_end_X,
                memberB.thickness_add_WR_end_XSpecified, memberB.thickness_add_WR_end_X, key, records);
            StbFoundationColumnThicknessAddWRStartY.Compare(memberA.thickness_add_WR_start_YSpecified, memberA.thickness_add_WR_start_Y,
                memberB.thickness_add_WR_start_YSpecified, memberB.thickness_add_WR_start_Y, key, records);
            StbFoundationColumnThicknessAddWREndY.Compare(memberA.thickness_add_WR_end_YSpecified, memberA.thickness_add_WR_end_Y,
                memberB.thickness_add_WR_end_YSpecified, memberB.thickness_add_WR_end_Y, key, records);
        }
    }
}

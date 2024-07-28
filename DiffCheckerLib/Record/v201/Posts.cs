using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbColumn = STBridge201.StbColumn;

namespace STBDiffChecker.v201.Records
{
    internal static class Posts
    {
        internal static double analysisMargin = Utility.Tolerance;
        internal static double offsetMargin = Utility.Tolerance;

        private class CashPost
        {
            internal readonly StbPost cashPost;
            internal readonly StbNode cashNodeBottom;
            internal readonly StbNode cashNodeTop;

            internal CashPost(StbPost post, ST_BRIDGE stBridge)
            {
                cashPost = post;
                cashNodeBottom = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == post.id_node_bottom);
                cashNodeTop = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == post.id_node_top);
            }
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbPosts;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbPosts;
            List<CashPost> cashPosts = new List<CashPost>();

            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashPosts.Add(new CashPost(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashPost>(cashPosts);

            if (membersA != null)
            {
                foreach (var a in membersA)
                {
                    var bottomA = stBridgeA?.StbModel?.StbNodes.First(n => n.id == a.id_node_bottom);
                    var topA = stBridgeA?.StbModel?.StbNodes.First(n => n.id == a.id_node_top);
                    var key = new List<string>
                    {
                        $"bottom=({bottomA.X},{bottomA.Y},{bottomA.Z})",
                        $"top=({topA.X},{topA.Y},{topA.Z})"
                    };

                    bool hasItem = false;
                    foreach (var b in cashPosts)
                    {
                        if (Nodes.CheckAnalysisDistance(bottomA, b.cashNodeBottom, analysisMargin) && Nodes.CheckAnalysisDistance(topA, b.cashNodeTop, analysisMargin))
                        {
                            if (Nodes.CheckAnalysisDistance(bottomA, b.cashNodeBottom, Utility.Tolerance) &&
                                Nodes.CheckAnalysisDistance(topA, b.cashNodeTop, Utility.Tolerance))
                            {
                                CheckObjects.StbColumn.AppendConcistentRecord(nameof(StbColumn), key, records);
                            }
                            else
                            {
                                CheckObjects.StbColumn.AppendConcistentRecord(nameof(StbColumn), key, records, true);
                            }
                            ComparePost(stBridgeA, stBridgeB, a, b.cashPost, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbColumn.Compare(nameof(StbColumn), null, key, records);
                    }
                }

                foreach (var b in setB)
                {
                    var key = new List<string>
                    {
                        $"bottom=({b.cashNodeBottom.X},{b.cashNodeBottom.Y},{b.cashNodeBottom.Z})",
                        $"top=({b.cashNodeTop.X},{b.cashNodeTop.Y},{b.cashNodeTop.Z})"
                    };
                    CheckObjects.StbColumn.Compare(null, nameof(StbColumn), key, records);
                }
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void ComparePost(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbPost columnA, StbPost columnB, IReadOnlyList<string> key, List<Record> records)
        {
            StbColumnId.Compare(columnA.id, columnB.id, key, records);
            StbColumnGuid.Compare(columnA.guid, columnB.guid, key, records);
            StbColumnName.Compare(columnA.name, columnB.name, key, records);
            StbColumnIdNodeBottom.Compare(columnA.id_node_bottom, stBridgeA, columnB.id_node_bottom, stBridgeB, analysisMargin, key, records);
            StbColumnIdNodeBottom.Compare(columnA.id_node_top, stBridgeA, columnB.id_node_top, stBridgeB, analysisMargin, key, records);
            StbColumnRotate.Compare(columnA.rotate, columnB.rotate, key, records);
            StbColumnIdSection.CompareColumnSection(columnA.id_section, columnA.kind_structure.ToString(), stBridgeA,
                columnB.id_section, columnB.kind_structure.ToString(), stBridgeB, key, records);
            StbColumnKindStructure.Compare(columnA.kind_structure.ToString(), columnB.kind_structure.ToString(), key, records);
            StbColumnOffsetBottomX.Compare(columnA.offset_bottom_XSpecified, columnA.offset_bottom_X,
                columnB.offset_bottom_XSpecified, columnB.offset_bottom_X, key, records, offsetMargin);
            StbColumnOffsetBottomX.Compare(columnA.offset_bottom_YSpecified, columnA.offset_bottom_Y,
                columnB.offset_bottom_YSpecified, columnB.offset_bottom_Y, key, records, offsetMargin);
            StbColumnOffsetBottomX.Compare(columnA.offset_bottom_ZSpecified, columnA.offset_bottom_Z,
                columnB.offset_bottom_ZSpecified, columnB.offset_bottom_Z, key, records, offsetMargin);
            StbColumnOffsetTopX.Compare(columnA.offset_top_XSpecified, columnA.offset_top_X,
                columnB.offset_top_XSpecified, columnB.offset_top_X, key, records, offsetMargin);
            StbColumnOffsetTopY.Compare(columnA.offset_top_YSpecified, columnA.offset_top_Y,
                columnB.offset_top_YSpecified, columnB.offset_top_Y, key, records, offsetMargin);
            StbColumnOffsetTopZ.Compare(columnA.offset_top_ZSpecified, columnA.offset_top_Z,
                columnB.offset_top_ZSpecified, columnB.offset_top_Z, key, records, offsetMargin);
            StbColumnThicknessAddStartX.Compare(columnA.thickness_add_start_XSpecified, columnA.thickness_add_start_X,
                columnB.thickness_add_start_XSpecified, columnB.thickness_add_start_X, key, records);
            StbColumnThicknessAddEndX.Compare(columnA.thickness_add_end_XSpecified, columnA.thickness_add_end_X,
                columnB.thickness_add_end_XSpecified, columnB.thickness_add_end_X, key, records);
            StbColumnThicknessAddStartY.Compare(columnA.thickness_add_start_YSpecified, columnA.thickness_add_start_Y,
                columnB.thickness_add_start_YSpecified, columnB.thickness_add_start_Y, key, records);
            StbColumnThicknessAddEndY.Compare(columnA.thickness_add_end_YSpecified, columnA.thickness_add_end_Y,
                columnB.thickness_add_end_YSpecified, columnB.thickness_add_end_Y, key, records);
            StbColumnConditionBottom.Compare(columnA.condition_bottom.ToString(), columnB.condition_bottom.ToString(), key, records);
            StbColumnConditionTop.Compare(columnA.condition_top.ToString(), columnB.condition_top.ToString(), key, records);
            StbColumnJointTop.Compare(columnA.joint_topSpecified, columnA.joint_top, columnB.joint_topSpecified, columnB.joint_top, key, records);
            StbColumnJointBottom.Compare(columnA.joint_bottomSpecified, columnA.joint_bottom, columnB.joint_bottomSpecified, columnB.joint_bottom, key, records);
            StbColumnKindJointTop.Compare(columnA.kind_joint_top.ToString(), columnB.kind_joint_bottom.ToString(), key, records);
            StbColumnKindJointBottom.Compare(columnA.kind_joint_bottom.ToString(), columnB.kind_joint_bottom.ToString(), key, records);
            StbColumnJointIdTop.Compare(columnA.joint_id_top, stBridgeA, columnB.joint_id_top, stBridgeB, key, records);
            StbColumnJointIdBottom.Compare(columnA.joint_id_bottom, stBridgeA, columnB.joint_id_bottom, stBridgeB, key, records);


        }
    }
}

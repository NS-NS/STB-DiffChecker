using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbWall = STBridge201.StbWall;
using StbWallOffset = STBridge201.StbWallOffset;
using StbWallOffsetList = STBridge201.StbWallOffsetList;

namespace STBDiffChecker.v201.Records
{
    internal static class Walls
    {
        internal static double analysisMargin = Utility.Tolerance;
        internal static double offsetMargin = Utility.Tolerance;

        private class CashWall
        {
            internal StbWall cashWall;
            internal List<StbNode> cashNodes;

            internal CashWall(StbWall wall, ST_BRIDGE stBridge)
            {
                cashWall = wall;
                cashNodes = GetNodes(wall, stBridge);
            }
        }

        private static List<StbNode> GetNodes(StbWall wall, ST_BRIDGE stBridge)
        {
            var nodeList = new List<StbNode>();
            var split = wall.StbNodeIdOrder.Split(' ');
            foreach (var s in split)
            {
                var temp = stBridge?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == s);
                if (temp != null)
                {
                    nodeList.Add(temp);
                }
                else
                {
                    nodeList = null;
                    break;
                }
            }

            return nodeList;
        }

        private static List<string> GetKey(List<StbNode> nodes)
        {
            var key = new List<string>();
            for (int i = 0; i < nodes.Count; i++)
            {
                key.Add($"節点{i + 1}=({nodes[i].X},{nodes[i].Y},{nodes[i].Z})");
            }

            return key;
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbWalls;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbWalls;
            List<CashWall> cashWalls = new List<CashWall>();
            if (membersB != null)
            {
                foreach (var b in membersB)
                {
                    cashWalls.Add(new CashWall(b, stBridgeB));
                }
            }

            var setB = new HashSet<CashWall>(cashWalls);

            if (membersA != null)
            {
                foreach (var a in membersA)
                {
                    var nodeListA = GetNodes(a, stBridgeA);
                    if (nodeListA == null)
                    {
                        throw new Exception();
                    }

                    var key = GetKey(nodeListA);

                    bool hasItem = false;
                    foreach (var cash in cashWalls)
                    {
                        if (IsSameNodes(nodeListA, cash.cashNodes))
                        {
                            var b = cash.cashWall;
                            StbWallId.Compare(a.id, b.id, key, records);
                            StbWallGuid.Compare(a.guid, b.guid, key, records);
                            StbWallName.Compare(a.name, b.name, key, records);
                            StbWallIdSection.CompareWallSection(a.id_section, a.kind_structure.ToString(), stBridgeA,
                                b.id_section, b.kind_structure.ToString(), stBridgeB, key, records);
                            StbWallKindStructure.Compare(a.kind_structure, b.kind_structure, key, records);
                            StbWallThicknessAddRight.Compare(a.thickness_add_rightSpecified, a.thickness_add_right,
                                b.thickness_add_rightSpecified, b.thickness_add_right, key, records);
                            StbWallThicknessAddLeft.Compare(a.thickness_add_leftSpecified, a.thickness_add_left,
                                b.thickness_add_leftSpecified, b.thickness_add_left, key, records);
                            StbWallKindWall.Compare(a.kind_wall.ToString(), b.kind_wall.ToString(), key, records);
                            StbWallSlitUpper.Compare(a.slit_upperSpecified, a.slit_upper, b.slit_upperSpecified,
                                b.slit_upper, key, records);
                            StbWallSlitBottom.Compare(a.slit_bottomSpecified, a.slit_bottom, b.slit_bottomSpecified,
                                b.slit_bottom, key, records);
                            StbWallSlitRight.Compare(a.slit_rightSpecified, a.slit_right, b.slit_rightSpecified,
                                b.slit_right, key, records);
                            StbWallSlitLeft.Compare(a.slit_leftSpecified, a.slit_left, b.slit_leftSpecified,
                                b.slit_left, key, records);
                            StbWallTypeOutside.Compare(a.type_outside.ToString(), b.type_outside.ToString(), key, records);
                            StbWallIsPress.Compare(a.isPress, b.isPress, key, records);
                            // offsetList用にNodeのペアーを作成
                            var nodeDictionary = new Dictionary<string, StbNode>();
                            for (int i = 0; i < nodeListA.Count; i++)
                            {
                                StbWallStbNodeIdOrderId.Compare(nodeListA[i], cash.cashNodes[i], analysisMargin, key, records);
                                nodeDictionary.Add(nodeListA[i].id, cash.cashNodes[i]);
                            }

                            if (a.StbWallOffsetList != null || b.StbWallOffsetList != null)
                            {
                                if (b.StbWallOffsetList == null)
                                {
                                    CheckObjects.StbWallOffsetList.Compare(nameof(StbWallOffsetList), null, key, records);
                                }
                                else if (a.StbWallOffsetList == null)
                                {
                                    CheckObjects.StbWallOffsetList.Compare(null, nameof(StbWallOffsetList), key, records);
                                }
                                else
                                {
                                    var set = new HashSet<StbWallOffset>(b.StbWallOffsetList);
                                    foreach (var t in a.StbWallOffsetList)
                                    {
                                        bool hasOffset = false;
                                        foreach (var stbWallOffset in b.StbWallOffsetList)
                                        {
                                            if (stbWallOffset.id_node == nodeDictionary[t.id_node].id)
                                            {
                                                var node = nodeDictionary[t.id_node];
                                                var key1 = new List<string>(key) { $"offset({node.X},{node.Y},{node.Z})" };
                                                // Keyだから不要？
                                                // CheckObjects.StbWallOffsetIdNode.Compare(t.id_node, stbWallOffset.id_node, key1, records);
                                                CheckObjects.StbWallOffsetOffsetX.Compare(t.offset_X, stbWallOffset.offset_X, key1, records, offsetMargin);
                                                CheckObjects.StbWallOffsetOffsetY.Compare(t.offset_Y, stbWallOffset.offset_Y, key1, records, offsetMargin);
                                                CheckObjects.StbWallOffsetOffsetZ.Compare(t.offset_Z, stbWallOffset.offset_Z, key1, records, offsetMargin);
                                                set.Remove(stbWallOffset);
                                                hasOffset = true;
                                            }
                                        }

                                        if (!hasOffset)
                                            CheckObjects.StbWallOffset.Compare(nameof(StbWallOffset), null, key, records);
                                    }

                                    foreach (var offsetB in set)
                                    {
                                        var node = stBridgeB?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == offsetB.id_node);
                                        var key1 = new List<string>(key) { $"offset=({node.X},{ node.Y},{node.Z})" };
                                        CheckObjects.StbWallOffset.Compare(null, nameof(StbWallOffset), key, records);
                                    }
                                }
                            }

                            if (a.StbOpenIdList != null || b.StbOpenIdList != null)
                            {
                                if (b.StbOpenIdList == null)
                                {
                                    StbWallStbOpenIdList.Compare(nameof(StbOpenIdList), null, key, records);
                                }
                                else if (b.StbNodeIdOrder == null)
                                {
                                    StbWallStbOpenIdList.Compare(null, nameof(StbOpenIdList), key, records);
                                }
                                else
                                {
                                    var openSet = new HashSet<StbOpenId>(b.StbOpenIdList);
                                    foreach (var idA in a.StbOpenIdList)
                                    {
                                        bool hasOpen = false;
                                        var openA = stBridgeA?.StbModel?.StbMembers?.StbOpens?.FirstOrDefault(n =>
                                            n.id == idA.id);
                                        foreach (var idB in b.StbOpenIdList)
                                        {
                                            var openB = stBridgeB?.StbModel?.StbMembers?.StbOpens?.FirstOrDefault(n =>
                                                n.id == idB.id);
                                            if ((Math.Abs(openA.length_X - openB.length_X) < Utility.Tolerance) &&
                                                (Math.Abs(openA.length_Y - openB.length_Y) < Utility.Tolerance) &&
                                                (Math.Abs(openA.position_X - openB.position_X) < Utility.Tolerance) &&
                                                (Math.Abs(openA.position_Y - openB.position_Y) < Utility.Tolerance) &&
                                                Math.Abs(openA.rotate - openB.rotate) < Utility.Tolerance)
                                            {
                                                openSet.Remove(idB);
                                                hasOpen = true;
                                            }
                                        }

                                        if (!hasOpen)
                                        {
                                            var key1 = new List<string>(key)
                                            {
                                                $"length_X={openA.length_X}, length_Y={openA.length_Y}",
                                                $"position_X={openA.position_X}, position_Y={openA.position_Y}",
                                                $"rotate={openA.rotate}"
                                            };
                                            StbWallStbOpenIdList.Compare(nameof(StbOpen), null, key, records);
                                        }
                                    }

                                    foreach (var idB in openSet)
                                    {
                                        var open = stBridgeB?.StbModel?.StbMembers?.StbOpens?.FirstOrDefault(n =>
                                            n.id == idB.id);
                                        var key1 = new List<string>(key)
                                        {
                                            $"length_X={open.length_X}, length_Y={open.length_Y}",
                                            $"position_X={open.position_X}, position_Y={open.position_Y}",
                                            $"rotate={open.rotate}"
                                        };
                                        StbWallStbOpenIdList.Compare(null, nameof(StbOpen), key, records);
                                    }
                                }
                            }

                            setB.Remove(cash);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbWall.Compare(nameof(StbWall), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                CheckObjects.StbWall.Compare(null, nameof(StbWall), GetKey(b.cashNodes), records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static bool IsSameNodes(List<StbNode> nodeListA, List<StbNode> nodeListB)
        {
            if (nodeListA.Count != nodeListB.Count)
                return false;
            for (int i = 0; i < nodeListA.Count; i++)
            {
                if (!Nodes.CheckAnalysisDistance(nodeListA[i], nodeListB[i], analysisMargin))
                    return false;
            }

            return true;
        }
    }
}

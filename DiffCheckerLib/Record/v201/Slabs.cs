using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbSlab = STBridge201.StbSlab;
using StbSlabOffset = STBridge201.StbSlabOffset;
using StbSlabOffsetList = STBridge201.StbSlabOffsetList;

namespace STBDiffChecker.v201.Records
{
    internal static class Slabs
    {
        internal static double analysisMargin = Utility.Tolerance;
        internal static double offsetMargin = Utility.Tolerance;

        private class CashSlab
        {
            internal StbSlab cashSlab;
            internal List<StbNode> cashNodes;

            internal CashSlab(StbSlab slab, ST_BRIDGE stBridge)
            {
                cashSlab = slab;
                cashNodes = GetNodes(slab, stBridge);
            }
        } 

        private static List<StbNode> GetNodes(StbSlab slab, ST_BRIDGE stBridge)
        {
            var nodeList = new List<StbNode>();
            var split = slab.StbNodeIdOrder.Split(' ');
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
            for (int i=0; i<nodes.Count; i++)
            {
                key.Add($"節点{i+1}=({nodes[i].X},{nodes[i].Y},{nodes[i].Z})");
            }

            return key;
        }

        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var membersA = stBridgeA?.StbModel?.StbMembers?.StbSlabs;
            var membersB = stBridgeB?.StbModel?.StbMembers?.StbSlabs;


            List<CashSlab> cashSlabs = new List<CashSlab>();
            if (membersB != null)
            {
                foreach (var stbSlab in membersB)
                {
                    cashSlabs.Add(new CashSlab(stbSlab, stBridgeB));
                }
            }

            var setB = new HashSet<CashSlab>(cashSlabs);

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
                    foreach (var cash in cashSlabs)
                    {
                        if (IsSameNodes(nodeListA, cash.cashNodes))
                        {
                            var b = cash.cashSlab;
                            StbSlabId.Compare(a.id, b.id, key, records);
                            StbSlabGuid.Compare(a.guid, b.guid, key, records);
                            StbSlabName.Compare(a.name, b.name, key, records);
                            StbSlabIdSection.CompareSlabSection(a.id_section, a.kind_structure.ToString(), stBridgeA,
                                b.id_section, b.kind_structure.ToString(), stBridgeB, key, records);
                            StbSlabKindStructure.Compare(a.kind_structure.ToString(), b.kind_structure.ToString(), key, records);
                            StbSlabKindSlab.Compare(a.kind_slab.ToString(), b.kind_slab.ToString(), key, records);
                            StbSlabThicknessAddTop.Compare(a.thickness_add_topSpecified, a.thickness_add_top,
                                b.thickness_add_topSpecified, b.thickness_add_top, key, records);
                            StbSlabThicknessAddBottom.Compare(a.thickness_add_bottomSpecified, a.thickness_add_bottom,
                                b.thickness_add_bottomSpecified, b.thickness_add_bottom, key, records);
                            StbSlabDirectionLoad.Compare(a.direction_loadSpecified, a.direction_load.ToString(), b.direction_loadSpecified,
                                b.direction_load.ToString(), key, records);
                            StbSlabAngleLoad.Compare(a.angle_loadSpecified, a.angle_load, b.angle_loadSpecified,
                                b.angle_load, key, records);
                            StbSlabAngleMainBarDirection.Compare(a.angle_main_bar_directionSpecified,
                                a.angle_main_bar_direction, b.angle_main_bar_directionSpecified,
                                b.angle_main_bar_direction, key, records);
                            StbSlabIsFoundation.Compare(a.isFoundation, b.isFoundation, key, records);
                            StbSlabTypeHaunch.Compare(a.type_haunchSpecified, a.type_haunch.ToString(),
                                b.type_haunchSpecified, b.type_haunch.ToString(), key, records);
                            // offsetList用にNodeのペアーを作成
                            var nodeDictionary = new Dictionary<string, StbNode>();
                            for (int i = 0; i < nodeListA.Count; i++)
                            {
                                StbSlabStbNodeIdOrderId.Compare(nodeListA[i], cash.cashNodes[i], analysisMargin, key, records);
                                nodeDictionary.Add(nodeListA[i].id, cash.cashNodes[i]);
                            }

                            if (a.StbSlabOffsetList != null || b.StbSlabOffsetList != null)
                            {
                                if (b.StbSlabOffsetList == null)
                                {
                                    CheckObjects.StbSlabOffsetList.Compare(nameof(StbSlabOffsetList), null, key, records);
                                }
                                else if (a.StbSlabOffsetList == null)
                                {
                                    CheckObjects.StbSlabOffsetList.Compare(null, nameof(StbSlabOffsetList), key, records);
                                }
                                else
                                {
                                    var set = new HashSet<StbSlabOffset>(b.StbSlabOffsetList);
                                    foreach (var t in a.StbSlabOffsetList)
                                    {
                                        bool hasOffset = false;
                                        foreach (var stbSlabOffset in b.StbSlabOffsetList)
                                        {
                                            if (stbSlabOffset.id_node == nodeDictionary[t.id_node].id)
                                            {
                                                var node = nodeDictionary[t.id_node];
                                                var key1 = new List<string>(key){$"offset({node.X},{node.Y},{node.Z})"};
                                                // Keyだから不要？
                                                // CheckObjects.StbSlabOffsetIdNode.Compare(t.id_node, stbSlabOffset.id_node, key1, records);
                                                CheckObjects.StbSlabOffsetOffsetX.Compare(t.offset_X, stbSlabOffset.offset_X, key1, records);
                                                CheckObjects.StbSlabOffsetOffsetY.Compare(t.offset_Y, stbSlabOffset.offset_Y, key1, records);
                                                CheckObjects.StbSlabOffsetOffsetZ.Compare(t.offset_Z, stbSlabOffset.offset_Z, key1, records);
                                                set.Remove(stbSlabOffset);
                                                hasOffset = true;
                                            }
                                        }

                                        if (!hasOffset)
                                            CheckObjects.StbSlabOffset.Compare(nameof(StbSlabOffset), null, key, records);
                                    }

                                    foreach (var offsetB in set)
                                    {
                                        var node = stBridgeB?.StbModel?.StbNodes?.FirstOrDefault(n => n.id == offsetB.id_node);
                                        var key1 = new List<string>(key){$"offset=({node.X},{ node.Y},{node.Z})"};
                                        CheckObjects.StbSlabOffset.Compare(null, nameof(StbSlabOffset), key, records);
                                    }
                                }
                            }

                            if (a.StbOpenIdList != null || b.StbOpenIdList != null)
                            {
                                if (b.StbOpenIdList == null)
                                {
                                    StbSlabStbOpenIdList.Compare(nameof(StbOpenIdList), null, key, records);
                                }
                                else if (a.StbOpenIdList == null)
                                {
                                    StbSlabStbOpenIdList.Compare(null, nameof(StbOpenIdList), key, records);
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
                                            StbSlabStbOpenIdList.Compare(nameof(StbOpen), null, key, records);
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
                                        StbSlabStbOpenIdList.Compare(null, nameof(StbOpen), key, records);
                                    }
                                }
                            }

                            setB.Remove(cash);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbSlab.Compare(nameof(StbSlab), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                CheckObjects.StbSlab.Compare(null, nameof(StbSlab), GetKey(b.cashNodes), records);
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

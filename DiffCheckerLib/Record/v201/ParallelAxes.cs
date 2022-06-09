using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.v201.Records
{
    class ParallelAxes
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var axesA = stBridgeA?.StbModel?.StbAxes?.StbParallelAxes;
            var axesB = stBridgeB?.StbModel?.StbAxes?.StbParallelAxes;
            var setB = axesB != null ? new HashSet<StbParallelAxes>(axesB) : new HashSet<StbParallelAxes>();

            if (axesA != null)
            {
                foreach (var a in axesA)
                {
                    var key = new List<string> { $"group_name={a.group_name}"};
                    var b = axesB?.SingleOrDefault(n => n.group_name == a.group_name);
                    if (b == null)
                    {
                        CheckObjects.StbParallelAxes.Compare(nameof(StbParallelAxes), null, key, records);
                    }
                    else
                    {
                        CheckObjects.StbParallelAxesGroupName.Compare(a.group_name, b.group_name, key, records);
                        CheckObjects.StbParallelAxesX.Compare(a.X, b.X, key, records);
                        CheckObjects.StbParallelAxesY.Compare(a.Y, b.Y, key, records);
                        CheckObjects.StbParallelAxesAngle.Compare(a.angle, b.angle, key, records);

                        var set2 = new HashSet<StbParallelAxis>(b.StbParallelAxis);
                        foreach (var axisA in a.StbParallelAxis)
                        {
                            var key2 = new List<string>(key) { $"name={axisA.name}" };
                            bool hasItem2 = false;
                            foreach (var axisB in b.StbParallelAxis)
                            {
                                if (axisB.name == axisA.name)
                                {
                                    CheckObjects.StbParallelAxisId.Compare(axisA.id, axisB.id, key2, records);
                                    CheckObjects.StbParallelAxisGuid.Compare(axisA.guid, axisB.guid, key2, records);
                                    CheckObjects.StbParallelAxisName.Compare(axisA.name, axisB.name, key2, records);
                                    CheckObjects.StbParallelAxisDistance.Compare(axisA.distance, axisB.distance, key2, records);

                                    if (axisA.StbNodeIdList != null || axisB.StbNodeIdList != null)
                                    {
                                        if (axisB.StbNodeIdList == null)
                                        {
                                            CheckObjects.StbParallelAxisStbNodeIdList.Compare(nameof(StbNodeIdList), null, key2, records);
                                        }
                                        else if (axisA.StbNodeIdList == null)
                                        {
                                            CheckObjects.StbParallelAxisStbNodeIdList.Compare(null, nameof(StbNodeIdList), key2, records);
                                        }
                                        else
                                        {
                                            CompareStbNodeIdList(axisA.StbNodeIdList, stBridgeA, axisB.StbNodeIdList,
                                                stBridgeB, key2, records);
                                        }
                                    }

                                    set2.Remove(axisB);
                                    hasItem2 = true;
                                }
                            }

                            if (!hasItem2)
                            {
                                CheckObjects.StbParallelAxis.Compare(nameof(StbParallelAxis), null, key2, records);
                            }
                        }

                        foreach (var axisB in set2)
                        {
                            var key2 = new List<string>(key) { $"name={axisB.name}" };
                            CheckObjects.StbParallelAxis.Compare(null, nameof(StbParallelAxis), key2, records);
                        }

                        setB.Remove(b);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"group_name={b.group_name}"};
                CheckObjects.StbParallelAxes.Compare(null, nameof(StbParallelAxes), key, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareStbNodeIdList(StbNodeId[] nodeIdA, ST_BRIDGE stBridgeA, StbNodeId[] nodeIdB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var set = new Dictionary<StbNodeId, StbNode>();
            foreach (var idA in nodeIdA)
            {
                var nodeA = stBridgeA.StbModel.StbNodes.First(n => n.id == idA.id);
                bool hasItem = false;
                var key2 = new List<string>(key) { $"({nodeA.X},{nodeA.Y},{nodeA.Z})" };
                foreach (var idB in nodeIdB)
                {
                    var nodeB = stBridgeB.StbModel.StbNodes.First(n => n.id == idB.id);
                    if (Math.Abs(nodeA.X - nodeB.X) < Utility.Tolerance &&
                        Math.Abs(nodeA.Y - nodeB.Y) < Utility.Tolerance &&
                        Math.Abs(nodeA.Z - nodeB.Z) < Utility.Tolerance)
                    {
                        CheckObjects.StbParallelAxisStbNodeIdListStbNodeIdId.Compare(idA.id, stBridgeA, idB.id,
                            stBridgeB, key2, records);
                        set.Add(idB, nodeB);
                        hasItem = true;
                    }
                }
                if (!hasItem)
                {
                    CheckObjects.StbParallelAxisStbNodeIdListStbNodeId.Compare(nameof(StbNodeId), null, key2,
                        records);
                }
            }

            foreach (var node in nodeIdB)
            {
                if (!set.ContainsKey(node))
                {
                    var stbNode = stBridgeB.StbModel.StbNodes.First(n => n.id == node.id);
                    var key2 = new List<string>(key) { $"({stbNode.X},{stbNode.Y},{stbNode.Z})" };
                    CheckObjects.StbParallelAxisStbNodeIdListStbNodeId.Compare(null, nameof(StbNodeId), key2, records);
                }
            }

        }
    }
}

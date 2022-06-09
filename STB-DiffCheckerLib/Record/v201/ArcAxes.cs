using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.v201.Records
{
    internal static class ArcAxes
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var axesA = stBridgeA?.StbModel?.StbAxes?.StbArcAxes;
            var axesB = stBridgeB?.StbModel?.StbAxes?.StbArcAxes;
            var setB = axesB != null ? new HashSet<StbArcAxes>(axesB) : new HashSet<StbArcAxes>();

            if (axesA != null)
            {
                foreach (var a in axesA)
                {
                    var key = new List<string> { $"group_name={a.group_name}"};
                    var b = axesB?.SingleOrDefault(n => n.group_name == a.group_name);
                    if (b == null)
                    {
                        CheckObjects.StbArcAxes.Compare(nameof(StbArcAxes), null, key, records);
                    }
                    else
                    {
                        CheckObjects.StbArcAxesGroupName.Compare(a.group_name, b.group_name, key, records);
                        CheckObjects.StbArcAxesX.Compare(a.X, b.X, key, records);
                        CheckObjects.StbArcAxesY.Compare(a.Y, b.Y, key, records);
                        CheckObjects.StbArcAxesStartAngle.Compare(a.start_angle, b.start_angle, key, records);
                        CheckObjects.StbArcAxesEndAngle.Compare(a.end_angle, b.end_angle, key, records);

                        var set2 = new HashSet<StbArcAxis>(b.StbArcAxis);
                        foreach (var axisA in a.StbArcAxis)
                        {
                            var key2 = new List<string>(key) { $"name={ axisA.name }" };
                            bool hasItem2 = false;
                            foreach (var axisB in b.StbArcAxis)
                            {
                                if (axisB.name == axisA.name)
                                {
                                    CheckObjects.StbArcAxisId.Compare(axisA.id, axisB.id, key2, records);
                                    CheckObjects.StbArcAxisGuid.Compare(axisA.guid, axisB.guid, key2, records);
                                    CheckObjects.StbArcAxisName.Compare(axisA.name, axisB.name, key2, records);
                                    CheckObjects.StbArcAxisRadius.Compare(axisA.radius, axisB.radius, key2, records);

                                    if (axisA.StbNodeIdList != null || axisB.StbNodeIdList != null)
                                    {
                                        if (axisB.StbNodeIdList == null)
                                        {
                                            CheckObjects.StbArcAxisStbNodeIdList.Compare(nameof(StbNodeIdList), null, key, records);
                                        }
                                        else if (axisA.StbNodeIdList == null)
                                        {
                                            CheckObjects.StbArcAxisStbNodeIdList.Compare(null, nameof(StbNodeIdList), key, records);
                                        }
                                        else
                                        {
                                            CompareStbNodeIdList(axisA.StbNodeIdList, stBridgeA, axisB.StbNodeIdList,
                                                stBridgeB, key, records);
                                        }
                                    }

                                    set2.Remove(axisB);
                                    hasItem2 = true;
                                }
                            }

                            if (!hasItem2)
                            {
                                CheckObjects.StbArcAxis.Compare(nameof(StbArcAxis), null, key2, records);
                            }
                        }

                        foreach (var axisB in set2)
                        {
                            var key2 = new List<string>(key) { $"name={ axisB.name }" };
                            CheckObjects.StbArcAxis.Compare(null, nameof(StbArcAxis), key2, records);
                        }

                        setB.Remove(b);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"group_name={b.group_name}"};
                CheckObjects.StbArcAxes.Compare(null, nameof(StbArcAxes), key, records);
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
                var key2 = new List<string>(key) { $"節点=({nodeA.X},{nodeA.Y},{nodeA.Z})" };
                foreach (var idB in nodeIdB)
                {
                    var nodeB = stBridgeB.StbModel.StbNodes.First(n => n.id == idB.id);
                    if (Math.Abs(nodeA.X - nodeB.X) < Utility.Tolerance &&
                        Math.Abs(nodeA.Y - nodeB.Y) < Utility.Tolerance &&
                        Math.Abs(nodeA.Z - nodeB.Z) < Utility.Tolerance)
                    {
                        CheckObjects.StbArcAxisStbNodeIdListStbNodeIdId.Compare(idA.id, stBridgeA, idB.id,
                            stBridgeB, key2, records);
                        set.Add(idB, nodeB);
                        hasItem = true;
                    }
                }
                if (!hasItem)
                {
                    CheckObjects.StbArcAxisStbNodeIdListStbNodeId.Compare(nameof(StbNodeId), null, key2,
                        records);
                }
            }

            foreach (var node in nodeIdB)
            {
                if (!set.ContainsKey(node))
                {
                    var key2 = new List<string>(key) { $"節点=({ set[node].X},{set[node].Y},{set[node].Z})"};
                    CheckObjects.StbArcAxisStbNodeIdListStbNodeId.Compare(null, nameof(StbNodeId), key2, records);
                }
            }

        }
    }
}

using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.v201.Records
{
    internal static class RadialAxes
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var axesA = stBridgeA?.StbModel?.StbAxes?.StbRadialAxes;
            var axesB = stBridgeB?.StbModel?.StbAxes?.StbRadialAxes;
            var setB = axesB != null ? new HashSet<StbRadialAxes>(axesB) : new HashSet<StbRadialAxes>();

            if (axesA != null)
            {
                foreach (var a in axesA)
                {
                    var key = new List<string> { $"group_name={a.group_name}"};
                    var b = axesB?.SingleOrDefault(n => n.group_name == a.group_name);
                    if (b == null)
                    {
                        CheckObjects.StbRadialAxes.Compare(nameof(StbRadialAxes), null, key, records);
                    }
                    else
                    {
                        CheckObjects.StbRadialAxesGroupName.Compare(a.group_name, b.group_name, key, records);
                        CheckObjects.StbRadialAxesX.Compare(a.X, b.X, key, records);
                        CheckObjects.StbRadialAxesY.Compare(a.Y, b.Y, key, records);

                        var set2 = new HashSet<StbRadialAxis>(b.StbRadialAxis);
                        foreach (var axisA in a.StbRadialAxis)
                        {
                            var key2 = new List<string>(key) { $"name={axisA.name}" };
                            bool hasItem2 = false;
                            foreach (var axisB in b.StbRadialAxis)
                            {
                                if (axisB.name == axisA.name)
                                {
                                    CheckObjects.StbRadialAxisId.Compare(axisA.id, axisB.id, key2, records);
                                    CheckObjects.StbRadialAxisGuid.Compare(axisA.guid, axisB.guid, key2, records);
                                    CheckObjects.StbRadialAxisName.Compare(axisA.name, axisB.name, key2, records);
                                    CheckObjects.StbRadialAxisAngle.Compare(axisA.angle, axisB.angle, key2, records);

                                    if (axisA.StbNodeIdList != null || axisB.StbNodeIdList != null)
                                    {
                                        if (axisB.StbNodeIdList == null)
                                        {
                                            CheckObjects.StbRadialAxisStbNodeIdList.Compare(nameof(StbNodeIdList), null, key, records);
                                        }
                                        else if (axisA.StbNodeIdList == null)
                                        {
                                            CheckObjects.StbRadialAxisStbNodeIdList.Compare(null, nameof(StbNodeIdList), key, records);
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
                                CheckObjects.StbRadialAxis.Compare(nameof(StbRadialAxis), null, key2, records);
                            }
                        }

                        foreach (var axisB in set2)
                        {
                            var key2 = new List<string>(key) { $"name={axisB.name}" };
                            CheckObjects.StbRadialAxis.Compare(null, nameof(StbRadialAxis), key2, records);
                        }
                        setB.Remove(b);

                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"group_name={b.group_name}"};
                CheckObjects.StbRadialAxes.Compare(null, nameof(StbRadialAxes), key, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareStbNodeIdList(StbNodeId[] nodeIdA, ST_BRIDGE stBridgeA, StbNodeId[] nodeIdB, ST_BRIDGE stBridgeB, List<string> key, List<Record> records)
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
                        CheckObjects.StbRadialAxisStbNodeIdListStbNodeIdId.Compare(idA.id, stBridgeA, idB.id,
                            stBridgeB, key2, records);
                        set.Add(idB, nodeB);
                        hasItem = true;
                    }
                }
                if (!hasItem)
                {
                    CheckObjects.StbRadialAxisStbNodeIdListStbNodeId.Compare(nameof(StbNodeId), null, key2,
                        records);
                }
            }

            foreach (var node in nodeIdB)
            {
                if (!set.ContainsKey(node))
                {
                    var key2 = new List<string>(key) { $"({set[node].X},{set[node].Y},{set[node].Z})" };
                    CheckObjects.StbRadialAxisStbNodeIdListStbNodeId.Compare(nameof(StbNodeId), null, key2, records);
                }
            }

        }
    }
}

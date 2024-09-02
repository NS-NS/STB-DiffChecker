using STBridge201;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.v201.Records
{
    class DrawingLineAxis
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var lineA = stBridgeA?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis;
            var lineB = stBridgeB?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis;
            var setB = lineB != null ? new HashSet<StbDrawingLineAxis>(lineB) : new HashSet<StbDrawingLineAxis>();

            if (lineA != null)
            {
                foreach (var a in lineA)
                {
                    var key = new List<string> { $"name={a.name}"};
                    bool hasItem = false;
                    if (lineB != null)
                    {
                        foreach (var b in lineB.Where(n => n.name == a.name))
                        {
                            CheckObjects.StbDrawingLineAxis.AppendConcistentRecord(nameof(StbDrawingLineAxis), key, records);
                            CheckObjects.StbDrawingLineAxisGroupName.Compare(a.group_name, b.group_name, key, records);
                            CheckObjects.StbDrawingLineAxisName.Compare(a.name, b.name, key, records);
                            CheckObjects.StbDrawingLineAxisStartX.Compare(a.start_X, b.start_X, key, records);
                            CheckObjects.StbDrawingLineAxisStartY.Compare(a.start_Y, b.start_Y, key, records);
                            CheckObjects.StbDrawingLineAxisEndX.Compare(a.end_X, b.end_X, key, records);
                            CheckObjects.StbDrawingLineAxisEndY.Compare(a.end_Y, b.end_Y, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbStory.Compare(nameof(StbDrawingLineAxis), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"name={b.name}"};
                CheckObjects.StbDrawingLineAxis.Compare(null, nameof(StbDrawingLineAxis), key, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }
    }
}

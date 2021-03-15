using STBridge201;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.v201.Records
{
    internal static class DrawingArcAxis
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var lineA = stBridgeA?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis;
            var lineB = stBridgeB?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis;
            var setB = lineB != null ? new HashSet<StbDrawingArcAxis>(lineB) : new HashSet<StbDrawingArcAxis>();

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
                            CheckObjects.StbDrawingArcAxisGroupName.Compare(a.group_name, b.group_name, key, records);
                            CheckObjects.StbDrawingArcAxisName.Compare(a.name, b.name, key, records);
                            CheckObjects.StbDrawingArcAxisX.Compare(a.X, b.X, key, records);
                            CheckObjects.StbDrawingArcAxisY.Compare(a.Y, b.Y, key, records);
                            CheckObjects.StbDrawingArcAxisRadius.Compare(a.radius, b.radius, key, records);
                            CheckObjects.StbDrawingArcAxisStartAngle.Compare(a.start_angle, b.start_angle, key, records);
                            CheckObjects.StbDrawingArcAxisEndAngle.Compare(a.end_angle, b.end_angle, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbStory.Compare(nameof(StbDrawingArcAxis), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"name={b.name}"};
                CheckObjects.StbDrawingArcAxis.Compare(null, nameof(StbDrawingArcAxis), key, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }
    }
}

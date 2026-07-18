using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using STB_DiffChecker_202;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbGirder : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            if (obj is not StbGirder other)
            {
                return false;
            }
            StbNode startA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_start);
            StbNode endA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_end);
            StbNode startB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_start);
            StbNode endB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_end);

            return Math.Abs(startA.X - startB.X) < Utility.Tolerance &&
                   Math.Abs(startA.Y - startB.Y) < Utility.Tolerance &&
                   Math.Abs(startA.Z - startB.Z) < Utility.Tolerance &&
                   Math.Abs(endA.X - endB.X) < Utility.Tolerance &&
                   Math.Abs(endA.Y - endB.Y) < Utility.Tolerance &&
                   Math.Abs(endA.Z - endB.Z) < Utility.Tolerance;
        }

        public bool AlmostCompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB, IToleranceSetting itoleranceSetting)
        {
            if (obj is not StbColumn other)
            {
                return false;
            }
            ToleranceSetting? toleranceSetting = itoleranceSetting as ToleranceSetting;
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            StbNode startA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_start);
            StbNode endA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_end);
            StbNode startB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_bottom);
            StbNode endB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_top);

            return Math.Abs(startA.X - startB.X) < toleranceSetting.GirderTolerance.Node &&
                   Math.Abs(startA.Y - startB.Y) < toleranceSetting.GirderTolerance.Node &&
                   Math.Abs(startA.Z - startB.Z) < toleranceSetting.GirderTolerance.Node &&
                   Math.Abs(endA.X - endB.X) < toleranceSetting.GirderTolerance.Node &&
                   Math.Abs(endA.Y - endB.Y) < toleranceSetting.GirderTolerance.Node &&
                   Math.Abs(endA.Z - endB.Z) < toleranceSetting.GirderTolerance.Node;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE? stb = istb as ST_BRIDGE;
            StbNode bottom = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_start);
            StbNode top = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_end);
            return [$"start=({bottom.X},{bottom.Y},{bottom.Z})", $"end=({top.X},{top.Y},{top.Z})"];
        }

        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name is "id_section" or "offset_start_X" or "offset_start_Y" or "offset_start_Z" or "offset_end_X" or "offset_end_Y" or "offset_end_Z" or "joint_id_start" or "joint_id_end";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting itoleranceSetting)
        {
            ToleranceSetting? toleranceSetting = itoleranceSetting as ToleranceSetting;
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);

            if (info.Name == "id_section")
            {
                // kind
                PropertyInfo kind = this.GetType().GetProperty("kind_structure");
                object kindA = kind.GetValue(this);
                object kindB = kind.GetValue(objB);
                string beamA = FindBeamName(valueA.ToString(), kindA.ToString(), stbA);
                string beamB = FindBeamName(valueB.ToString(), kindB.ToString(), stbB);

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    beamA,
                    beamB,
                    beamA == beamB ? Consistency.Consistent : Consistency.Inconsistent,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));
            }
            else if (info.Name is "offset_start_X" or
                      "offset_start_Y" or
                      "offset_start_Z" or
                      "offset_end_X" or
                      "offset_end_Y" or
                      "offset_end_Z")
            {
                double offsetA = Convert.ToDouble(valueA);
                double offsetB = Convert.ToDouble(valueB);
                Consistency consistency = Math.Abs(offsetA - offsetB) < Utility.Tolerance
                    ? Consistency.Consistent
                    : Math.Abs(offsetA - offsetB) < toleranceSetting.GirderTolerance.Offset ? Consistency.AlmostMatch : Consistency.Inconsistent;
                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    offsetA.ToString(),
                    offsetB.ToString(),
                    consistency,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));
            }
            else if (info.Name is "joint_id_start" or "joint_id_end")
            {
                string jointA = string.Empty;
                if (stbA.StbModel.StbJoints.StbJointBeamShapeH.Any(n => n.id == valueA.ToString()))
                {
                    jointA = stbA.StbModel.StbJoints.StbJointBeamShapeH.First(n => n.id == valueA.ToString()).joint_mark;
                }

                string jointB = string.Empty;
                if (stbB.StbModel.StbJoints.StbJointBeamShapeH.Any(n => n.id == valueA.ToString()))
                {
                    jointB = stbB.StbModel.StbJoints.StbJointBeamShapeH.First(n => n.id == valueA.ToString()).joint_mark;
                }

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    jointA,
                    jointB,
                    jointA == jointB ? Consistency.Consistent : Consistency.Inconsistent,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));

            }
        }
        private string? FindBeamName(string id, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                StbSecBeam_RC beam = stbridge?.StbModel?.StbSections?.StbSecBeam_RC.FirstOrDefault(n => n.id == id);
                return beam.floor + "/" + beam.name;
            }
            else if (kind == "S")
            {
                StbSecBeam_S beam = stbridge?.StbModel?.StbSections?.StbSecBeam_S.FirstOrDefault(n => n.id == id);
                return beam.floor + "/" + beam.name;
            }
            else if (kind == "SRC")
            {
                StbSecBeam_SRC beam = stbridge?.StbModel?.StbSections?.StbSecBeam_SRC.FirstOrDefault(n => n.id == id);
                return beam.floor + "/" + beam.name;
            }
            else
            {
                return null;
            }
        }
    }
}
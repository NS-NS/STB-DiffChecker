using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using STB_DiffChecker_201;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbColumn : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            if (obj is not StbColumn other)
            {
                return false;
            }
            StbNode bottomA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_bottom);
            StbNode topA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_top);
            StbNode bottomB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_bottom);
            StbNode topB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_top);

            return Math.Abs(bottomA.X - bottomB.X) < Utility.Tolerance &&
                   Math.Abs(bottomA.Y - bottomB.Y) < Utility.Tolerance &&
                   Math.Abs(bottomA.Z - bottomB.Z) < Utility.Tolerance &&
                   Math.Abs(topA.X - topB.X) < Utility.Tolerance &&
                   Math.Abs(topA.Y - topB.Y) < Utility.Tolerance &&
                   Math.Abs(topA.Z - topB.Z) < Utility.Tolerance;
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

            StbNode bottomA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_bottom);
            StbNode topA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_top);
            StbNode bottomB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_bottom);
            StbNode topB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node_top);

            return Math.Abs(bottomA.X - bottomB.X) < toleranceSetting.ColumnTolerance.Node &&
                   Math.Abs(bottomA.Y - bottomB.Y) < toleranceSetting.ColumnTolerance.Node &&
                   Math.Abs(bottomA.Z - bottomB.Z) < toleranceSetting.ColumnTolerance.Node &&
                   Math.Abs(topA.X - topB.X) < toleranceSetting.ColumnTolerance.Node &&
                   Math.Abs(topA.Y - topB.Y) < toleranceSetting.ColumnTolerance.Node &&
                   Math.Abs(topA.Z - topB.Z) < toleranceSetting.ColumnTolerance.Node;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE? stb = istb as ST_BRIDGE;
            StbNode bottom = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_bottom);
            StbNode top = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_top);
            return [$"bottom=({bottom.X},{bottom.Y},{bottom.Z})", $"top=({top.X},{top.Y},{top.Z})"];
        }

        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name is "id_section" or "offset_bottom_X" or "offset_bottom_Y" or "offset_bottom_Z" or "offset_top_X" or "offset_top_Y" or "offset_top_Z" or "joint_id_top" or "joint_id_bottom";
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
                string columnA = FindColumnName(valueA.ToString(), kindA.ToString(), stbA);
                string columnB = FindColumnName(valueB.ToString(), kindB.ToString(), stbB);

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    columnA,
                    columnB,
                    columnA == columnB ? Consistency.Consistent : Consistency.Inconsistent,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));
            }
            else if (info.Name is "offset_bottom_X" or
                      "offset_bottom_Y" or
                      "offset_bottom_Z" or
                      "offset_top_X" or
                      "offset_top_Y" or
                      "offset_top_Z")
            {
                double offsetA = Convert.ToDouble(valueA);
                double offsetB = Convert.ToDouble(valueB);
                Consistency consistency = Math.Abs(offsetA - offsetB) < Utility.Tolerance
                    ? Consistency.Consistent
                    : Math.Abs(offsetA - offsetB) < toleranceSetting.ColumnTolerance.Offset ? Consistency.AlmostMatch : Consistency.Inconsistent;
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
            else if (info.Name is "joint_id_top" or "joint_id_bottom")
            {
                string jointA = string.Empty;
                if (stbA.StbModel.StbJoints.StbJointColumnShapeH.Any(n => n.id == valueA.ToString()))
                {
                    jointA = stbA.StbModel.StbJoints.StbJointColumnShapeH.First(n => n.id == valueA.ToString()).joint_mark;
                }
                else if (stbA.StbModel.StbJoints.StbJointColumnShapeT.Any(n => n.id == valueA.ToString()))
                {
                    jointA = stbA.StbModel.StbJoints.StbJointColumnShapeT.First(n => n.id == valueA.ToString()).joint_mark;
                }
                else if (stbA.StbModel.StbJoints.StbJointColumnShapeCross.Any(n => n.id == valueA.ToString()))
                {
                    jointA = stbA.StbModel.StbJoints.StbJointColumnShapeCross.First(n => n.id == valueA.ToString()).joint_mark;
                }

                string jointB = string.Empty;
                if (stbB.StbModel.StbJoints.StbJointColumnShapeH.Any(n => n.id == valueA.ToString()))
                {
                    jointB = stbB.StbModel.StbJoints.StbJointColumnShapeH.First(n => n.id == valueA.ToString()).joint_mark;
                }
                else if (stbB.StbModel.StbJoints.StbJointColumnShapeT.Any(n => n.id == valueA.ToString()))
                {
                    jointB = stbB.StbModel.StbJoints.StbJointColumnShapeT.First(n => n.id == valueA.ToString()).joint_mark;
                }
                else if (stbB.StbModel.StbJoints.StbJointColumnShapeCross.Any(n => n.id == valueA.ToString()))
                {
                    jointB = stbB.StbModel.StbJoints.StbJointColumnShapeCross.First(n => n.id == valueA.ToString()).joint_mark;
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
        private string? FindColumnName(string id, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                StbSecColumn_RC column = stbridge?.StbModel?.StbSections?.StbSecColumn_RC.FirstOrDefault(n => n.id == id);
                return column.floor + "/" + column.name;
            }
            else if (kind == "S")
            {
                StbSecColumn_S column = stbridge?.StbModel?.StbSections?.StbSecColumn_S.FirstOrDefault(n => n.id == id);
                return column.floor + "/" + column.name;
            }
            else if (kind == "SRC")
            {
                StbSecColumn_SRC column = stbridge?.StbModel?.StbSections?.StbSecColumn_SRC.FirstOrDefault(n => n.id == id);
                return column.floor + "/" + column.name;
            }
            else if (kind == "CFT")
            {
                StbSecColumn_CFT column = stbridge?.StbModel?.StbSections?.StbSecColumn_CFT.FirstOrDefault(n => n.id == id);
                return column.floor + "/" + column.name;
            }
            else
            {
                return null;
            }
        }
    }
}
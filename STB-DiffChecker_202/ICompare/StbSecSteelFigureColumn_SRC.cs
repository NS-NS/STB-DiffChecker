using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSecSteelFigureColumn_SRC : IProperty
    {
        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "joint_id_top" || info.Name == "joint_id_bottom";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);

            if (info.Name == "joint_id_top" || info.Name == "joint_id_bottom")
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
    }
}
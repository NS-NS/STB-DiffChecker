using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSecSteelFigureBeam_S : IProperty
    {
        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name is "joint_id_start" or "joint_id_end";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);

            if (info.Name is "joint_id_start" or "joint_id_end")
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
    }
}
using STBridge201;
using System;
using System.Collections.Generic;

namespace STBDiffChecker.AttributeType
{
    public class ReferenceJointAttribute : AbstractAttribute
    {
        public ReferenceJointAttribute(string stbName) : base(stbName)
        {
        }

        internal void Compare(string keyA, ST_BRIDGE stbridgeA, string keyB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            if (keyA == null && keyB == null)
                return;
            else if (keyA == null && keyB != null)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), "無", "有", Consistency.Incomparable, Importance));
                return;
            }
            else if (keyA != null && keyB == null)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), "有", "無", Consistency.Incomparable, Importance));
                return;
            }

            var jointA = FindJoint(keyA, stbridgeA);
            var jointB = FindJoint(keyB, stBridgeB);



            if (jointA is StbJointBeamShapeH beamShapeHA)
            {
                if (jointB is StbJointBeamShapeH shapeB)
                {
                    List<string> key2 = new List<string>(key);
                    key2.Add("joint_mark=" + beamShapeHA.joint_mark);
                    if (beamShapeHA.joint_mark == shapeB.joint_mark)
                    {
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), beamShapeHA.joint_mark, shapeB.joint_mark, Consistency.Consistent, this.Importance));
                    }
                    else
                    {
                        // 違う場合records.Add(new Record(this.ParentElement(), key2, this.Item(), beamShapeHA.joint_mark, shapeB.joint_mark, Consistency.Inconsistent, this.Importance));
                    }
                }
                else if (jointB is StbJointColumnShapeH)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointBeamShapeH), nameof(StbJointColumnShapeH), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeT)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointBeamShapeH), nameof(StbJointColumnShapeT), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeCross)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointBeamShapeH), nameof(StbJointColumnShapeCross), Consistency.Inconsistent, Importance));
                }
            }
            else if (jointA is StbJointColumnShapeH columnShapeHA)
            {
                if (jointB is StbJointBeamShapeH)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeH), nameof(StbJointBeamShapeH), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeH shapeB)
                {
                    List<string> key2 = new List<string>(key);
                    key2.Add("joint_mark=" + columnShapeHA.joint_mark);
                    if (columnShapeHA.joint_mark == shapeB.joint_mark)
                    {
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), columnShapeHA.joint_mark, shapeB.joint_mark, Consistency.Consistent, this.Importance));
                    }
                    else
                    {
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), columnShapeHA.joint_mark, shapeB.joint_mark, Consistency.Inconsistent, this.Importance));
                    }
                }
                else if (jointB is StbJointColumnShapeT)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeH), nameof(StbJointColumnShapeT), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeCross)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeH), nameof(StbJointColumnShapeCross), Consistency.Inconsistent, Importance));
                }
            }
            else if (jointA is StbJointColumnShapeT columnShapeTA)
            {
                if (jointB is StbJointBeamShapeH)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeT), nameof(StbJointBeamShapeH), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeH)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeT), nameof(StbJointColumnShapeH), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeT shapeB)
                {
                    List<string> key2 = new List<string>(key);
                    key2.Add("joint_mark=" + columnShapeTA.joint_mark);
                    if (columnShapeTA.joint_mark == shapeB.joint_mark)
                    {
                        // 同じ場合
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), columnShapeTA.joint_mark, shapeB.joint_mark, Consistency.Consistent, this.Importance));
                    }
                    else
                    {
                        // 違う場合
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), columnShapeTA.joint_mark, shapeB.joint_mark, Consistency.Inconsistent, this.Importance));
                    }
                }
                else if (jointB is StbJointColumnShapeCross)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeT), nameof(StbJointColumnShapeCross), Consistency.Inconsistent, Importance));
                }
            }
            else if (jointA is StbJointColumnShapeCross crossA)
            {
                if (jointB is StbJointBeamShapeH)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeCross), nameof(StbJointBeamShapeH), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeH)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeCross), nameof(StbJointColumnShapeH), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeT)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), nameof(StbJointColumnShapeCross), nameof(StbJointColumnShapeT), Consistency.Inconsistent, this.Importance));
                }
                else if (jointB is StbJointColumnShapeCross shapeB)
                {
                    List<string> key2 = new List<string>(key);
                    key2.Add("joint_mark=" + crossA.joint_mark);
                    if (crossA.joint_mark == shapeB.joint_mark)
                    {
                        // 同じ場合
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), crossA.joint_mark, shapeB.joint_mark, Consistency.Consistent, this.Importance));
                    }
                    else
                    {
                        // 違う場合
                        records.Add(new Record(this.ParentElement(), key2, this.Item(), crossA.joint_mark, shapeB.joint_mark, Consistency.Inconsistent, this.Importance));
                    }
                }
            }

            throw new Exception();
        }

        private static object FindJoint(string id, ST_BRIDGE stBridge)
        {
            if (id == null)
                return null;

            var joints = stBridge?.StbModel?.StbJoints;
            if (joints != null)
            {
                if (joints.StbJointBeamShapeH != null)
                {
                    foreach (var shapeH in joints.StbJointBeamShapeH)
                    {
                        if (shapeH.id == id)
                            return shapeH;
                    }
                }

                if (joints.StbJointColumnShapeH != null)
                {
                    foreach (var shapeH in joints.StbJointColumnShapeH)
                    {
                        if (shapeH.id == id)
                            return shapeH;
                    }
                }

                if (joints.StbJointColumnShapeT != null)
                {
                    foreach (var shapeT in joints.StbJointColumnShapeT)
                    {
                        if (shapeT.id == id)
                            return shapeT;
                    }
                }

                if (joints.StbJointColumnShapeCross != null)
                {
                    foreach (var cross in joints.StbJointColumnShapeCross)
                    {
                        if (cross.id == id)
                            return cross;
                    }
                }

            }

            throw new Exception();
        }

    }
}

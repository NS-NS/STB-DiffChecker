using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbFooting : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            if (obj is not StbFooting other)
            {
                return false;
            }

            StbNode nodeA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);
            StbNode nodeB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);


            return Math.Abs(nodeA.X - nodeB.X) <= Utility.Tolerance &&
                   Math.Abs(nodeA.Y - nodeB.Y) <= Utility.Tolerance &&
                   Math.Abs(nodeA.Z - nodeB.Z) <= Utility.Tolerance;

        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE stb = istb as ST_BRIDGE;
            StbNode node = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);
            return [$"node=({node.X},{node.Y},{node.Z})"];
        }

        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "id_section";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);

            if (info.Name == "id_section")
            {
                string foundationA = FindFootingName(valueA.ToString(), stbA);
                string foundationB = FindFootingName(valueB.ToString(), stbB);

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    foundationA,
                    foundationB,
                    foundationA == foundationB ? Consistency.Consistent : Consistency.Inconsistent,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));
            }
        }
        private string FindFootingName(string id, ST_BRIDGE stbridge)
        {
            StbSecFoundation_RC footing = stbridge?.StbModel?.StbSections?.StbSecFoundation_RC.FirstOrDefault(n => n.id == id);
            return footing.name;
        }
    }
}
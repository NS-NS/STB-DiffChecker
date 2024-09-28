using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbFoundationColumn : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            if (!(obj is StbFoundationColumn other))
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
            return new List<string> { $"node=({node.X},{node.Y},{node.Z})" };
        }

        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "id_section_FD" || info.Name == "id_section_WR";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting itoleranceSetting)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);

            if (info.Name == "id_section_FD" || info.Name == "id_section_WR")
            {
                string columnA = FindColumnName(valueA.ToString(), stbA);
                string columnB = FindColumnName(valueB.ToString(), stbB);

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
        }
        private string FindColumnName(string id, ST_BRIDGE stbridge)
        {
            StbSecColumn_RC footing = stbridge?.StbModel?.StbSections?.StbSecColumn_RC.FirstOrDefault(n => n.id == id);
            return footing.floor + "/" + footing.name;
        }
    }
}
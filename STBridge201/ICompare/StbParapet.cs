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
    public partial class StbParapet : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            if (obj is not StbParapet other)
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

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE stb = istb as ST_BRIDGE;
            StbNode bottom = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_start);
            StbNode top = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node_end);
            return [$"start=({bottom.X},{bottom.Y},{bottom.Z})", $"end=({top.X},{top.Y},{top.Z})"];
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
                // kind
                PropertyInfo kind = this.GetType().GetProperty("kind_structure");
                object kindA = kind.GetValue(this);
                object kindB = kind.GetValue(objB);
                string parapetA = FindParapetName(valueA.ToString(), kindA.ToString(), stbA);
                string parapetB = FindParapetName(valueB.ToString(), kindB.ToString(), stbB);

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    parapetA,
                    parapetB,
                    parapetA == parapetB ? Consistency.Consistent : Consistency.Inconsistent,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));
            }
        }
        private string FindParapetName(string id, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                StbSecParapet_RC parapet = stbridge?.StbModel?.StbSections?.StbSecParapet_RC.FirstOrDefault(n => n.id == id);
                return parapet.name;
            }
            else
            {
                return null;
            }
        }
    }
}
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
    public partial class StbPile : ICompare, IProperty
    {
        // Pileは同じ点に複数配置することがあるので、offsetまで比較する。
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            if (obj is not StbPile other)
            {
                return false;
            }

            StbNode nodeA = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id_node);
            StbNode nodeB = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == other.id_node);


            return Math.Abs(nodeA.X - nodeB.X) <= Utility.Tolerance &&
                   Math.Abs(nodeA.Y - nodeB.Y) <= Utility.Tolerance &&
                   Math.Abs(nodeA.Z - nodeB.Z) <= Utility.Tolerance &&
                   Math.Abs(this.offset_X - other.offset_X) <= Utility.Tolerance &&
                   Math.Abs(this.offset_Y - other.offset_Y) <= Utility.Tolerance &&
                   Math.Abs(this.level_top - other.level_top) <= Utility.Tolerance;

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
                // kind
                PropertyInfo kind = this.GetType().GetProperty("kind_structure");
                object kindA = kind.GetValue(this);
                object kindB = kind.GetValue(objB);
                string foundationA = FilePileName(valueA.ToString(), kindA.ToString(), stbA);
                string foundationB = FilePileName(valueB.ToString(), kindB.ToString(), stbB);

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
        private string FilePileName(string id_section, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                StbSecPile_RC pile = stbridge?.StbModel?.StbSections?.StbSecPile_RC?.FirstOrDefault(n => n.id == id_section);
                return pile.name;
            }
            else if (kind == "S")
            {
                StbSecPile_S pile = stbridge?.StbModel?.StbSections?.StbSecPile_S.FirstOrDefault(n => n.id == id_section);
                return pile.name;
            }
            else if (kind == "PC")
            {
                StbSecPileProduct pile = stbridge?.StbModel?.StbSections?.StbSecPileProduct.FirstOrDefault(n => n.id == id_section);
                return pile.name;
            }
            return null;
        }
    }
}
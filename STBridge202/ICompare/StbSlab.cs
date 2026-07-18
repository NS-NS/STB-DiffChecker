using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using STB_DiffChecker_202;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSlab : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            if (!(obj is StbSlab other))
            {
                return false;
            }
            List<StbNode> nodeA = new List<StbNode>();
            foreach (string id in this.StbNodeIdOrder.Split(' '))
            {
                StbNode node = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
                nodeA.Add(node);
            }

            List<StbNode> nodeB = new List<StbNode>();
            foreach (string id in other.StbNodeIdOrder.Split(' '))
            {
                StbNode node = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
                nodeB.Add(node);
            }

            if (nodeA.Count != nodeB.Count)
            {
                return false;
            }

            // startが同じと仮定
            for (int i = 0; i < nodeA.Count; i++)
            {
                if (Math.Abs(nodeA[i].X - nodeB[i].X) > Utility.Tolerance ||
                    Math.Abs(nodeA[i].Y - nodeB[i].Y) > Utility.Tolerance ||
                    Math.Abs(nodeA[i].Z - nodeB[i].Z) > Utility.Tolerance)
                {
                    return false;
                }
            }

            return true;

        }

        public bool AlmostCompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB, IToleranceSetting itoleranceSetting)
        {
            if (!(obj is StbSlab other))
            {
                return false;
            }
            ToleranceSetting toleranceSetting = itoleranceSetting as ToleranceSetting;
            ST_BRIDGE stbA = istbA as ST_BRIDGE;
            ST_BRIDGE stbB = istbB as ST_BRIDGE;

            List<StbNode> nodeA = new List<StbNode>();
            foreach (string id in this.StbNodeIdOrder.Split(' '))
            {
                StbNode node = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
                nodeA.Add(node);
            }

            List<StbNode> nodeB = new List<StbNode>();
            foreach (string id in other.StbNodeIdOrder.Split(' '))
            {
                StbNode node = stbB.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
                nodeB.Add(node);
            }

            if (nodeA.Count != nodeB.Count)
            {
                return false;
            }

            // startが同じと仮定
            for (int i = 0; i < nodeA.Count; i++)
            {
                if (Math.Abs(nodeA[i].X - nodeB[i].X) > toleranceSetting.SlabTolerance.Node ||
                    Math.Abs(nodeA[i].Y - nodeB[i].Y) > toleranceSetting.SlabTolerance.Node ||
                    Math.Abs(nodeA[i].Z - nodeB[i].Z) > toleranceSetting.SlabTolerance.Node)
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE stb = istb as ST_BRIDGE;
            List<string> key = new List<string>();
            string[] ids = this.StbNodeIdOrder.Split(' ');
            for (int i = 0; i < ids.Count(); i++)
            {
                StbNode node = stb.StbModel.StbNodes.FirstOrDefault(n => n.id == ids.ElementAt(i));
                key.Add($"節点{i + 1}=({node.X},{node.Y},{node.Z})");
            }
            return key;
        }

        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "id_section" || info.Name == "StbNodeIdOrder";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting itoleranceSetting)
        {
            ToleranceSetting toleranceSetting = itoleranceSetting as ToleranceSetting;
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
                string slabA = FindSlabName(valueA.ToString(), kindA.ToString(), stbA);
                string slabB = FindSlabName(valueB.ToString(), kindB.ToString(), stbB);

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    slabA,
                    slabB,
                    slabA == slabB ? Consistency.Consistent : Consistency.Inconsistent,
                    ObjectComparer.CheckImportance(parentElement, importanceDict)
                ));
            }
            else if (info.Name == "StbNodeIdOrder")
            {
                string[] splitA = valueA.ToString().Split(' ');
                string[] splitB = valueA.ToString().Split(' ');
                for (int i = 0; i < splitA.Count(); i++)
                {
                    records.Add(new Record(
                        parentElement,
                        key,
                        "@" + info.Name,
                        splitA.ElementAt(i),
                        splitB.ElementAt(i),
                        splitA.ElementAt(i) == splitB.ElementAt(i) ? Consistency.Consistent : Consistency.Inconsistent,
                        ObjectComparer.CheckImportance(parentElement, importanceDict)
                    ));

                }
            }
        }
        private string FindSlabName(string id, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                StbSecSlab_RC slab = stbridge?.StbModel?.StbSections?.StbSecSlab_RC.FirstOrDefault(n => n.id == id);
                return slab.name;
            }
            else if (kind == "DECK")
            {
                StbSecSlabDeck slab = stbridge?.StbModel?.StbSections?.StbSecSlabDeck.FirstOrDefault(n => n.id == id);
                return slab.name;
            }
            else if (kind == "PRECAST")
            {
                StbSecSlabPrecast slab = stbridge?.StbModel?.StbSections?.StbSecSlabPrecast.FirstOrDefault(n => n.id == id);
                return slab.name;
            }
            else
            {
                return null;
            }
        }
    }
}
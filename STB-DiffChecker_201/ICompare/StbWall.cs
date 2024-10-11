using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using STB_DiffChecker_201;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbWall : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            if (obj is not StbWall other)
            {
                return false;
            }
            List<StbNode> nodeA = [];
            foreach (string id in this.StbNodeIdOrder.Split(' '))
            {
                StbNode node = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
                nodeA.Add(node);
            }

            List<StbNode> nodeB = [];
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
            if (obj is not StbWall other)
            {
                return false;
            }
            ToleranceSetting? toleranceSetting = itoleranceSetting as ToleranceSetting;
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            List<StbNode> nodeA = [];
            foreach (string id in this.StbNodeIdOrder.Split(' '))
            {
                StbNode node = stbA.StbModel.StbNodes.FirstOrDefault(n => n.id == id);
                nodeA.Add(node);
            }

            List<StbNode> nodeB = [];
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
                if (Math.Abs(nodeA[i].X - nodeB[i].X) > toleranceSetting.WallTolerance.Node ||
                    Math.Abs(nodeA[i].Y - nodeB[i].Y) > toleranceSetting.WallTolerance.Node ||
                    Math.Abs(nodeA[i].Z - nodeB[i].Z) > toleranceSetting.WallTolerance.Node)
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE? stb = istb as ST_BRIDGE;
            List<string> key = [];
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
            return info.Name is "id_section" or "StbNodeIdOrder";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting itoleranceSetting)
        {
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
                string wallA = FindWallName(valueA.ToString(), kindA.ToString(), stbA);
                string wallB = FindWallName(valueB.ToString(), kindB.ToString(), stbB);

                records.Add(new Record(
                    parentElement,
                    key,
                    "@" + info.Name,
                    wallA,
                    wallB,
                    wallA == wallB ? Consistency.Consistent : Consistency.Inconsistent,
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
        private string? FindWallName(string id, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                StbSecWall_RC wall = stbridge?.StbModel?.StbSections?.StbSecWall_RC.FirstOrDefault(n => n.id == id);
                return wall.name;
            }
            else
            {
                return null;
            }
        }
    }
}
using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    // StbOpenはStbOpenでまとめるよりもWall, Slabでまとめるほうがよさそう
    // IPropertyの使い方が少し意図しているものと違いそうだが、名前を必須としないのでkeyでまとめづらいためここですべて比較する。
    public partial class StbOpenId : ICompare, IProperty
    {
        public bool CompareTo(object obj, IST_BRIDGE istbA, IST_BRIDGE istbB)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            if (obj is not StbOpenId other)
            {
                return false;
            }

            StbOpen openA = stbA.StbModel.StbMembers.StbOpens.FirstOrDefault(n => n.id == id);
            StbOpen openB = stbB.StbModel.StbMembers.StbOpens.FirstOrDefault(n => n.id == id);

            return Math.Abs(openA.position_X - openB.position_X) <= Utility.Tolerance &&
                Math.Abs(openA.position_Y - openB.position_Y) <= Utility.Tolerance &&
                Math.Abs(openA.length_X - openB.length_X) <= Utility.Tolerance &&
                Math.Abs(openA.length_Y - openB.length_Y) <= Utility.Tolerance &&
                Math.Abs(openA.rotate - openB.rotate) <= Utility.Tolerance;
        }

        public IEnumerable<string> GetKey(IST_BRIDGE istb)
        {
            ST_BRIDGE? stb = istb as ST_BRIDGE;
            StbOpen open = stb.StbModel.StbMembers.StbOpens.FirstOrDefault(n => n.id == id);
            return [$"position_X={open.position_X},position_Y={open.position_Y},length_X={open.length_X},length_Y={open.length_Y},rotate={open.rotate}"];
        }

        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "id";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            if (info.Name == "id")
            {
                StbOpen openA = stbA.StbModel.StbMembers.StbOpens.FirstOrDefault(n => n.id == info.GetValue(this).ToString());
                StbOpen openB = stbB.StbModel.StbMembers.StbOpens.FirstOrDefault(n => n.id == info.GetValue(objB).ToString());

                // @id=>/StbOpen
                parentElement += "=>/StbOpen";

                ObjectComparer.CompareProperties(openA, stbA, openB, stbB, parentElement, key, records, importanceDict, toleranceSetting);
            }
        }
    }
}

using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbOpen : IProperty
    {
        public bool IsSpecial(PropertyInfo info)
        {
            return info.Name == "id_section";
        }

        public void CompareProperty(PropertyInfo info, IST_BRIDGE istbA, object objB, IST_BRIDGE istbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            ST_BRIDGE? stbA = istbA as ST_BRIDGE;
            ST_BRIDGE? stbB = istbB as ST_BRIDGE;

            object valueA = info.GetValue(this);
            object valueB = info.GetValue(objB);


            if (info.Name == "id_section")
            {
                StbSecOpen_RC openA = stbA.StbModel.StbSections.StbSecOpen_RC.FirstOrDefault(n => n.id == info.GetValue(this).ToString());
                StbSecOpen_RC openB = stbB.StbModel.StbSections.StbSecOpen_RC.FirstOrDefault(n => n.id == info.GetValue(objB).ToString());

                // @id_section=>/StbSecOpen_RC
                parentElement += "=>/StbSecOpen_RC";
                ObjectComparer.CompareProperties(openA, stbA, openB, stbB, parentElement, key, records, importanceDict, toleranceSetting);
            }
        }
    }
}

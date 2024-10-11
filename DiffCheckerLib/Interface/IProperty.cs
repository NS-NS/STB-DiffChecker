using DiffCheckerLib.Enum;
using System.Collections.Generic;
using System.Reflection;

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// 参照先の内容まで比較するクラス用のインターフェース
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// 比較が必要な属性かどうか。StbSecSteelのshapeとか
        /// </summary>
        bool IsSpecial(PropertyInfo info);

        /// <summary>
        /// 参照先の比較結果を返す関数
        /// </summary>
        void CompareProperty(PropertyInfo info, IST_BRIDGE stbA, object valueB, IST_BRIDGE stbB, string parentElement, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting);
    }
}

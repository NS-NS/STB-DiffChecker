using System.Collections.Generic;

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// ST_BRIDGEのpartialクラスに付加するインターフェース。
    /// id_nodeなどid属性の参照している位置や符号名を比較するものに付加する。
    /// </summary>
    public interface ICompare
    {
        bool CompareTo(object other, IST_BRIDGE stbA, IST_BRIDGE stbB);
        bool AlmostCompareTo(object other, IST_BRIDGE stbA, IST_BRIDGE stB, IToleranceSetting toleranceSetting) { return false; }
        IEnumerable<string> GetKey(IST_BRIDGE stb);
    }
}

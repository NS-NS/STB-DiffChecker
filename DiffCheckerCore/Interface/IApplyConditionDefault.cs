using System.Reflection;

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// StbApplyConditionsListのset_default指定を、意味的に対応する断面配筋側の属性へ当て込むためのインターフェース。
    /// </summary>
    public interface IApplyConditionDefault
    {
        /// <summary>
        /// set_default=trueかつApplyCondition側の対応属性が入力済みの場合のみtrueを返し、
        /// defaultValueに実効値(既存のPropertyTypeと同じ型: double or string)を格納する。
        /// 対象外・条件不成立の場合は必ずfalseを返す(=呼び出し元の現行動作を一切変えない)。
        /// </summary>
        bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE stb, out object defaultValue);
    }
}

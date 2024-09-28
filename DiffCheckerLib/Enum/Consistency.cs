namespace DiffCheckerLib.Enum
{
    /// <summary>
    /// 比較結果を表すEnum
    /// </summary>
    public enum Consistency
    {
        /// <summary>
        /// 完全一致
        /// </summary>
        Consistent,

        /// <summary>
        /// 許容差内の一致
        /// </summary>
        AlmostMatch,

        /// <summary>
        /// 不一致
        /// </summary>
        Inconsistent,

        /// <summary>
        /// 比較対象がない(属性)
        /// </summary>
        Incomparable,

        /// <summary>
        /// 比較対象がない(要素)
        /// </summary>
        ElementIncomparable
    }
}
namespace DiffCheckerLib.Enum
{
    /// <summary>
    /// 重要度を表すEnum
    /// </summary>
    public enum Importance
    {
        /// <summary>
        /// 高
        /// </summary>
        Required,

        /// <summary>
        /// 中
        /// </summary>
        Optional,

        /// <summary>
        /// 低
        /// </summary>
        Unnecessary,

        /// <summary>
        /// 対象外。idなどの属性に対して重要度を設定する場合に使用
        /// </summary>
        NotApplicable
    }
}
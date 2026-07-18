namespace DiffCheckerLib.Setting
{
    /// <summary>
    /// ユーザーが設定する許容差
    /// </summary>
    public class UserTolerance
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 節点の許容差
        /// </summary>
        public double Node;

        /// <summary>
        /// オフセットの許容差
        /// </summary>
        public double Offset;

        public UserTolerance(string name)
        {
            Name = name;
        }
    }
}
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;

namespace STB_DiffChecker_202
{
    /// <summary>
    /// 許容差用クラス
    /// </summary>
    public class ToleranceSetting : IToleranceSetting
    {
        public UserTolerance ColumnTolerance = new("柱(StbColumn)");
        public UserTolerance PostTolerance = new("間柱(StbPost)");
        public UserTolerance GirderTolerance = new("大梁(StbGirder)");
        public UserTolerance BeamTolerance = new("小梁(StbBeam)");
        public UserTolerance BraceTolerance = new("ブレース(StbBrace)");
        public UserTolerance SlabTolerance = new("スラブ(Stbslab)");
        public UserTolerance WallTolerance = new("壁(StbWall)");
        public UserTolerance OpenTolerance = new("開口(StbOpen)");

        public List<UserTolerance> Tolerances()
        {
            return
            [
                ColumnTolerance,
                PostTolerance,
                GirderTolerance,
                BeamTolerance,
                BraceTolerance,
                SlabTolerance,
                WallTolerance,
                OpenTolerance
            ];
        }
    }
}
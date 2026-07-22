using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    /// <summary>
    /// StbSecBarColumn_SRC_RectSame/RectNotSame/CircleSame/CircleNotSameは当て込み対象の属性名が全て共通のため、
    /// ロジックをここに集約し、各partial classからは委譲するのみにする。
    /// </summary>
    internal static class StbSecBarColumn_SRCApplyConditionDefault
    {
        public static bool TryGet(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbColumn_SRC_BarSpacingApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbColumn_SRC_BarSpacingApply;
            if (apply == null || !apply.set_default)
            {
                return false;
            }

            if (property.Name == nameof(StbSecBarColumn_SRC_RectSame.pitch_bar_spacing))
            {
                if (!apply.pitch_bar_spacingSpecified)
                {
                    return false;
                }
                defaultValue = apply.pitch_bar_spacing;
                return true;
            }

            string value = property.Name switch
            {
                nameof(StbSecBarColumn_SRC_RectSame.D_bar_spacing) => apply.D_bar_spacing,
                nameof(StbSecBarColumn_SRC_RectSame.N_bar_spacing_X) => apply.N_bar_spacing_X,
                nameof(StbSecBarColumn_SRC_RectSame.N_bar_spacing_Y) => apply.N_bar_spacing_Y,
                _ => null,
            };
            if (value == null)
            {
                return false;
            }
            defaultValue = value;
            return true;
        }
    }

    public partial class StbSecBarColumn_SRC_RectSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_SRCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarColumn_SRC_RectNotSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_SRCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarColumn_SRC_CircleSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_SRCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarColumn_SRC_CircleNotSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_SRCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }
}

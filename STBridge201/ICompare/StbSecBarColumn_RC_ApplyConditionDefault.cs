using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    /// <summary>
    /// StbSecBarColumn_RC_RectSame/RectNotSame/CircleSame/CircleNotSameは当て込み対象の属性名が全て共通のため、
    /// ロジックをここに集約し、各partial classからは委譲するのみにする。
    /// </summary>
    internal static class StbSecBarColumn_RCApplyConditionDefault
    {
        public static bool TryGet(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbColumn_RC_BarSpacingApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbColumn_RC_BarSpacingApply;
            if (apply == null || !apply.set_default)
            {
                return false;
            }

            if (property.Name == nameof(StbSecBarColumn_RC_RectSame.pitch_bar_spacing))
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
                nameof(StbSecBarColumn_RC_RectSame.D_bar_spacing) => apply.D_bar_spacing,
                nameof(StbSecBarColumn_RC_RectSame.N_bar_spacing_X) => apply.N_bar_spacing_X,
                nameof(StbSecBarColumn_RC_RectSame.N_bar_spacing_Y) => apply.N_bar_spacing_Y,
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

    public partial class StbSecBarColumn_RC_RectSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarColumn_RC_RectNotSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarColumn_RC_CircleSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarColumn_RC_CircleNotSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarColumn_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }
}

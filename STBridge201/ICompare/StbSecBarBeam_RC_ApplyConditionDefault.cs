using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    /// <summary>
    /// StbSecBarBeam_RC_Same/StartEnd/ThreeTypesは当て込み対象の属性名が全て共通のため、
    /// ロジックをここに集約し、各partial classからは委譲するのみにする。
    /// </summary>
    internal static class StbSecBarBeam_RCApplyConditionDefault
    {
        public static bool TryGet(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbApplyConditionsList applyList = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList;

            if (property.Name is nameof(StbSecBarBeam_RC_Same.D_web) or nameof(StbSecBarBeam_RC_Same.N_web))
            {
                StbBeam_RC_BarWebApply apply = applyList?.StbBeam_RC_BarWebApply;
                if (apply == null || !apply.set_default)
                {
                    return false;
                }
                string webValue = property.Name == nameof(StbSecBarBeam_RC_Same.D_web) ? apply.D_web : apply.N_web;
                if (webValue == null)
                {
                    return false;
                }
                defaultValue = webValue;
                return true;
            }

            if (property.Name is nameof(StbSecBarBeam_RC_Same.D_bar_spacing) or nameof(StbSecBarBeam_RC_Same.pitch_bar_spacing) or nameof(StbSecBarBeam_RC_Same.N_bar_spacing))
            {
                StbBeam_RC_BarSpacingApply apply = applyList?.StbBeam_RC_BarSpacingApply;
                if (apply == null || !apply.set_default)
                {
                    return false;
                }
                if (property.Name == nameof(StbSecBarBeam_RC_Same.pitch_bar_spacing))
                {
                    if (!apply.pitch_bar_spacingSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.pitch_bar_spacing;
                    return true;
                }
                string spacingValue = property.Name == nameof(StbSecBarBeam_RC_Same.D_bar_spacing) ? apply.D_bar_spacing : apply.N_bar_spacing;
                if (spacingValue == null)
                {
                    return false;
                }
                defaultValue = spacingValue;
                return true;
            }

            return false;
        }
    }

    public partial class StbSecBarBeam_RC_Same : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarBeam_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarBeam_RC_StartEnd : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarBeam_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }

    public partial class StbSecBarBeam_RC_ThreeTypes : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
            => StbSecBarBeam_RCApplyConditionDefault.TryGet(property, istb, out defaultValue);
    }
}

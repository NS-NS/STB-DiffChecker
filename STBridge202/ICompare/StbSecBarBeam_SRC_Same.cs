using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSecBarBeam_SRC_Same : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbApplyConditionsList applyList = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList;

            if (property.Name is nameof(D_web) or nameof(N_web))
            {
                StbBeam_SRC_BarWebApply apply = applyList?.StbBeam_SRC_BarWebApply;
                if (apply == null || !apply.set_default)
                {
                    return false;
                }
                string webValue = property.Name == nameof(D_web) ? apply.D_web : apply.N_web;
                if (webValue == null)
                {
                    return false;
                }
                defaultValue = webValue;
                return true;
            }

            if (property.Name is nameof(D_bar_spacing) or nameof(pitch_bar_spacing) or nameof(N_bar_spacing))
            {
                StbBeam_SRC_BarSpacingApply apply = applyList?.StbBeam_SRC_BarSpacingApply;
                if (apply == null || !apply.set_default)
                {
                    return false;
                }
                if (property.Name == nameof(pitch_bar_spacing))
                {
                    if (!apply.pitch_bar_spacingSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.pitch_bar_spacing;
                    return true;
                }
                string spacingValue = property.Name == nameof(D_bar_spacing) ? apply.D_bar_spacing : apply.N_bar_spacing;
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
}

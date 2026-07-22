using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbSecBarColumn_RC_CircleSame : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbColumn_RC_BarSpacingApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbColumn_RC_BarSpacingApply;
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

            string value = property.Name switch
            {
                nameof(D_bar_spacing) => apply.D_bar_spacing,
                nameof(N_bar_spacing_X) => apply.N_bar_spacing_X,
                nameof(N_bar_spacing_Y) => apply.N_bar_spacing_Y,
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
}

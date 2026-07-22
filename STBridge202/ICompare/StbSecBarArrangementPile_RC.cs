using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSecBarArrangementPile_RC : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbPile_RC_BarPositionApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbPile_RC_BarPositionApply;
            if (apply == null || !apply.set_default || !apply.depth_coverSpecified)
            {
                return false;
            }

            if (property.Name == nameof(depth_cover))
            {
                defaultValue = apply.depth_cover;
                return true;
            }

            return false;
        }
    }
}

using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSecBarArrangementParapet_RC : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbParapet_RC_BarPositionApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbParapet_RC_BarPositionApply;
            if (apply == null || !apply.set_default || !apply.depth_coverSpecified)
            {
                return false;
            }

            if (property.Name is nameof(depth_cover_outside) or nameof(depth_cover_inside))
            {
                defaultValue = apply.depth_cover;
                return true;
            }

            return false;
        }
    }
}

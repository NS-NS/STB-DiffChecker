using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbSecBarArrangementFoundation_RC : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbFoundation_RC_BarPositionApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbFoundation_RC_BarPositionApply;
            if (apply == null || !apply.set_default || !apply.depth_coverSpecified)
            {
                return false;
            }

            if (property.Name is nameof(depth_cover_top) or nameof(depth_cover_bottom) or nameof(depth_cover_side))
            {
                defaultValue = apply.depth_cover;
                return true;
            }

            return false;
        }
    }
}

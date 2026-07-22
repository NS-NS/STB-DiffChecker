using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbSecBarArrangementSlab_RC : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbSlab_RC_BarPositionApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbSlab_RC_BarPositionApply;
            if (apply == null || !apply.set_default || !apply.depth_coverSpecified)
            {
                return false;
            }

            if (property.Name is nameof(depth_cover_top) or nameof(depth_cover_bottom))
            {
                defaultValue = apply.depth_cover;
                return true;
            }

            return false;
        }
    }
}

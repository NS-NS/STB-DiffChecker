using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE202
{
    public partial class StbSecBarArrangementBeam_SRC : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbBeam_SRC_RebarPositionApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbBeam_SRC_RebarPositionApply;
            if (apply == null || !apply.set_default)
            {
                return false;
            }

            switch (property.Name)
            {
                case nameof(depth_cover_left):
                case nameof(depth_cover_right):
                    if (!apply.depth_cover_sideSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.depth_cover_side;
                    return true;
                case nameof(depth_cover_top):
                case nameof(depth_cover_bottom):
                    if (!apply.depth_cover_top_bottomSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.depth_cover_top_bottom;
                    return true;
                case nameof(interval):
                    if (!apply.intervalSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.interval;
                    return true;
                case nameof(center_side):
                    if (!apply.center_sideSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.center_side;
                    return true;
                case nameof(center_top):
                case nameof(center_bottom):
                    if (!apply.center_top_bottomSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.center_top_bottom;
                    return true;
                default:
                    return false;
            }
        }
    }
}

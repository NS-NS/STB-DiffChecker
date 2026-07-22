using DiffCheckerLib.Interface;
using System.Reflection;

namespace ST_BRIDGE201
{
    public partial class StbSecBarArrangementColumn_SRC : IApplyConditionDefault
    {
        public bool TryGetApplyConditionDefault(PropertyInfo property, IST_BRIDGE istb, out object defaultValue)
        {
            defaultValue = null;
            StbColumn_SRC_RebarPositionApply apply = (istb as ST_BRIDGE)?.StbCommon?.StbApplyConditionsList?.StbColumn_SRC_RebarPositionApply;
            if (apply == null || !apply.set_default)
            {
                return false;
            }

            switch (property.Name)
            {
                case nameof(depth_cover_start_X):
                case nameof(depth_cover_end_X):
                case nameof(depth_cover_start_Y):
                case nameof(depth_cover_end_Y):
                    if (!apply.depth_coverSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.depth_cover;
                    return true;
                case nameof(interval):
                    if (!apply.intervalSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.interval;
                    return true;
                case nameof(center_start_X):
                case nameof(center_end_X):
                case nameof(center_start_Y):
                case nameof(center_end_Y):
                case nameof(center_interval):
                    if (!apply.centerSpecified)
                    {
                        return false;
                    }
                    defaultValue = apply.center;
                    return true;
                default:
                    return false;
            }
        }
    }
}

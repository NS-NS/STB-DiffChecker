using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbReinforcementStrength = STBridge201.StbReinforcementStrength;
using StbReinforcementStrengthList = STBridge201.StbReinforcementStrengthList;

namespace STBDiffChecker.v201.Records
{
    internal static class Common
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            StbridgeVersion.Compare(stBridgeA.version, stBridgeB.version, null, records);
            StbCommonGuid.Compare(stBridgeA.StbCommon.guid, stBridgeB.StbCommon.guid, null, records);
            StbCommonProjectName.Compare(stBridgeA.StbCommon.project_name, stBridgeB.StbCommon.project_name, null, records);
            StbCommonAppName.Compare(stBridgeA.StbCommon.app_name, stBridgeB.StbCommon.app_name, null, records);
            StbCommonStrengthConcrete.Compare(stBridgeA.StbCommon.strength_concrete, stBridgeB.StbCommon.strength_concrete, null, records);

            if (stBridgeA.StbCommon.StbReinforcementStrengthList != null ||
                stBridgeB.StbCommon.StbReinforcementStrengthList != null)
            {
                if (stBridgeB.StbCommon.StbReinforcementStrengthList == null)
                {
                    CheckObjects.StbReinforcementStrengthList.Compare(nameof(StbReinforcementStrengthList), null, null, records);
                }
                else if (stBridgeA.StbCommon.StbReinforcementStrengthList == null)
                {
                    CheckObjects.StbReinforcementStrengthList.Compare(null, nameof(StbReinforcementStrengthList), null, records);
                }
                else
                {
                    var setB = new HashSet<StbReinforcementStrength>(stBridgeB.StbCommon.StbReinforcementStrengthList);
                    foreach (var a in stBridgeA.StbCommon.StbReinforcementStrengthList)
                    {
                        var key = new List<string> { $"D={a.D}"};
                        var b = stBridgeB.StbCommon.StbReinforcementStrengthList.FirstOrDefault(n => n.D == a.D);
                        if (b != null)
                        {
                            StbReinforcementStrengthD.Compare(a.D, b.D, key, records);
                            StbReinforcementStrengthStrength.Compare(a.strength, b.strength, key, records);
                            setB.Remove(b);
                        }
                        else
                        {
                            CheckObjects.StbReinforcementStrength.Compare(nameof(StbReinforcementStrength), null, key, records);
                        }
                    }

                    foreach (var b in setB)
                    {
                        var key = new List<string> { $"D={b.D}"};
                        CheckObjects.StbReinforcementStrength.Compare(null, nameof(StbReinforcementStrength), key, records);
                    }
                }
            }

            if (stBridgeA.StbCommon.StbApplyConditionsList != null ||
                stBridgeB.StbCommon.StbApplyConditionsList != null)
            {
                if (stBridgeB.StbCommon.StbApplyConditionsList == null)
                {
                    CheckObjects.StbApplyConditionList.Compare(nameof(StbApplyConditionList), null, null, records);
                }
                else if (stBridgeA.StbCommon.StbApplyConditionsList == null)
                {
                    CheckObjects.StbApplyConditionList.Compare(null, nameof(StbApplyConditionList), null, records);
                }
            }

            if (records.Count == 0)
                return null;
            return records;
        }
    }
}

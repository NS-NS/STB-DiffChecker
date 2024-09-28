using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using ST_BRIDGE202;

namespace STB_DiffChecker_202
{
    public class TotalRecord : ITotalRecords
    {
        public List<RecordTab> recordTabs { get; set; } = [];
        public ResultFormSetting resultFormSetting { get; set; }
        public Summary Summary { get; set; } = new Summary();
        public IST_BRIDGE istbridgeA { get; set; }
        public IST_BRIDGE istbridgeB { get; set; }

        public TotalRecord(ResultFormSetting resultFormSetting, ST_BRIDGE stbA, ST_BRIDGE stbB)
        {
            Summary.dateTime = DateTime.Now;
            this.resultFormSetting = resultFormSetting;
            istbridgeA = stbA;
            istbridgeB = stbB;

            recordTabs = [];
            foreach ((string, string) tab in resultFormSetting.importanceSetting.GetTabs())
            {
                recordTabs.Add(new RecordTab(tab.Item2));
            }
        }

        public Summary CreateSummary()
        {
            IReadOnlyList<(string, string)> tabs = resultFormSetting.importanceSetting.GetTabs();
            ST_BRIDGE? stbridgeA = istbridgeA as ST_BRIDGE;
            ST_BRIDGE? stbridgeB = istbridgeB as ST_BRIDGE;
            for (int i = 0; i < tabs.Count(); i++)
            {
                if (recordTabs.ElementAt(i).records != null)
                {
                    int numberA, numberB;
                    switch (i)
                    {
                        case 0: numberA = stbridgeA?.StbCommon != null ? 1 : 0; numberB = stbridgeB?.StbCommon != null ? 1 : 0; break;
                        case 1: numberA = stbridgeA?.StbModel?.StbNodes?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbNodes?.Length ?? 0; break;
                        case 2: numberA = stbridgeA?.StbModel?.StbAxes?.StbParallelAxes?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbAxes?.StbParallelAxes?.Length ?? 0; break;
                        case 3: numberA = stbridgeA?.StbModel?.StbAxes?.StbArcAxes?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbAxes?.StbArcAxes?.Length ?? 0; break;
                        case 4: numberA = stbridgeA?.StbModel?.StbAxes?.StbRadialAxes?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbAxes?.StbRadialAxes?.Length ?? 0; break;
                        case 5: numberA = stbridgeA?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis?.Length ?? 0; break;
                        case 6: numberA = stbridgeA?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis?.Length ?? 0; break;
                        case 7: numberA = stbridgeA?.StbModel?.StbStories?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbStories?.Length ?? 0; break;
                        case 8: numberA = stbridgeA?.StbModel?.StbMembers?.StbColumns?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbColumns?.Length ?? 0; break;
                        case 9: numberA = stbridgeA?.StbModel?.StbMembers?.StbPosts?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbPosts?.Length ?? 0; break;
                        case 10: numberA = stbridgeA?.StbModel?.StbMembers?.StbGirders?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbGirders?.Length ?? 0; break;
                        case 11: numberA = stbridgeA?.StbModel?.StbMembers?.StbBeams?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbBeams?.Length ?? 0; break;
                        case 12: numberA = stbridgeA?.StbModel?.StbMembers?.StbBraces?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbBraces?.Length ?? 0; break;
                        case 13: numberA = stbridgeA?.StbModel?.StbMembers?.StbSlabs?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbSlabs?.Length ?? 0; break;
                        case 14: numberA = stbridgeA?.StbModel?.StbMembers?.StbWalls?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbWalls?.Length ?? 0; break;
                        case 15: numberA = stbridgeA?.StbModel?.StbMembers?.StbFootings?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbFootings?.Length ?? 0; break;
                        case 16: numberA = stbridgeA?.StbModel?.StbMembers?.StbStripFootings?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbStripFootings?.Length ?? 0; break;
                        case 17: numberA = stbridgeA?.StbModel?.StbMembers?.StbPiles?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbPiles?.Length ?? 0; break;
                        case 18: numberA = stbridgeA?.StbModel?.StbMembers?.StbFoundationColumns?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbFoundationColumns?.Length ?? 0; break;
                        case 19: numberA = stbridgeA?.StbModel?.StbMembers?.StbParapets?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbMembers?.StbParapets?.Length ?? 0; break;
                        case 20: numberA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_RC?.Length ?? 0; break;
                        case 21: numberA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_S?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_S?.Length ?? 0; break;
                        case 22: numberA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_SRC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_SRC?.Length ?? 0; break;
                        case 23: numberA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_CFT?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_CFT?.Length ?? 0; break;
                        case 24: numberA = stbridgeA?.StbModel?.StbSections?.StbSecBeam_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecBeam_RC?.Length ?? 0; break;
                        case 25: numberA = stbridgeA?.StbModel?.StbSections?.StbSecBeam_S?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecBeam_S?.Length ?? 0; break;
                        case 26: numberA = stbridgeA?.StbModel?.StbSections?.StbSecBeam_SRC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecBeam_SRC?.Length ?? 0; break;
                        case 27: numberA = stbridgeA?.StbModel?.StbSections?.StbSecBrace_S?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecBrace_S?.Length ?? 0; break;
                        case 28: numberA = stbridgeA?.StbModel?.StbSections?.StbSecSlab_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecSlab_RC?.Length ?? 0; break;
                        case 29: numberA = stbridgeA?.StbModel?.StbSections?.StbSecSlabDeck?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecSlabDeck?.Length ?? 0; break;
                        case 30: numberA = stbridgeA?.StbModel?.StbSections?.StbSecSlabPrecast?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecSlabPrecast?.Length ?? 0; break;
                        case 31: numberA = stbridgeA?.StbModel?.StbSections?.StbSecWall_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecWall_RC?.Length ?? 0; break;
                        case 32: numberA = stbridgeA?.StbModel?.StbSections?.StbSecFoundation_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecFoundation_RC?.Length ?? 0; break;
                        case 33: numberA = stbridgeA?.StbModel?.StbSections?.StbSecPile_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecPile_RC?.Length ?? 0; break;
                        case 34: numberA = stbridgeA?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0; break;
                        case 35: numberA = stbridgeA?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0; break;
                        case 36: numberA = stbridgeA?.StbModel?.StbSections?.StbSecParapet_RC?.Length ?? 0; numberB = stbridgeB?.StbModel?.StbSections?.StbSecParapet_RC?.Length ?? 0; break;
                        case 37:
                            numberA = stbridgeA?.StbModel?.StbJoints?.StbJointColumnShapeH?.Length ?? 0
                                    + stbridgeA?.StbModel?.StbJoints?.StbJointColumnShapeT?.Length ?? 0
                                    + stbridgeA?.StbModel?.StbJoints?.StbJointColumnShapeCross?.Length ?? 0
                                    + stbridgeA?.StbModel?.StbJoints?.StbJointBeamShapeH?.Length ?? 0;
                            numberB = stbridgeB?.StbModel?.StbJoints?.StbJointColumnShapeH?.Length ?? 0
                                    + stbridgeB?.StbModel?.StbJoints?.StbJointColumnShapeT?.Length ?? 0
                                    + stbridgeB?.StbModel?.StbJoints?.StbJointColumnShapeCross?.Length ?? 0
                                    + stbridgeB?.StbModel?.StbJoints?.StbJointBeamShapeH?.Length ?? 0;
                            break;
                        default: throw new NotImplementedException();
                    }
                    Summary.Rows.Add(
                        Summary.CollectResult(tabs.ElementAt(i).Item2, numberA, numberB, recordTabs.ElementAt(i).records));
                }
            }

            Summary.SetRow();
            return Summary;
        }
    }
}

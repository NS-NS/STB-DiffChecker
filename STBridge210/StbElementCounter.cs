using ST_BRIDGE210;

namespace STB_DiffChecker_210
{
    /// <summary>
    /// タブ名に対応するST_BRIDGE要素数を取得するクラス
    /// </summary>
    public static class StbElementCounter
    {
        public static int CountElements(ST_BRIDGE? stb, string tabName)
        {
            return tabName switch
            {
                "StbCommon" => stb?.StbCommon != null ? 1 : 0,
                "StbNodes" => stb?.StbModel?.StbNodes?.Length ?? 0,
                "StbParallelAxes" => stb?.StbModel?.StbAxes?.StbParallelAxes?.Length ?? 0,
                "StbArcAxes" => stb?.StbModel?.StbAxes?.StbArcAxes?.Length ?? 0,
                "StbRadialAxes" => stb?.StbModel?.StbAxes?.StbRadialAxes?.Length ?? 0,
                "StbDrawingLineAxis" => stb?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis?.Length ?? 0,
                "StbDrawingArcAxis" => stb?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis?.Length ?? 0,
                "StbStories" => stb?.StbModel?.StbStories?.Length ?? 0,
                "StbColumns" => stb?.StbModel?.StbMembers?.StbColumns?.Length ?? 0,
                "StbPosts" => stb?.StbModel?.StbMembers?.StbPosts?.Length ?? 0,
                "StbGirders" => stb?.StbModel?.StbMembers?.StbGirders?.Length ?? 0,
                "StbBeams" => stb?.StbModel?.StbMembers?.StbBeams?.Length ?? 0,
                "StbBraces" => stb?.StbModel?.StbMembers?.StbBraces?.Length ?? 0,
                "StbSlabs" => stb?.StbModel?.StbMembers?.StbSlabs?.Length ?? 0,
                "StbWalls" => stb?.StbModel?.StbMembers?.StbWalls?.Length ?? 0,
                "StbIsolatingDevices" => stb?.StbModel?.StbMembers?.StbIsolatingDevices?.Length ?? 0,
                "StbDampingDevices" => stb?.StbModel?.StbMembers?.StbDampingDevices?.Length ?? 0,
                "StbFrameDampingDevices" => stb?.StbModel?.StbMembers?.StbFrameDampingDevices?.Length ?? 0,
                "StbFootings" => stb?.StbModel?.StbMembers?.StbFootings?.Length ?? 0,
                "StbStripFootings" => stb?.StbModel?.StbMembers?.StbStripFootings?.Length ?? 0,
                "StbPiles" => stb?.StbModel?.StbMembers?.StbPiles?.Length ?? 0,
                "StbFoundationColumns" => stb?.StbModel?.StbMembers?.StbFoundationColumns?.Length ?? 0,
                "StbParapets" => stb?.StbModel?.StbMembers?.StbParapets?.Length ?? 0,
                "StbOpenArrangements" => stb?.StbModel?.StbMembers?.StbOpenArrangements?.Length ?? 0,
                "StbPenetrationArrangements" => stb?.StbModel?.StbMembers?.StbPenetrationArrangements?.Length ?? 0,
                "StbJointArrangements" => stb?.StbModel?.StbMembers?.StbJointArrangements?.Length ?? 0,
                "StbPanelZoneArrangements" => stb?.StbModel?.StbMembers?.StbPanelZoneArrangements?.Length ?? 0,
                "StbConnectionArrangements" => stb?.StbModel?.StbMembers?.StbConnectionArrangements?.Length ?? 0,
                "StbSecColumn_RC" => stb?.StbModel?.StbSections?.StbSecColumn_RC?.Length ?? 0,
                "StbSecColumn_S" => stb?.StbModel?.StbSections?.StbSecColumn_S?.Length ?? 0,
                "StbSecColumn_SRC" => stb?.StbModel?.StbSections?.StbSecColumn_SRC?.Length ?? 0,
                "StbSecColumn_CFT" => stb?.StbModel?.StbSections?.StbSecColumn_CFT?.Length ?? 0,
                "StbSecBeam_RC" => stb?.StbModel?.StbSections?.StbSecBeam_RC?.Length ?? 0,
                "StbSecBeam_S" => stb?.StbModel?.StbSections?.StbSecBeam_S?.Length ?? 0,
                "StbSecBeam_SRC" => stb?.StbModel?.StbSections?.StbSecBeam_SRC?.Length ?? 0,
                "StbSecBrace_S" => stb?.StbModel?.StbSections?.StbSecBrace_S?.Length ?? 0,
                "StbSecSlab_RC" => stb?.StbModel?.StbSections?.StbSecSlab_RC?.Length ?? 0,
                "StbSecSlabDeck" => stb?.StbModel?.StbSections?.StbSecSlabDeck?.Length ?? 0,
                "StbSecSlabPrecast" => stb?.StbModel?.StbSections?.StbSecSlabPrecast?.Length ?? 0,
                "StbSecSlabLoad" => stb?.StbModel?.StbSections?.StbSecSlabLoad?.Length ?? 0,
                "StbSecWall_RC" => stb?.StbModel?.StbSections?.StbSecWall_RC?.Length ?? 0,
                "StbSecWallLoad" => stb?.StbModel?.StbSections?.StbSecWallLoad?.Length ?? 0,
                "StbSecIsolatingDevice" => stb?.StbModel?.StbSections?.StbSecIsolatingDevice?.Length ?? 0,
                "StbSecDampingDevice" => stb?.StbModel?.StbSections?.StbSecDampingDevice?.Length ?? 0,
                "StbSecFoundation_RC" => stb?.StbModel?.StbSections?.StbSecFoundation_RC?.Length ?? 0,
                "StbSecPile_RC" => stb?.StbModel?.StbSections?.StbSecPile_RC?.Length ?? 0,
                "StbSecPile_S" => stb?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0,
                "StbSecPilePrecast" => stb?.StbModel?.StbSections?.StbSecPilePrecast?.Length ?? 0,
                "StbSecParapet_RC" => stb?.StbModel?.StbSections?.StbSecParapet_RC?.Length ?? 0,
                "StbSecPenetration_S" => stb?.StbModel?.StbSections?.StbSecPenetration_S?.Length ?? 0,
                "StbSecPanelZone" => stb?.StbModel?.StbSections?.StbSecPanelZone != null ? 1 : 0,
                "StbJoints" => (stb?.StbModel?.StbJoints?.StbJointColumnShapeH?.Length ?? 0)
                             + (stb?.StbModel?.StbJoints?.StbJointColumnShapeT?.Length ?? 0)
                             + (stb?.StbModel?.StbJoints?.StbJointColumnShapeCross?.Length ?? 0)
                             + (stb?.StbModel?.StbJoints?.StbJointBeamShapeH?.Length ?? 0),
                "StbConnections" => (stb?.StbModel?.StbConnections?.StbGussetPlates?.Length ?? 0)
                                  + (stb?.StbModel?.StbConnections?.StbRibPlates?.Length ?? 0)
                                  + (stb?.StbModel?.StbConnections?.StbDiaphragms?.Length ?? 0)
                                  + (stb?.StbModel?.StbConnections?.StbStiffners?.Length ?? 0),
                "StbWeld" => stb?.StbModel?.StbWeld != null ? 1 : 0,
                _ => throw new NotImplementedException(tabName),
            };
        }
    }
}

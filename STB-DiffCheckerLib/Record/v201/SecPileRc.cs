using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecPileRc
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stbridgeA?.StbModel?.StbSections?.StbSecPile_RC;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecPile_RC;
            var setB = secB != null ? new HashSet<StbSecPile_RC>(secB) : new HashSet<StbSecPile_RC>();

            if (secA != null)
            {
                foreach (var secPileA in secA)
                {
                    var key = new List<string> { "Name=" + secPileA.name };
                    var secPileB = secB?.FirstOrDefault(n => n.name == secPileA.name);
                    if (secPileB != null)
                    {
                        CompareSecPileRc(secPileA, secPileB, key, records);
                        setB.Remove(secPileB);
                    }
                    else
                    {
                        StbSecPileRc.Compare(nameof(StbSecPile_RC), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { "Name=" + b.name };
                StbSecPileRc.Compare(null, nameof(StbSecPile_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecPileRc(StbSecPile_RC secPileA, StbSecPile_RC secPileB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecPileRcId.Compare(secPileA.id, secPileB.id, key, records);
            StbSecPileRcGuid.Compare(secPileA.guid, secPileB.guid, key, records);
            StbSecPileRcName.Compare(secPileA.name, secPileB.name, key, records);
            StbSecPileRcStrengthConcrete.Compare(secPileA.strength_concrete, secPileB.strength_concrete, key, records);

            CompareStbSecFigurePileRc(secPileA.StbSecFigurePile_RC, secPileB.StbSecFigurePile_RC, key, records);

            if (secPileA.StbSecBarArrangementPile_RC != null || secPileB.StbSecBarArrangementPile_RC != null)
            {
                if (secPileB.StbSecBarArrangementPile_RC == null)
                {
                    StbSecBarArrangementPileRc.Compare(nameof(StbSecBarArrangementPile_RC), null, key, records);
                }
                else if (secPileA.StbSecBarArrangementPile_RC == null)
                {
                    StbSecBarArrangementPileRc.Compare(null, nameof(StbSecBarArrangementPile_RC), key, records);
                }
                else
                {
                    CompareStbSecBarArrangementPileRc(secPileA.StbSecBarArrangementPile_RC,
                        secPileB.StbSecBarArrangementPile_RC, key, records);
                }
            }

        }

        private static void CompareStbSecBarArrangementPileRc(StbSecBarArrangementPile_RC secA, StbSecBarArrangementPile_RC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementPileRcDepthCover.Compare(secA.depth_coverSpecified, secA.depth_cover,
                secB.depth_coverSpecified, secB.depth_cover, key, records);
            StbSecBarArrangementPileRcDepthCoverTop.Compare(secA.depth_cover_topSpecified, secA.depth_cover_top,
                secB.depth_cover_topSpecified, secB.depth_cover_top, key, records);
            StbSecBarArrangementPileRcIsSpiral.Compare(secA.isSpiral, secB.isSpiral, key, records);

            if (secA.Items.Any(n => n is StbSecBarPile_RC_Same))
            {
                if (secB.Items.Any(n => n is StbSecBarPile_RC_Same))
                {
                    var a = secA.Items.First() as StbSecBarPile_RC_Same;
                    var b = secB.Items.First() as StbSecBarPile_RC_Same;
                    StbSecBarPileRcSameDMainCircumference1st.Compare(a.D_main_circumference_1st, b.D_main_circumference_1st, key, records);
                    StbSecBarPileRcSameDMainCircumference2nd.Compare(a.D_main_circumference_2nd, b.D_main_circumference_2nd, key, records);
                    StbSecBarPileRcSameDMainCore.Compare(a.D_main_core, b.D_main_core, key, records);
                    StbSecBarPileRcSameDBand.Compare(a.D_band, b.D_band, key, records);
                    StbSecBarPileRcSameStrengthMainCircumference1st.Compare(a.strength_main_circumference_1st,
                        b.strength_main_circumference_1st, key, records);
                    StbSecBarPileRcSameStrengthMainCircumference2nd.Compare(a.strength_main_circumference_2nd,
                        b.strength_main_circumference_2nd, key, records);
                    StbSecBarPileRcSameStrengthMainCore.Compare(a.strength_main_core, b.strength_main_core, key, records);
                    StbSecBarPileRcSameStrengthBand.Compare(a.strength_band, b.strength_band, key, records);
                    StbSecBarPileRcSameNMainCircumference1st.Compare(a.N_main_circumference_1st, b.N_main_circumference_1st, key, records);
                    StbSecBarPileRcSameNMainCircumference2nd.Compare(a.N_main_circumference_2nd, b.N_main_circumference_2nd, key, records);
                    StbSecBarPileRcSameNMainCore.Compare(a.N_main_core, b.N_main_core, key, records);
                    StbSecBarPileRcSamePitchBand.Compare(a.pitch_band, b.pitch_band, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarPile_RC_TopBottom))
                {
                    StbSecBarPileRcSame.Compare(nameof(StbSecBarPile_RC_Same), nameof(StbSecBarPile_RC_TopBottom), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarPile_RC_TopCenterBottom))
                {
                    StbSecBarPileRcSame.Compare(nameof(StbSecBarPile_RC_Same), nameof(StbSecBarPile_RC_TopCenterBottom), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarPile_RC_TopBottom))
            {
                if (secB.Items.Any(n => n is StbSecBarPile_RC_Same))
                {
                    StbSecBarPileRcTopBottom.Compare(nameof(StbSecBarPile_RC_TopBottom), nameof(StbSecBarPile_RC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarPile_RC_TopBottom))
                {
                    var set = new HashSet<StbSecBarPile_RC_TopBottom>();
                    foreach (var a in secA.Items.OfType<StbSecBarPile_RC_TopBottom>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarPile_RC_TopBottom>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarPileRcTopBottomPos.Compare(a.pos.ToString(), b.pos.ToString(), key, records);
                            StbSecBarPileRcTopBottomDMainCircumference1st.Compare(a.D_main_circumference_1st, b.D_main_circumference_1st, key, records);
                            StbSecBarPileRcTopBottomDMainCircumference2nd.Compare(a.D_main_circumference_2nd, b.D_main_circumference_2nd, key, records);
                            StbSecBarPileRcTopBottomDMainCore.Compare(a.D_main_core, b.D_main_core, key, records);
                            StbSecBarPileRcTopBottomDBand.Compare(a.D_band, b.D_band, key, records);
                            StbSecBarPileRcTopBottomStrengthMainCircumference1st.Compare(a.strength_main_circumference_1st,
                                b.strength_main_circumference_1st, key, records);
                            StbSecBarPileRcTopBottomStrengthMainCircumference2nd.Compare(a.strength_main_circumference_2nd,
                                b.strength_main_circumference_2nd, key, records);
                            StbSecBarPileRcTopBottomStrengthMainCore.Compare(a.strength_main_core, b.strength_main_core, key, records);
                            StbSecBarPileRcTopBottomStrengthBand.Compare(a.strength_band, b.strength_band, key, records);
                            StbSecBarPileRcTopBottomNMainCircumference1st.Compare(a.N_main_circumference_1st, b.N_main_circumference_1st, key, records);
                            StbSecBarPileRcTopBottomNMainCircumference2nd.Compare(a.N_main_circumference_2nd, b.N_main_circumference_2nd, key, records);
                            StbSecBarPileRcTopBottomNMainCore.Compare(a.N_main_core, b.N_main_core, key, records);
                            StbSecBarPileRcTopBottomPitchBand.Compare(a.pitch_band, b.pitch_band, key, records);
                            StbSecBarPileRcTopBottomLengthBar.Compare(a.length_barSpecified, a.length_bar,
                                b.length_barSpecified, b.length_bar, key, records);
                            StbSecBarPileRcTopBottomLengthLapBar.Compare(a.length_lap_barSpecified, a.length_lap_bar,
                                b.length_lap_barSpecified, b.length_lap_bar, key, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarPile_RC_TopBottom>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarPileRcTopBottom.Compare(null, nameof(StbSecBarPile_RC_TopBottom), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarPile_RC_TopCenterBottom))
                {
                    StbSecBarPileRcTopBottom.Compare(nameof(StbSecBarPile_RC_TopBottom), nameof(StbSecBarPile_RC_TopCenterBottom), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarPile_RC_TopCenterBottom))
            {
                if (secB.Items.Any(n => n is StbSecBarPile_RC_Same))
                {
                    StbSecBarPileRcTopCenterBottom.Compare(nameof(StbSecBarPile_RC_TopCenterBottom), nameof(StbSecBarPile_RC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarPile_RC_TopBottom))
                {
                    StbSecBarPileRcTopCenterBottom.Compare(nameof(StbSecBarPile_RC_TopCenterBottom), nameof(StbSecBarPile_RC_TopBottom), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarPile_RC_TopCenterBottom))
                {
                    var set = new HashSet<StbSecBarPile_RC_TopCenterBottom>();
                    foreach (var a in secA.Items.OfType<StbSecBarPile_RC_TopCenterBottom>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarPile_RC_TopCenterBottom>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarPileRcTopCenterBottomPos.Compare(a.pos.ToString(), b.pos.ToString(), key, records);
                            StbSecBarPileRcTopCenterBottomDMainCircumference1st.Compare(a.D_main_circumference_1st, b.D_main_circumference_1st, key, records);
                            StbSecBarPileRcTopCenterBottomDMainCircumference2nd.Compare(a.D_main_circumference_2nd, b.D_main_circumference_2nd, key, records);
                            StbSecBarPileRcTopCenterBottomDMainCore.Compare(a.D_main_core, b.D_main_core, key, records);
                            StbSecBarPileRcTopCenterBottomDBand.Compare(a.D_band, b.D_band, key, records);
                            StbSecBarPileRcTopCenterBottomStrengthMainCircumference1st.Compare(a.strength_main_circumference_1st,
                                b.strength_main_circumference_1st, key, records);
                            StbSecBarPileRcTopCenterBottomStrengthMainCircumference2nd.Compare(a.strength_main_circumference_2nd,
                                b.strength_main_circumference_2nd, key, records);
                            StbSecBarPileRcTopCenterBottomStrengthMainCore.Compare(a.strength_main_core, b.strength_main_core, key, records);
                            StbSecBarPileRcTopCenterBottomStrengthBand.Compare(a.strength_band, b.strength_band, key, records);
                            StbSecBarPileRcTopCenterBottomNMainCircumference1st.Compare(a.N_main_circumference_1st, b.N_main_circumference_1st, key, records);
                            StbSecBarPileRcTopCenterBottomNMainCircumference2nd.Compare(a.N_main_circumference_2nd, b.N_main_circumference_2nd, key, records);
                            StbSecBarPileRcTopCenterBottomNMainCore.Compare(a.N_main_core, b.N_main_core, key, records);
                            StbSecBarPileRcTopCenterBottomPitchBand.Compare(a.pitch_band, b.pitch_band, key, records);
                            StbSecBarPileRcTopCenterBottomLengthBar.Compare(a.length_barSpecified, a.length_bar,
                                b.length_barSpecified, b.length_bar, key, records);
                            StbSecBarPileRcTopCenterBottomLengthLapBar.Compare(a.length_lap_barSpecified, a.length_lap_bar,
                                b.length_lap_barSpecified, b.length_lap_bar, key, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarPile_RC_TopCenterBottom>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarPileRcTopCenterBottom.Compare(null, nameof(StbSecBarPile_RC_TopCenterBottom), keyB, records);
                        }
                    }
                }
            }
        }

        private static void CompareStbSecFigurePileRc(StbSecFigurePile_RC secA, StbSecFigurePile_RC secB,
            IReadOnlyList<string> key, List<Record> records)
        {

            StbSecFigurePileRcLengthPipe.Compare(secA.length_pipeSpecified, secA.length_pipe,
                secB.length_pipeSpecified, secB.length_pipe, key, records);
            StbSecFigurePileRcTPipe.Compare(secA.t_pipeSpecified, secA.t_pipe,
                secB.t_pipeSpecified, secB.t_pipe, key, records);
            StbSecFigurePileRcStrengthPipe.Compare(secA.strength_pipe, secB.strength_pipe, key, records);

            if (secA.Item is StbSecPile_RC_Straight straightA)
            {
                if (secB.Item is StbSecPile_RC_Straight straightB)
                {
                    StbSecPileRcStraightD.Compare(straightA.D, straightB.D, key, records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedFoot)
                {
                    StbSecPileRcStraight.Compare(nameof(StbSecPile_RC_Straight), nameof(StbSecPile_RC_ExtendedFoot), key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTop)
                {
                    StbSecPileRcStraight.Compare(nameof(StbSecPile_RC_Straight), nameof(StbSecPile_RC_ExtendedTop), key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTopFoot)
                {
                    StbSecPileRcStraight.Compare(nameof(StbSecPile_RC_Straight), nameof(StbSecPile_RC_ExtendedTopFoot), key,
                        records);
                }
            }
            else if (secA.Item is StbSecPile_RC_ExtendedFoot footA)
            {
                if (secB.Item is StbSecPile_RC_Straight)
                {
                    StbSecPileRcExtendedFoot.Compare(nameof(StbSecPile_RC_ExtendedFoot), nameof(StbSecPile_RC_Straight), key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedFoot footB)
                {
                    StbSecPileRcExtendedFootDAxial.Compare(footA.D_axial, footB.D_axial, key, records);
                    StbSecPileRcExtendedFootDExtendedFoot.Compare(footA.D_extended_foot, footB.D_extended_foot, key, records);
                    StbSecPileRcExtendedFootLengthExtendedFoot.Compare(footA.length_extended_foot, footB.length_extended_foot, key, records);
                    StbSecPileRcExtendedFootAngleExtendedFootTaper.Compare(footA.angle_extended_foot_taper, footB.angle_extended_foot_taper, key, records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTop)
                {
                    StbSecPileRcExtendedFoot.Compare(nameof(StbSecPile_RC_ExtendedFoot), nameof(StbSecPile_RC_ExtendedTop), key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTopFoot)
                {
                    StbSecPileRcExtendedFoot.Compare(nameof(StbSecPile_RC_ExtendedFoot), nameof(StbSecPile_RC_ExtendedTopFoot), key,
                        records);
                }
            }
            if (secA.Item is StbSecPile_RC_ExtendedTop topA)
            {
                if (secB.Item is StbSecPile_RC_Straight)
                {
                    StbSecPileRcExtendedTop.Compare(nameof(StbSecPile_RC_ExtendedTop), nameof(StbSecPile_RC_Straight), key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedFoot)
                {
                    StbSecPileRcExtendedTop.Compare(nameof(StbSecPile_RC_ExtendedTop), nameof(StbSecPile_RC_ExtendedFoot), key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTop topB)
                {
                    StbSecPileRcExtendedTopDExtendedTop.Compare(topA.D_extended_top, topB.D_extended_top, key, records);
                    StbSecPileRcExtendedTopDAxial.Compare(topA.D_axial, topB.D_axial, key, records);
                    StbSecPileRcExtendedTopAngleExtendedTopTaper.Compare(topA.angle_extended_top_taper, topB.angle_extended_top_taper, key, records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTopFoot)
                {
                    StbSecPileRcExtendedTop.Compare(nameof(StbSecPile_RC_ExtendedTop), nameof(StbSecPile_RC_ExtendedTopFoot), key, records);
                }
            }

            if (secA.Item is StbSecPile_RC_ExtendedTopFoot topFootA)
            {
                if (secB.Item is StbSecPile_RC_Straight)
                {
                    StbSecPileRcExtendedTopFoot.Compare(nameof(StbSecPile_RC_ExtendedTopFoot), nameof(StbSecPile_RC_Straight),
                        key,
                        records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedFoot)
                {
                    StbSecPileRcExtendedTopFoot.Compare(nameof(StbSecPile_RC_ExtendedTopFoot), nameof(StbSecPile_RC_ExtendedFoot), key, records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTop)
                {
                    StbSecPileRcExtendedTopFoot.Compare(nameof(StbSecPile_RC_ExtendedTopFoot), nameof(StbSecPile_RC_ExtendedTop), key, records);
                }
                else if (secB.Item is StbSecPile_RC_ExtendedTopFoot topFootB)
                {
                    StbSecPileRcExtendedTopFootDExtendedTop.Compare(topFootA.D_extended_top, topFootB.D_extended_top, key, records);
                    StbSecPileRcExtendedTopFootDAxial.Compare(topFootA.D_axial, topFootB.D_axial, key, records);
                    StbSecPileRcExtendedTopFootDExtendedFoot.Compare(topFootA.D_extended_foot, topFootB.D_extended_foot, key, records);
                    StbSecPileRcExtendedTopFootAngleExtendedTopTaper.Compare(topFootA.angle_extended_top_taper, topFootB.angle_extended_top_taper, key, records);
                    StbSecPileRcExtendedTopFootLengthExtendedFoot.Compare(topFootA.length_extended_foot, topFootB.length_extended_foot, key, records);
                    StbSecPileRcExtendedTopFootAngleExtendedFootTaper.Compare(topFootA.angle_extended_foot_taper, topFootB.angle_extended_foot_taper, key, records);
                }
            }
        }
    }
}

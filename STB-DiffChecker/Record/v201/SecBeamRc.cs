using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbSecBarBeamXReinforced = STBridge201.StbSecBarBeamXReinforced;

namespace STBDiffChecker.v201.Records
{
    internal static class SecBeamRc
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stbridgeA?.StbModel?.StbSections?.StbSecBeam_RC;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecBeam_RC;
            var setB = secB != null ? new HashSet<StbSecBeam_RC>(secB) : new HashSet<StbSecBeam_RC>();

            if (secA != null)
            {
                foreach (var secBeamA in secA)
                {
                    var key = new List<string> {$"Name={secBeamA.name}", $"floor={secBeamA.floor}"};
                    var secBeamB = secB?.FirstOrDefault(n => n.name == secBeamA.name && n.floor == secBeamA.floor);
                    if (secBeamB != null)
                    {
                        CompareSecBeamRc(secBeamA, secBeamB, key, records);
                        setB.Remove(secBeamB);
                    }
                    else
                    {
                        StbSecBeamRc.Compare(nameof(StbSecBeam_RC), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> {$"Name={b.name}", $"floor={b.floor}"};
                StbSecBeamRc.Compare(null, nameof(StbSecBeam_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecBeamRc(StbSecBeam_RC secBeamA, StbSecBeam_RC secBeamB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBeamRcId.Compare(secBeamA.id, secBeamB.id, key, records);
            StbSecBeamRcGuid.Compare(secBeamA.guid, secBeamB.guid, key, records);
            StbSecBeamRcName.Compare(secBeamA.name, secBeamB.name, key, records);
            StbSecBeamRcFloor.Compare(secBeamA.floor, secBeamB.floor, key, records);
            StbSecBeamRcKindBeam.Compare(secBeamA.kind_beam.ToString(), secBeamB.kind_beam.ToString(), key, records);
            StbSecBeamRcIsFoundation.Compare(secBeamA.isFoundation, secBeamB.isFoundation, key, records);
            StbSecBeamRcIsCanti.Compare(secBeamA.isCanti, secBeamB.isCanti, key, records);
            StbSecBeamRcIsOutin.Compare(secBeamA.isOutin, secBeamB.isOutin, key, records);
            StbSecBeamRcStrengthConcrete.Compare(secBeamA.strength_concrete, secBeamB.strength_concrete, key, records);

            CompareStbSecFigureBeamRc(secBeamA.StbSecFigureBeam_RC, secBeamB.StbSecFigureBeam_RC, key, records);

            if (secBeamA.StbSecBarArrangementBeam_RC != null || secBeamB.StbSecBarArrangementBeam_RC != null)
            {
                if (secBeamB.StbSecBarArrangementBeam_RC == null)
                {
                    StbSecBarArrangementBeamRc.Compare(nameof(StbSecBarArrangementBeam_RC), null, key, records);
                }
                else if (secBeamA.StbSecBarArrangementBeam_RC == null)
                {
                    StbSecBarArrangementBeamRc.Compare(null, nameof(StbSecBarArrangementBeam_RC), key, records);
                }
                else
                {
                    CompareStbSecBarArrangementBeamRc(secBeamA.StbSecBarArrangementBeam_RC,
                        secBeamB.StbSecBarArrangementBeam_RC, key, records);
                }
            }

        }

        private static void CompareStbSecBarArrangementBeamRc(StbSecBarArrangementBeam_RC secA, StbSecBarArrangementBeam_RC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementBeamRcDepthCoverLeft.Compare(secA.depth_cover_leftSpecified, secA.depth_cover_left,
                secB.depth_cover_leftSpecified, secB.depth_cover_left, key, records);
            StbSecBarArrangementBeamRcDepthCoverRight.Compare(secA.depth_cover_rightSpecified, secA.depth_cover_right,
                secB.depth_cover_rightSpecified, secB.depth_cover_right, key, records);
            StbSecBarArrangementBeamRcDepthCoverTop.Compare(secA.depth_cover_topSpecified, secA.depth_cover_top,
                secB.depth_cover_topSpecified, secB.depth_cover_top, key, records);
            StbSecBarArrangementBeamRcDepthCoverBottom.Compare(secA.depth_cover_bottomSpecified, secA.depth_cover_bottom,
                secB.depth_cover_bottomSpecified, secB.depth_cover_bottom, key, records);
            StbSecBarArrangementBeamRcInterval.Compare(secA.intervalSpecified, secA.interval, secB.intervalSpecified,
                secB.interval, key, records);
            StbSecBarArrangementBeamRcCenterTop.Compare(secA.center_topSpecified, secA.center_top,
                secB.center_topSpecified, secB.center_top, key, records);
            StbSecBarArrangementBeamRcCenterBottom.Compare(secA.center_bottomSpecified, secA.center_bottom,
                secB.center_bottomSpecified, secB.center_bottom, key, records);
            StbSecBarArrangementBeamRcCenterSide.Compare(secA.center_sideSpecified, secA.center_side,
                secB.center_sideSpecified, secB.center_side, key, records);
            StbSecBarArrangementBeamRcCenterInterval.Compare(secA.center_intervalSpecified, secA.center_interval,
                secB.center_intervalSpecified, secB.center_interval, key, records);
            StbSecBarArrangementBeamRcLengthBarStart.Compare(secA.length_bar_startSpecified, secA.length_bar_start,
                secB.length_bar_startSpecified, secB.length_bar_start, key, records);
            StbSecBarArrangementBeamRcLengthBarEnd.Compare(secA.length_bar_endSpecified, secA.length_bar_end,
                secB.length_bar_endSpecified, secB.length_bar_end, key, records);


            if (secA.Items.Any(n => n is StbSecBarBeam_RC_Same))
            {
                if (secB.Items.Any(n => n is StbSecBarBeam_RC_Same))
                {
                    var a = secA.Items.OfType<StbSecBarBeam_RC_Same>().First();
                    var b = secB.Items.OfType<StbSecBarBeam_RC_Same>().First();
                    StbSecBarBeamRcSameDMain.Compare(a.D_main, b.D_main, key, records);
                    StbSecBarBeamRcSameD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key, records);
                    StbSecBarBeamRcSameDStirrup.Compare(a.D_stirrup, b.D_stirrup, key, records);
                    StbSecBarBeamRcSameDWeb.Compare(a.D_web, b.D_web, key, records);
                    StbSecBarBeamRcSameDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key, records);
                    StbSecBarBeamRcSameStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                    StbSecBarBeamRcSameStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key, records);
                    StbSecBarBeamRcSameStrengthStirrup.Compare(a.strength_stirrup, b.strength_stirrup, key, records);
                    StbSecBarBeamRcSameStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                    StbSecBarBeamRcSameStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key, records);
                    StbSecBarBeamRcSameNMainTop1st.Compare(a.N_main_top_1st, b.N_main_top_1st, key, records);
                    StbSecBarBeamRcSameNMainTop2nd.Compare(a.N_main_top_2nd, b.N_main_top_2nd, key, records);
                    StbSecBarBeamRcSameNMainTop3rd.Compare(a.N_main_top_3rd, b.N_main_top_3rd, key, records);
                    StbSecBarBeamRcSameNMainBottom1st.Compare(a.N_main_bottom_1st, b.N_main_bottom_1st, key, records);
                    StbSecBarBeamRcSameNMainBottom2nd.Compare(a.N_main_bottom_2nd, b.N_main_bottom_2nd, key, records);
                    StbSecBarBeamRcSameNMainBottom3rd.Compare(a.N_main_bottom_3rd, b.N_main_bottom_3rd, key, records);
                    StbSecBarBeamRcSameNMain2ndTop1st.Compare(a.N_2nd_main_top_1st, b.N_2nd_main_top_1st, key, records);
                    StbSecBarBeamRcSameNMain2ndTop2nd.Compare(a.N_2nd_main_top_2nd, b.N_2nd_main_top_2nd, key, records);
                    StbSecBarBeamRcSameNMain2ndTop3rd.Compare(a.N_2nd_main_top_3rd, b.N_2nd_main_top_3rd, key, records);
                    StbSecBarBeamRcSameNMain2ndBottom1st.Compare(a.N_2nd_main_bottom_1st, b.N_2nd_main_bottom_1st, key, records);
                    StbSecBarBeamRcSameNMain2ndBottom2nd.Compare(a.N_2nd_main_bottom_2nd, b.N_2nd_main_bottom_2nd, key, records);
                    StbSecBarBeamRcSameNMain2ndBottom3rd.Compare(a.N_2nd_main_bottom_3rd, b.N_2nd_main_bottom_3rd, key, records);
                    StbSecBarBeamRcSameNStirrup.Compare(a.N_stirrup, b.N_stirrup, key, records);
                    StbSecBarBeamRcSamePitchStirrup.Compare(a.pitch_stirrup, b.pitch_stirrup, key, records);
                    StbSecBarBeamRcSameNWeb.Compare(a.N_web, b.N_web, key, records);
                    StbSecBarBeamRcSameNBarSpacing.Compare(a.N_bar_spacing, b.N_bar_spacing, key, records);
                    StbSecBarBeamRcSamePitchBarSpacing.Compare(a.pitch_bar_spacingSpecified, a.pitch_bar_spacing,
                        b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_RC_ThreeTypes))
                {
                    StbSecBarBeamRcSame.Compare(nameof(StbSecBarBeam_RC_Same), nameof(StbSecBarBeam_RC_ThreeTypes), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_RC_StartEnd))
                {
                    StbSecBarBeamRcSame.Compare(nameof(StbSecBarBeam_RC_Same), nameof(StbSecBarBeam_RC_StartEnd), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarBeam_RC_ThreeTypes))
            {
                if (secB.Items.Any(n => n is StbSecBarBeam_RC_Same))
                {
                    StbSecBarBeamRcThreeTypes.Compare(nameof(StbSecBarBeam_RC_ThreeTypes), nameof(StbSecBarBeam_RC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_RC_ThreeTypes))
                {
                    var set = new HashSet<StbSecBarBeam_RC_ThreeTypes>();
                    foreach (var a in secA.Items.OfType<StbSecBarBeam_RC_ThreeTypes>())
                    {
                        var key1 = new List<string>(key) { $"pos={a.pos}"};
                        var b = secB.Items.OfType<StbSecBarBeam_RC_ThreeTypes>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarBeamRcThreeTypesPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarBeamRcThreeTypesDMain.Compare(a.D_main, b.D_main, key1, records);
                            StbSecBarBeamRcThreeTypesD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key1, records);
                            StbSecBarBeamRcThreeTypesDStirrup.Compare(a.D_stirrup, b.D_stirrup, key1, records);
                            StbSecBarBeamRcThreeTypesDWeb.Compare(a.D_web, b.D_web, key1, records);
                            StbSecBarBeamRcThreeTypesDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key1, records);
                            StbSecBarBeamRcThreeTypesStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecBarBeamRcThreeTypesStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key1, records);
                            StbSecBarBeamRcThreeTypesStrengthStirrup.Compare(a.strength_stirrup, b.strength_stirrup, key1, records);
                            StbSecBarBeamRcThreeTypesStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            StbSecBarBeamRcThreeTypesStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key1, records);
                            StbSecBarBeamRcThreeTypesNMainTop1st.Compare(a.N_main_top_1st, b.N_main_top_1st, key1, records);
                            StbSecBarBeamRcThreeTypesNMainTop2nd.Compare(a.N_main_top_2nd, b.N_main_top_2nd, key1, records);
                            StbSecBarBeamRcThreeTypesNMainTop3rd.Compare(a.N_main_top_3rd, b.N_main_top_3rd, key1, records);
                            StbSecBarBeamRcThreeTypesNMainBottom1st.Compare(a.N_main_bottom_1st, b.N_main_bottom_1st, key1, records);
                            StbSecBarBeamRcThreeTypesNMainBottom2nd.Compare(a.N_main_bottom_2nd, b.N_main_bottom_2nd, key1, records);
                            StbSecBarBeamRcThreeTypesNMainBottom3rd.Compare(a.N_main_bottom_3rd, b.N_main_bottom_3rd, key1, records);
                            StbSecBarBeamRcThreeTypesNMain2ndTop1st.Compare(a.N_2nd_main_top_1st, b.N_2nd_main_top_1st, key1, records);
                            StbSecBarBeamRcThreeTypesNMain2ndTop2nd.Compare(a.N_2nd_main_top_2nd, b.N_2nd_main_top_2nd, key1, records);
                            StbSecBarBeamRcThreeTypesNMain2ndTop3rd.Compare(a.N_2nd_main_top_3rd, b.N_2nd_main_top_3rd, key1, records);
                            StbSecBarBeamRcThreeTypesNMain2ndBottom1st.Compare(a.N_2nd_main_bottom_1st, b.N_2nd_main_bottom_1st, key1, records);
                            StbSecBarBeamRcThreeTypesNMain2ndBottom2nd.Compare(a.N_2nd_main_bottom_2nd, b.N_2nd_main_bottom_2nd, key1, records);
                            StbSecBarBeamRcThreeTypesNMain2ndBottom3rd.Compare(a.N_2nd_main_bottom_3rd, b.N_2nd_main_bottom_3rd, key1, records);
                            StbSecBarBeamRcThreeTypesNStirrup.Compare(a.N_stirrup, b.N_stirrup, key1, records);
                            StbSecBarBeamRcThreeTypesPitchStirrup.Compare(a.pitch_stirrup, b.pitch_stirrup, key1, records);
                            StbSecBarBeamRcThreeTypesNWeb.Compare(a.N_web, b.N_web, key1, records);
                            StbSecBarBeamRcThreeTypesNBarSpacing.Compare(a.N_bar_spacing, b.N_bar_spacing, key1, records);
                            StbSecBarBeamRcThreeTypesPitchBarSpacing.Compare(a.pitch_bar_spacingSpecified, a.pitch_bar_spacing,
                                b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarBeam_RC_ThreeTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { $"pos={b.pos}" };
                            StbSecBarBeamRcThreeTypes.Compare(null, nameof(StbSecBarBeam_RC_ThreeTypes), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_RC_StartEnd))
                {
                    StbSecBarBeamRcThreeTypes.Compare(nameof(StbSecBarBeam_RC_ThreeTypes), nameof(StbSecBarBeam_RC_StartEnd), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarBeam_RC_StartEnd))
            {
                if (secB.Items.Any(n => n is StbSecBarBeam_RC_Same))
                {
                    StbSecBarBeamRcStartEnd.Compare(nameof(StbSecBarBeam_RC_StartEnd), nameof(StbSecBarBeam_RC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_RC_ThreeTypes))
                {
                    StbSecBarBeamRcStartEnd.Compare(nameof(StbSecBarBeam_RC_StartEnd), nameof(StbSecBarBeam_RC_ThreeTypes), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_RC_StartEnd))
                {
                    var set = new HashSet<StbSecBarBeam_RC_StartEnd>();
                    foreach (var a in secA.Items.OfType<StbSecBarBeam_RC_StartEnd>())
                    {
                        var key1 = new List<string>(key) { $"pos={a.pos}" };
                        var b = secB.Items.OfType<StbSecBarBeam_RC_StartEnd>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarBeamRcStartEndPos.Compare(a.pos.ToString(), b.pos.ToString(), key, records);
                            StbSecBarBeamRcStartEndDMain.Compare(a.D_main, b.D_main, key, records);
                            StbSecBarBeamRcStartEndD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key, records);
                            StbSecBarBeamRcStartEndDStirrup.Compare(a.D_stirrup, b.D_stirrup, key, records);
                            StbSecBarBeamRcStartEndDWeb.Compare(a.D_web, b.D_web, key, records);
                            StbSecBarBeamRcStartEndDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key, records);
                            StbSecBarBeamRcStartEndStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                            StbSecBarBeamRcStartEndStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key, records);
                            StbSecBarBeamRcStartEndStrengthStirrup.Compare(a.strength_stirrup, b.strength_stirrup, key, records);
                            StbSecBarBeamRcStartEndStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                            StbSecBarBeamRcStartEndStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key, records);
                            StbSecBarBeamRcStartEndNMainTop1st.Compare(a.N_main_top_1st, b.N_main_top_1st, key, records);
                            StbSecBarBeamRcStartEndNMainTop2nd.Compare(a.N_main_top_2nd, b.N_main_top_2nd, key, records);
                            StbSecBarBeamRcStartEndNMainTop3rd.Compare(a.N_main_top_3rd, b.N_main_top_3rd, key, records);
                            StbSecBarBeamRcStartEndNMainBottom1st.Compare(a.N_main_bottom_1st, b.N_main_bottom_1st, key, records);
                            StbSecBarBeamRcStartEndNMainBottom2nd.Compare(a.N_main_bottom_2nd, b.N_main_bottom_2nd, key, records);
                            StbSecBarBeamRcStartEndNMainBottom3rd.Compare(a.N_main_bottom_3rd, b.N_main_bottom_3rd, key, records);
                            StbSecBarBeamRcStartEndNMain2ndTop1st.Compare(a.N_2nd_main_top_1st, b.N_2nd_main_top_1st, key, records);
                            StbSecBarBeamRcStartEndNMain2ndTop2nd.Compare(a.N_2nd_main_top_2nd, b.N_2nd_main_top_2nd, key, records);
                            StbSecBarBeamRcStartEndNMain2ndTop3rd.Compare(a.N_2nd_main_top_3rd, b.N_2nd_main_top_3rd, key, records);
                            StbSecBarBeamRcStartEndNMain2ndBottom1st.Compare(a.N_2nd_main_bottom_1st, b.N_2nd_main_bottom_1st, key, records);
                            StbSecBarBeamRcStartEndNMain2ndBottom2nd.Compare(a.N_2nd_main_bottom_2nd, b.N_2nd_main_bottom_2nd, key, records);
                            StbSecBarBeamRcStartEndNMain2ndBottom3rd.Compare(a.N_2nd_main_bottom_3rd, b.N_2nd_main_bottom_3rd, key, records);
                            StbSecBarBeamRcStartEndNStirrup.Compare(a.N_stirrup, b.N_stirrup, key, records);
                            StbSecBarBeamRcStartEndPitchStirrup.Compare(a.pitch_stirrup, b.pitch_stirrup, key, records);
                            StbSecBarBeamRcStartEndNWeb.Compare(a.N_web, b.N_web, key, records);
                            StbSecBarBeamRcStartEndNBarSpacing.Compare(a.N_bar_spacing, b.N_bar_spacing, key, records);
                            StbSecBarBeamRcStartEndPitchBarSpacing.Compare(a.pitch_bar_spacingSpecified, a.pitch_bar_spacing,
                                b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarBeam_RC_StartEnd>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { $"pos={b.pos}"};
                            StbSecBarBeamRcStartEnd.Compare(null, nameof(StbSecBarBeam_RC_StartEnd), keyB, records);
                        }
                    }
                }
            }

            if (secA.Items.Any(n => n is StbSecBarBeamXReinforced))
            {
                if (secB.Items.Any(n => n is StbSecBarBeamXReinforced))
                {
                    var a = secA.Items.OfType<StbSecBarBeamXReinforced>().First();
                    var b = secA.Items.OfType<StbSecBarBeamXReinforced>().First();
                    StbSecBarBeamXReinforcedNMainTop.Compare(a.N_main_top, b.N_main_top, key, records);
                    StbSecBarBeamXReinforcedNMainBottom.Compare(a.N_main_bottom, b.N_main_bottom, key, records);
                }
                else
                {
                    CheckObjects.StbSecBarBeamXReinforced.Compare(nameof(StbSecBarBeamXReinforced), null, key, records);
                }
            }
            else
            {
                if (secB.Items.Any(n => n is StbSecBarBeamXReinforced))
                {
                    CheckObjects.StbSecBarBeamXReinforced.Compare(null, nameof(StbSecBarBeamXReinforced), key, records);
                }
            }
        }

        private static void CompareStbSecFigureBeamRc(StbSecFigureBeam_RC secA, StbSecFigureBeam_RC secB,
            IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Items.Any(n => n is StbSecBeam_RC_Straight))
            {
                if (secB.Items.Any(n => n is StbSecBeam_RC_Straight))
                {
                    var straightA = secA.Items.OfType<StbSecBeam_RC_Straight>().First();
                    var straightB = secB.Items.OfType<StbSecBeam_RC_Straight>().First();
                    StbSecBeamRcStraightWidth.Compare(straightA.width, straightB.width, key, records);
                    StbSecBeamRcStraightDepth.Compare(straightA.depth, straightB.depth, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_RC_Taper))
                {
                    StbSecBeamRcStraight.Compare(nameof(StbSecBeam_RC_Straight), nameof(StbSecBeam_RC_Taper), key,
                        records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_RC_Haunch))
                {
                    StbSecBeamRcStraight.Compare(nameof(StbSecBeam_RC_Straight), nameof(StbSecBeam_RC_Haunch), key,
                        records);

                }
            }
            else if (secA.Items.Any(n => n is StbSecBeam_RC_Taper))
            {
                if (secB.Items.Any(n => n is StbSecBeam_RC_Straight))
                {
                    StbSecBeamRcTaper.Compare(nameof(StbSecBeam_RC_Taper), nameof(StbSecBeam_RC_Straight), key,
                        records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_RC_Taper))
                {
                    var set = new HashSet<StbSecBeam_RC_Taper>();
                    foreach (var a in secA.Items.OfType<StbSecBeam_RC_Taper>())
                    {
                        var key1 = new List<string>(key) {$"pos={a.pos}"};
                        var b = secB.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBeamRcTaperPos.Compare(a.pos.ToString(), b.pos.ToString(), key, records);
                            StbSecBeamRcTaperWidth.Compare(a.width, b.width, key, records);
                            StbSecBeamRcTaperDepth.Compare(a.depth, b.depth, key, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBeamRcTaper.Compare(nameof(StbSecBeam_RC_Taper), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBeam_RC_Taper>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {$"pos={b.pos}"};
                            StbSecBeamRcTaper.Compare(null, nameof(StbSecBeam_RC_Taper), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBeam_RC_Haunch))
                {
                    StbSecBeamRcTaper.Compare(nameof(StbSecBeam_RC_Taper), nameof(StbSecBeam_RC_Haunch), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBeam_RC_Haunch))
            {
                if (secB.Items.Any(n => n is StbSecBeam_RC_Straight))
                {
                    StbSecBeamRcHaunch.Compare(nameof(StbSecBeam_RC_Haunch), nameof(StbSecBeam_RC_Straight), key,
                        records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_RC_Taper))
                {
                    StbSecBeamRcHaunch.Compare(nameof(StbSecBeam_RC_Haunch), nameof(StbSecBeam_RC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_RC_Haunch))
                {

                    var set = new HashSet<StbSecBeam_RC_Haunch>();
                    foreach (var a in secA.Items.OfType<StbSecBeam_RC_Haunch>())
                    {
                        var key1 = new List<string>(key) {$"pos={a.pos}"};
                        var b = secB.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBeamRcHaunchPos.Compare(a.pos.ToString(), b.pos.ToString(), key, records);
                            StbSecBeamRcHaunchWidth.Compare(a.width, b.width, key, records);
                            StbSecBeamRcHaunchDepth.Compare(a.depth, b.depth, key, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBeamRcHaunch.Compare(nameof(StbSecBeam_RC_Haunch), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBeam_RC_Haunch>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {$"pos={b.pos}"};
                            StbSecBeamRcHaunch.Compare(null, nameof(StbSecBeam_RC_Haunch), keyB, records);
                        }
                    }
                }
            }
        }
    }
}

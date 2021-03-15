using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecColumnRc
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_RC;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_RC;
            var setB = secB != null ? new HashSet<StbSecColumn_RC>(secB) : new HashSet<StbSecColumn_RC>();


            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name, "floor=" + secColumnA.floor };
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name && n.floor == secColumnA.floor); 
                    if (secColumnB != null)
                    {
                        CompareSecColumnRc(secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecColumnRc.Compare(nameof(StbSecColumn_RC), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> {"Name=" + b.name, "floor=" + b.floor};
                StbSecColumnRc.Compare(null, nameof(StbSecColumn_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecColumnRc(StbSecColumn_RC secColumnA, StbSecColumn_RC secColumnB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecColumnRcId.Compare(secColumnA.id, secColumnB.id, key, records);
            StbSecColumnRcGuid.Compare(secColumnA.guid, secColumnB.guid, key, records);
            StbSecColumnRcName.Compare(secColumnA.name, secColumnB.name, key, records);
            StbSecColumnRcFloor.Compare(secColumnA.floor, secColumnB.floor, key, records);
            StbSecColumnRcStrengthConcrete.Compare(secColumnA.strength_concrete, secColumnB.strength_concrete, key, records);

            CompareSecFigureColumnRc(secColumnA, secColumnB, key, records);

            if (secColumnA.StbSecBarArrangementColumn_RC != null ||
                secColumnB.StbSecBarArrangementColumn_RC != null)
            {
                if (secColumnA.StbSecBarArrangementColumn_RC == null)
                {
                    StbSecBarArrangementColumnRc.Compare(null, nameof(StbSecBarArrangementColumn_RC), key, records);
                }
                else if (secColumnB.StbSecBarArrangementColumn_RC == null)
                {
                    StbSecBarArrangementColumnRc.Compare(nameof(StbSecBarArrangementColumn_RC), null, key, records);
                }
                else
                {
                    CompareSecBarArrangement(secColumnA.StbSecBarArrangementColumn_RC,
                        secColumnB.StbSecBarArrangementColumn_RC, key, records);
                }
            }
        }

        private static void CompareSecBarArrangement(StbSecBarArrangementColumn_RC secA, StbSecBarArrangementColumn_RC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementColumnRcDepthCoverStartX.Compare(secA.depth_cover_start_XSpecified,
                secA.depth_cover_start_X, secB.depth_cover_start_XSpecified, secB.depth_cover_start_X, key, records);
            StbSecBarArrangementColumnRcDepthCoverEndX.Compare(secA.depth_cover_end_XSpecified, secA.depth_cover_end_X,
                secB.depth_cover_end_XSpecified, secB.depth_cover_end_X, key, records);
            StbSecBarArrangementColumnRcDepthCoverStartY.Compare(secA.depth_cover_start_YSpecified,
                secA.depth_cover_start_Y, secB.depth_cover_start_YSpecified, secB.depth_cover_start_Y, key, records);
            StbSecBarArrangementColumnRcDepthCoverEndY.Compare(secA.depth_cover_end_YSpecified, secA.depth_cover_end_Y,
                secB.depth_cover_end_YSpecified, secB.depth_cover_end_Y, key, records);
            StbSecBarArrangementColumnRcInterval.Compare(secA.intervalSpecified, secA.interval, secB.intervalSpecified,
                secB.interval, key, records);
            StbSecBarArrangementColumnRcIsSpiral.Compare(secA.isSpiral, secB.isSpiral, key, records);
            StbSecBarArrangementColumnRcCenterStartX.Compare(secA.center_start_XSpecified, secA.center_start_X,
                secB.center_start_XSpecified, secB.center_start_X, key, records);
            StbSecBarArrangementColumnRcCenterEndX.Compare(secA.center_end_XSpecified, secA.center_end_X,
                secB.center_end_XSpecified, secB.center_end_X, key, records);
            StbSecBarArrangementColumnRcCenterStartY.Compare(secA.center_start_YSpecified, secA.center_start_Y,
                secB.center_start_YSpecified, secB.center_start_Y, key, records);
            StbSecBarArrangementColumnRcCenterInterval.Compare(secA.center_intervalSpecified, secA.center_interval,
                secB.center_intervalSpecified, secB.center_interval, key, records);

            if (secA.Items.Any(n => n is StbSecBarColumn_RC_RectSame))
            {
                var rectSameA = secA.Items.OfType<StbSecBarColumn_RC_RectSame>().First();
                var rectSameB = secB.Items.OfType<StbSecBarColumn_RC_RectSame>().FirstOrDefault();
                if (rectSameB != null)
                {
                    StbSecBarColumnRcRectSameDMain.Compare(rectSameA.D_main, rectSameB.D_main, key, records);
                    StbSecBarColumnRcRectSameD2ndMain.Compare(rectSameA.D_2nd_main, rectSameB.D_2nd_main, key, records);
                    StbSecBarColumnRcRectSameDAxial.Compare(rectSameA.D_axial, rectSameB.D_axial, key, records);
                    StbSecBarColumnRcRectSameDBand.Compare(rectSameA.D_band, rectSameB.D_band, key, records);
                    StbSecBarColumnRcRectSameDBarSpacing.Compare(rectSameA.D_bar_spacing, rectSameB.D_bar_spacing, key, records);
                    StbSecBarColumnRcRectSameStrengthMain.Compare(rectSameA.strength_main, rectSameB.strength_main, key, records);
                    StbSecBarColumnRcRectSameStrength2ndMain.Compare(rectSameA.strength_2nd_main, rectSameB.strength_2nd_main, key, records);
                    StbSecBarColumnRcRectSameStrengthAxial.Compare(rectSameA.strength_axial, rectSameB.strength_axial, key, records);
                    StbSecBarColumnRcRectSameStrengthBand.Compare(rectSameA.strength_band, rectSameB.strength_band, key, records);
                    StbSecBarColumnRcRectSameStrengthBarSpacing.Compare(rectSameA.strength_bar_spacing, rectSameB.strength_bar_spacing, key, records);
                    StbSecBarColumnRcRectSameNMainX1st.Compare(rectSameA.N_main_X_1st, rectSameB.N_main_X_1st, key, records);
                    StbSecBarColumnRcRectSameNMainX2nd.Compare(rectSameA.N_main_X_2nd, rectSameB.N_main_X_2nd, key, records);
                    StbSecBarColumnRcRectSameNMainY1st.Compare(rectSameA.N_main_Y_1st, rectSameB.N_main_Y_1st, key, records);
                    StbSecBarColumnRcRectSameNMainY2nd.Compare(rectSameA.N_main_Y_2nd, rectSameB.N_main_Y_2nd, key, records);
                    StbSecBarColumnRcRectSameN2ndMainX1st.Compare(rectSameA.N_2nd_main_X_1st, rectSameB.N_2nd_main_X_1st, key, records);
                    StbSecBarColumnRcRectSameN2ndMainX2nd.Compare(rectSameA.N_2nd_main_X_2nd, rectSameB.N_2nd_main_X_2nd, key, records);
                    StbSecBarColumnRcRectSameN2ndMainY1st.Compare(rectSameA.N_2nd_main_Y_1st, rectSameB.N_2nd_main_Y_1st, key, records);
                    StbSecBarColumnRcRectSameN2ndMainY2nd.Compare(rectSameA.N_2nd_main_Y_2nd, rectSameB.N_2nd_main_Y_2nd, key, records);
                    StbSecBarColumnRcRectSameNMainTotal.Compare(rectSameA.N_main_total, rectSameB.N_main_total, key, records);
                    StbSecBarColumnRcRectSamePitchBand.Compare(rectSameA.pitch_band, rectSameB.pitch_band, key, records);
                    StbSecBarColumnRcRectSameNBandDirectionX.Compare(rectSameA.N_band_direction_X, rectSameB.N_band_direction_X, key, records);
                    StbSecBarColumnRcRectSameNBandDirectionY.Compare(rectSameA.N_band_direction_Y, rectSameB.N_band_direction_Y, key, records);
                    StbSecBarColumnRcRectSamePitchBarSpacing.Compare(rectSameA.pitch_bar_spacingSpecified,
                        rectSameA.pitch_bar_spacing, rectSameB.pitch_bar_spacingSpecified, rectSameB.pitch_bar_spacing, key,
                        records);
                    StbSecBarColumnRcRectSameNBarSpacingX.Compare(rectSameA.N_bar_spacing_X, rectSameB.N_bar_spacing_X, key, records);
                    StbSecBarColumnRcRectSameNBarSpacingY.Compare(rectSameA.N_bar_spacing_Y, rectSameB.N_bar_spacing_Y, key, records);
                }
                else
                {
                    StbSecBarColumnRcRectSame.Compare(nameof(StbSecBarColumn_RC_RectSame), null, key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarColumn_RC_RectNotSame))
            {
                var set = new HashSet<StbSecBarColumn_RC_RectNotSame>();
                foreach (var rectNotSameA in secA.Items.OfType<StbSecBarColumn_RC_RectNotSame>())
                {
                    var key1 = new List<string>(key) { "pos=" + rectNotSameA.pos };
                    var notSameB = secB.Items.OfType<StbSecBarColumn_RC_RectNotSame>()
                        .FirstOrDefault(n => n.pos == rectNotSameA.pos);
                    if (notSameB != null)
                    {
                        StbSecBarColumnRcRectNotSamePos.Compare(rectNotSameA.pos.ToString(), notSameB.pos.ToString(), key1, records);
                        StbSecBarColumnRcRectNotSameDMain.Compare(rectNotSameA.D_main, notSameB.D_main, key1, records);
                        StbSecBarColumnRcRectNotSameD2ndMain.Compare(rectNotSameA.D_2nd_main, notSameB.D_2nd_main, key1, records);
                        StbSecBarColumnRcRectNotSameDAxial.Compare(rectNotSameA.D_axial, notSameB.D_axial, key1, records);
                        StbSecBarColumnRcRectNotSameDBand.Compare(rectNotSameA.D_band, notSameB.D_band, key1, records);
                        StbSecBarColumnRcRectNotSameDBarSpacing.Compare(rectNotSameA.D_bar_spacing, notSameB.D_bar_spacing, key1, records);
                        StbSecBarColumnRcRectNotSameStrengthMain.Compare(rectNotSameA.strength_main, notSameB.strength_main, key1, records);
                        StbSecBarColumnRcRectNotSameStrength2ndMain.Compare(rectNotSameA.strength_2nd_main, notSameB.strength_2nd_main, key1, records);
                        StbSecBarColumnRcRectNotSameStrengthAxial.Compare(rectNotSameA.strength_axial, notSameB.strength_axial, key1, records);
                        StbSecBarColumnRcRectNotSameStrengthBand.Compare(rectNotSameA.strength_band, notSameB.strength_band, key1, records);
                        StbSecBarColumnRcRectNotSameStrengthBarSpacing.Compare(rectNotSameA.strength_bar_spacing, notSameB.strength_bar_spacing, key1, records);
                        StbSecBarColumnRcRectNotSameNMainX1st.Compare(rectNotSameA.N_main_X_1st, notSameB.N_main_X_1st, key1, records);
                        StbSecBarColumnRcRectNotSameNMainX2nd.Compare(rectNotSameA.N_main_X_2nd, notSameB.N_main_X_2nd, key1, records);
                        StbSecBarColumnRcRectNotSameNMainY1st.Compare(rectNotSameA.N_main_Y_1st, notSameB.N_main_Y_1st, key1, records);
                        StbSecBarColumnRcRectNotSameNMainY2nd.Compare(rectNotSameA.N_main_Y_2nd, notSameB.N_main_Y_2nd, key1, records);
                        StbSecBarColumnRcRectNotSameN2ndMainX1st.Compare(rectNotSameA.N_2nd_main_X_1st, notSameB.N_2nd_main_X_1st, key1, records);
                        StbSecBarColumnRcRectNotSameN2ndMainX2nd.Compare(rectNotSameA.N_2nd_main_X_2nd, notSameB.N_2nd_main_X_2nd, key1, records);
                        StbSecBarColumnRcRectNotSameN2ndMainY1st.Compare(rectNotSameA.N_2nd_main_Y_1st, notSameB.N_2nd_main_Y_1st, key1, records);
                        StbSecBarColumnRcRectNotSameN2ndMainY2nd.Compare(rectNotSameA.N_2nd_main_Y_2nd, notSameB.N_2nd_main_Y_2nd, key1, records);
                        StbSecBarColumnRcRectNotSameNMainTotal.Compare(rectNotSameA.N_main_total, notSameB.N_main_total, key1, records);
                        StbSecBarColumnRcRectNotSamePitchBand.Compare(rectNotSameA.pitch_band, notSameB.pitch_band, key1, records);
                        StbSecBarColumnRcRectNotSameNBandDirectionX.Compare(rectNotSameA.N_band_direction_X, notSameB.N_band_direction_X, key1, records);
                        StbSecBarColumnRcRectNotSameNBandDirectionY.Compare(rectNotSameA.N_band_direction_Y, notSameB.N_band_direction_Y, key1, records);
                        StbSecBarColumnRcRectNotSamePitchBarSpacing.Compare(rectNotSameA.pitch_bar_spacingSpecified,
                            rectNotSameA.pitch_bar_spacing, notSameB.pitch_bar_spacingSpecified, notSameB.pitch_bar_spacing, key1,
                            records);
                        StbSecBarColumnRcRectNotSameNBarSpacingX.Compare(rectNotSameA.N_bar_spacing_X, notSameB.N_bar_spacing_X, key1, records);
                        StbSecBarColumnRcRectNotSameNBarSpacingY.Compare(rectNotSameA.N_bar_spacing_Y, notSameB.N_bar_spacing_Y, key1, records);
                        set.Add(notSameB);
                    }
                    else
                    {
                        StbSecBarColumnRcRectNotSame.Compare(nameof(StbSecBarColumn_RC_RectNotSame), null, key1, records);
                    }
                }

                foreach (var b in secB.Items.OfType<StbSecBarColumn_RC_RectNotSame>())
                {
                    if (!set.Contains(b))
                    {
                        var keyB = new List<string>(key) {"pos=" + b.pos};
                        StbSecBarColumnRcRectNotSame.Compare(nameof(StbSecBarColumn_RC_RectNotSame), null, keyB, records);
                    }
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarColumn_RC_CircleSame))
            {
                var circleSameA = secA.Items.OfType<StbSecBarColumn_RC_CircleSame>().First();
                var circleSameB = secB.Items.OfType<StbSecBarColumn_RC_CircleSame>().FirstOrDefault();
                if (circleSameB != null)
                {
                    StbSecBarColumnRcCircleSameDMain.Compare(circleSameA.D_main, circleSameB.D_main, key, records);
                    StbSecBarColumnRcCircleSameDAxial.Compare(circleSameA.D_axial, circleSameB.D_axial, key, records);
                    StbSecBarColumnRcCircleSameDBand.Compare(circleSameA.D_band, circleSameB.D_band, key, records);
                    StbSecBarColumnRcCircleSameDBarSpacing.Compare(circleSameA.D_bar_spacing, circleSameB.D_bar_spacing, key, records);
                    StbSecBarColumnRcCircleSameStrengthMain.Compare(circleSameA.strength_main, circleSameB.strength_main, key, records);
                    StbSecBarColumnRcCircleSameStrengthAxial.Compare(circleSameA.strength_axial, circleSameB.strength_axial, key, records);
                    StbSecBarColumnRcCircleSameStrengthBand.Compare(circleSameA.strength_band, circleSameB.strength_band, key, records);
                    StbSecBarColumnRcCircleSameStrengthBarSpacing.Compare(circleSameA.strength_bar_spacing, circleSameB.strength_bar_spacing, key, records);
                    StbSecBarColumnRcCircleSameNMain.Compare(circleSameA.N_main, circleSameB.N_main, key, records);
                    StbSecBarColumnRcCircleSameNAxial.Compare(circleSameA.N_axial, circleSameB.N_axial, key, records);
                    StbSecBarColumnRcCircleSameNBand.Compare(circleSameA.N_band, circleSameB.N_band, key, records);
                    StbSecBarColumnRcCircleSamePitchBand.Compare(circleSameA.pitch_band, circleSameB.pitch_band, key, records);
                    StbSecBarColumnRcCircleSamePitchBarSpacing.Compare(circleSameA.pitch_bar_spacingSpecified,
                        circleSameA.pitch_bar_spacing, circleSameB.pitch_bar_spacingSpecified,
                        circleSameB.pitch_bar_spacing, key, records);
                    StbSecBarColumnRcCircleSameNBarSpacingX.Compare(circleSameA.N_bar_spacing_X, circleSameB.N_bar_spacing_X, key, records);
                    StbSecBarColumnRcCircleSameNBarSpacingY.Compare(circleSameA.N_bar_spacing_Y, circleSameB.N_bar_spacing_Y, key, records);
                }
                else
                {
                    StbSecBarColumnRcCircleSame.Compare(nameof(StbSecBarColumn_RC_CircleSame), null, key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarColumn_RC_CircleNotSame))
            {
                var set = new HashSet<StbSecBarColumn_RC_CircleNotSame>();
                foreach (var circleNotSameA in secA.Items.OfType<StbSecBarColumn_RC_CircleNotSame>())
                {
                    var key1 = new List<string>(key) { "pos=" + circleNotSameA.pos };
                    var circleNotSameB = secB.Items.OfType<StbSecBarColumn_RC_CircleNotSame>()
                        .FirstOrDefault(n => n.pos == circleNotSameA.pos);
                    if (circleNotSameB != null)
                    {
                        StbSecBarColumnRcCircleNotSamePos.Compare(circleNotSameA.pos.ToString(), circleNotSameB.pos.ToString(), key1, records);
                        StbSecBarColumnRcCircleNotSameDMain.Compare(circleNotSameA.D_main, circleNotSameB.D_main, key1, records);
                        StbSecBarColumnRcCircleNotSameDAxial.Compare(circleNotSameA.D_axial, circleNotSameB.D_axial, key1, records);
                        StbSecBarColumnRcCircleNotSameDBand.Compare(circleNotSameA.D_band, circleNotSameB.D_band, key1, records);
                        StbSecBarColumnRcCircleNotSameDBarSpacing.Compare(circleNotSameA.D_bar_spacing, circleNotSameB.D_bar_spacing, key1, records);
                        StbSecBarColumnRcCircleNotSameStrengthMain.Compare(circleNotSameA.strength_main, circleNotSameB.strength_main, key1, records);
                        StbSecBarColumnRcCircleNotSameStrengthAxial.Compare(circleNotSameA.strength_axial, circleNotSameB.strength_axial, key1, records);
                        StbSecBarColumnRcCircleNotSameStrengthBand.Compare(circleNotSameA.strength_band, circleNotSameB.strength_band, key1, records);
                        StbSecBarColumnRcCircleNotSameStrengthBarSpacing.Compare(circleNotSameA.strength_bar_spacing, circleNotSameB.strength_bar_spacing, key1, records);
                        StbSecBarColumnRcCircleNotSameNMain.Compare(circleNotSameA.N_main, circleNotSameB.N_main, key1, records);
                        StbSecBarColumnRcCircleNotSameNAxial.Compare(circleNotSameA.N_axial, circleNotSameB.N_axial, key1, records);
                        StbSecBarColumnRcCircleNotSameNBand.Compare(circleNotSameA.N_band, circleNotSameB.N_band, key1, records);
                        StbSecBarColumnRcCircleNotSamePitchBand.Compare(circleNotSameA.pitch_band, circleNotSameB.pitch_band, key1, records);
                        StbSecBarColumnRcCircleNotSamePitchBarSpacing.Compare(circleNotSameA.pitch_bar_spacingSpecified,
                            circleNotSameA.pitch_bar_spacing, circleNotSameB.pitch_bar_spacingSpecified,
                            circleNotSameB.pitch_bar_spacing, key1, records);
                        StbSecBarColumnRcCircleNotSameNBarSpacingX.Compare(circleNotSameA.N_bar_spacing_X, circleNotSameB.N_bar_spacing_X, key1, records);
                        StbSecBarColumnRcCircleNotSameNBarSpacingY.Compare(circleNotSameA.N_bar_spacing_Y, circleNotSameB.N_bar_spacing_Y, key1, records);
                        set.Add(circleNotSameB);
                    }
                    else
                    {
                        StbSecBarColumnRcCircleNotSame.Compare(nameof(StbSecBarColumn_RC_CircleNotSame), null, key1, records);
                    }
                }

                foreach (var b in secB.Items.OfType<StbSecBarColumn_RC_CircleNotSame>())
                {
                    if (!set.Contains(b))
                    {
                        var keyB = new List<string>(key) { "pos=" + b.pos };
                        StbSecBarColumnRcCircleNotSame.Compare(nameof(StbSecBarColumn_RC_CircleNotSame), null, keyB, records);
                    }
                }
            }

            if (secA.Items.Any(n => n is StbSecBarColumnXReinforced))
            {
                var xReinforcedA = secA.Items.OfType<StbSecBarColumnXReinforced>().First();
                var xReinforcedB = secB.Items.OfType<StbSecBarColumnXReinforced>().FirstOrDefault();
                if (xReinforcedB != null)
                {
                    StbSecBarColumnXReinforcedNMainX.Compare(xReinforcedA.N_main_X, xReinforcedB.N_main_X, key, records);
                    StbSecBarColumnXReinforcedNMainY.Compare(xReinforcedA.N_main_Y, xReinforcedB.N_main_Y, key, records);
                    StbSecBarColumnXReinforcedNMainTotal.Compare(xReinforcedA.N_main_total, xReinforcedB.N_main_total, key, records);
                }
                else
                {
                    CheckObjects.StbSecBarColumnXReinforced.Compare(nameof(STBridge201.StbSecBarColumnXReinforced), null, key, records);
                }
            }


        }

        private static void CompareSecFigureColumnRc(StbSecColumn_RC secColumnA, StbSecColumn_RC secColumnB, IReadOnlyList<string> key, List<Record> records)
        {
            if (secColumnA.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect rectA)
            {
                if (secColumnB.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect rectB)
                {
                    StbSecColumnRcRectWidthX.Compare(rectA.width_X, rectB.width_X, key, records);
                    StbSecColumnRcRectWidthY.Compare(rectA.width_Y, rectB.width_Y, key, records);
                }
                else
                {
                    StbSecColumnRcRect.Compare(nameof(StbSecColumn_RC_Rect), nameof(StbSecColumn_RC_Circle), key, records);
                }
            }
            else if (secColumnA.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Circle circleA)
            {
                if (secColumnB.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Circle circleB)
                {
                    StbSecColumnRcCircleD.Compare(circleA.D, circleB.D, key, records);
                }
                else
                {
                    StbSecColumnRcCircle.Compare(nameof(StbSecColumn_RC_Circle), nameof(StbSecColumn_RC_Rect), key, records);
                }
            }
            else
                throw new Exception();
        }
    }
}

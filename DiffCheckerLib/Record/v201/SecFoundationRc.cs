using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecFoundationRc
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stbridgeA?.StbModel?.StbSections?.StbSecFoundation_RC;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecFoundation_RC;
            var setB = secB != null ? new HashSet<StbSecFoundation_RC>(secB) : new HashSet<StbSecFoundation_RC>();

            if (secA != null)
            {
                foreach (var secFoundationA in secA)
                {
                    var key = new List<string> { "Name=" + secFoundationA.name};
                    var secFoundationB = secB?.FirstOrDefault(n => n.name == secFoundationA.name);
                    if (secFoundationB != null)
                    {
                        CompareSecFoundationRc(secFoundationA, secFoundationB, key, records);
                        setB.Remove(secFoundationB);
                    }
                    else
                    {
                        StbSecFoundationRc.Compare(nameof(StbSecFoundation_RC), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { "Name=" + b.name};
                StbSecFoundationRc.Compare(null, nameof(StbSecFoundation_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecFoundationRc(StbSecFoundation_RC secFoundationA, StbSecFoundation_RC secFoundationB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecFoundationRcId.Compare(secFoundationA.id, secFoundationB.id, key, records);
            StbSecFoundationRcGuid.Compare(secFoundationA.guid, secFoundationB.guid, key, records);
            StbSecFoundationRcName.Compare(secFoundationA.name, secFoundationB.name, key, records);
            StbSecFoundationRcStrengthConcrete.Compare(secFoundationA.strength_concrete, secFoundationB.strength_concrete, key, records);

            CompareStbSecFigureFoundationRc(secFoundationA.StbSecFigureFoundation_RC, secFoundationB.StbSecFigureFoundation_RC, key, records);

            if (secFoundationA.StbSecBarArrangementFoundation_RC != null || secFoundationB.StbSecBarArrangementFoundation_RC != null)
            {
                if (secFoundationB.StbSecBarArrangementFoundation_RC == null)
                {
                    StbSecBarArrangementFoundationRc.Compare(nameof(StbSecBarArrangementFoundation_RC), null, key, records);
                }
                else if (secFoundationA.StbSecBarArrangementFoundation_RC == null)
                {
                    StbSecBarArrangementFoundationRc.Compare(null, nameof(StbSecBarArrangementFoundation_RC), key, records);
                }
                else
                {
                    CompareStbSecBarArrangementFoundationRc(secFoundationA.StbSecBarArrangementFoundation_RC,
                        secFoundationB.StbSecBarArrangementFoundation_RC, key, records);
                }
            }

        }

        private static void CompareStbSecBarArrangementFoundationRc(StbSecBarArrangementFoundation_RC secA, StbSecBarArrangementFoundation_RC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementFoundationRcDepthCoverTop.Compare(secA.depth_cover_topSpecified, secA.depth_cover_top,
                secB.depth_cover_topSpecified, secB.depth_cover_top, key, records);
            StbSecBarArrangementFoundationRcDepthCoverBottom.Compare(secA.depth_cover_bottomSpecified, secA.depth_cover_bottom,
                secB.depth_cover_bottomSpecified, secB.depth_cover_bottom, key, records);
            StbSecBarArrangementFoundationRcDepthCoverSide.Compare(secA.depth_cover_sideSpecified, secA.depth_cover_side,
                secB.depth_cover_sideSpecified, secB.depth_cover_side, key, records);

            if (secA.Items.Any(n => n is StbSecBarFoundation_RC_Rect))
            {
                if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Rect))
                {
                    var set = new HashSet<StbSecBarFoundation_RC_Rect>();
                    foreach (var a in secA.Items.OfType<StbSecBarFoundation_RC_Rect>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarFoundation_RC_Rect>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarFoundationRcRectPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarFoundationRcRectStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarFoundationRcRectD.Compare(a.D, b.D, key1, records);
                            StbSecBarFoundationRcRectN.Compare(a.N, b.N, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarFoundation_RC_Rect>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarFoundationRcRect.Compare(null, nameof(StbSecBarFoundation_RC_Rect), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Triangle))
                {
                    StbSecBarFoundationRcRect.Compare(nameof(StbSecBarFoundation_RC_Rect), nameof(StbSecBarFoundation_RC_Triangle), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_ThreeWay))
                {
                    StbSecBarFoundationRcRect.Compare(nameof(StbSecBarFoundation_RC_Rect), nameof(StbSecBarFoundation_RC_ThreeWay), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Continuous))
                {
                    StbSecBarFoundationRcRect.Compare(nameof(StbSecBarFoundation_RC_Rect), nameof(StbSecBarFoundation_RC_Continuous), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarFoundation_RC_Triangle))
            {
                if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Rect))
                {
                    StbSecBarFoundationRcTriangle.Compare(nameof(StbSecBarFoundation_RC_Triangle), nameof(StbSecBarFoundation_RC_Rect), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Triangle))
                {
                    var set = new HashSet<StbSecBarFoundation_RC_Triangle>();
                    foreach (var a in secA.Items.OfType<StbSecBarFoundation_RC_Triangle>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarFoundation_RC_Triangle>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarFoundationRcTrianglePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarFoundationRcTriangleStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarFoundationRcTriangleD.Compare(a.D, b.D, key1, records);
                            StbSecBarFoundationRcTriangleN.Compare(a.N, b.N, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarFoundation_RC_Triangle>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarFoundationRcTriangle.Compare(null, nameof(StbSecBarFoundation_RC_Triangle), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_ThreeWay))
                {
                    StbSecBarFoundationRcTriangle.Compare(nameof(StbSecBarFoundation_RC_Triangle), nameof(StbSecBarFoundation_RC_ThreeWay), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Continuous))
                {
                    StbSecBarFoundationRcTriangle.Compare(nameof(StbSecBarFoundation_RC_Triangle), nameof(StbSecBarFoundation_RC_Continuous), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarFoundation_RC_ThreeWay))
            {
                if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Rect))
                {
                    StbSecBarFoundationRcThreeWay.Compare(nameof(StbSecBarFoundation_RC_ThreeWay), nameof(StbSecBarFoundation_RC_Rect), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Triangle))
                {
                    StbSecBarFoundationRcThreeWay.Compare(nameof(StbSecBarFoundation_RC_ThreeWay), nameof(StbSecBarFoundation_RC_Triangle), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_ThreeWay))
                {
                    var set = new HashSet<StbSecBarFoundation_RC_ThreeWay>();
                    foreach (var a in secA.Items.OfType<StbSecBarFoundation_RC_ThreeWay>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarFoundation_RC_ThreeWay>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarFoundationRcThreeWayPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarFoundationRcThreeWayStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarFoundationRcThreeWayD.Compare(a.D, b.D, key1, records);
                            StbSecBarFoundationRcThreeWayN.Compare(a.N, b.N, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarFoundation_RC_ThreeWay>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarFoundationRcThreeWay.Compare(null, nameof(StbSecBarFoundation_RC_ThreeWay), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Continuous))
                {
                    StbSecBarFoundationRcThreeWay.Compare(nameof(StbSecBarFoundation_RC_ThreeWay), nameof(StbSecBarFoundation_RC_Continuous), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarFoundation_RC_Continuous))
            {
                if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Rect))
                {
                    StbSecBarFoundationRcContinuous.Compare(nameof(StbSecBarFoundation_RC_Continuous), nameof(StbSecBarFoundation_RC_Rect), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Triangle))
                {
                    StbSecBarFoundationRcContinuous.Compare(nameof(StbSecBarFoundation_RC_Continuous), nameof(StbSecBarFoundation_RC_Triangle), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_ThreeWay))
                {
                    StbSecBarFoundationRcContinuous.Compare(nameof(StbSecBarFoundation_RC_Continuous), nameof(StbSecBarFoundation_RC_ThreeWay), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarFoundation_RC_Continuous))
                {
                    var set = new HashSet<StbSecBarFoundation_RC_Continuous>();
                    foreach (var a in secA.Items.OfType<StbSecBarFoundation_RC_Continuous>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarFoundation_RC_Continuous>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarFoundationRcContinuousPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarFoundationRcContinuousStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarFoundationRcContinuousD.Compare(a.D, b.D, key1, records);
                            StbSecBarFoundationRcContinuousN.Compare(a.N, b.N, key1, records);
                            StbSecBarFoundationRcContinuousPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarFoundation_RC_Continuous>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarFoundationRcContinuous.Compare(null, nameof(StbSecBarFoundation_RC_Continuous), keyB, records);
                        }
                    }
                }
            }

        }

        private static void CompareStbSecFigureFoundationRc(StbSecFigureFoundation_RC secA, StbSecFigureFoundation_RC secB,
            IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Item is StbSecFoundation_RC_Rect rectA)
            {
                if (secB.Item is StbSecFoundation_RC_Rect rectB)
                {
                    StbSecFoundationRcRectWidthX.Compare(rectA.width_X, rectB.width_X, key, records);
                    StbSecFoundationRcRectWidthY.Compare(rectA.width_Y, rectB.width_Y, key, records);
                    StbSecFoundationRcRectDepth.Compare(rectA.depth, rectB.depth, key, records);
                }
                else if (secB.Item is StbSecFoundation_RC_TaperedRect)
                {
                    StbSecFoundationRcRect.Compare(nameof(StbSecFoundation_RC_Rect), nameof(StbSecFoundation_RC_TaperedRect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Triangle)
                {
                    StbSecFoundationRcRect.Compare(nameof(StbSecFoundation_RC_Rect), nameof(StbSecFoundation_RC_Triangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_EquiTriangle)
                {
                    StbSecFoundationRcRect.Compare(nameof(StbSecFoundation_RC_Rect), nameof(StbSecFoundation_RC_EquiTriangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Octagon)
                {
                    StbSecFoundationRcRect.Compare(nameof(StbSecFoundation_RC_Rect), nameof(StbSecFoundation_RC_Octagon), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Continuous)
                {
                    StbSecFoundationRcRect.Compare(nameof(StbSecFoundation_RC_Rect), nameof(StbSecFoundation_RC_Continuous), key,
                        records);
                }
            }
            else if (secA.Item is StbSecFoundation_RC_TaperedRect taperedRectA)
            {
                if (secB.Item is StbSecFoundation_RC_Rect)
                {
                    StbSecFoundationRcTaperedRect.Compare(nameof(StbSecFoundation_RC_TaperedRect), nameof(StbSecFoundation_RC_Rect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_TaperedRect taperedRectB)
                {
                    StbSecFoundationRcTaperedRectWidthX.Compare(taperedRectA.width_X, taperedRectB.width_X, key, records);
                    StbSecFoundationRcTaperedRectWidthY.Compare(taperedRectA.width_Y, taperedRectB.width_Y, key, records);
                    StbSecFoundationRcTaperedRectDepthBase.Compare(taperedRectA.depth_base, taperedRectB.depth_base, key, records);
                    StbSecFoundationRcTaperedRectDepthTip.Compare(taperedRectA.depth_tip, taperedRectB.depth_tip, key, records);
                }
                else if (secB.Item is StbSecFoundation_RC_Triangle)
                {
                    StbSecFoundationRcTaperedRect.Compare(nameof(StbSecFoundation_RC_TaperedRect), nameof(StbSecFoundation_RC_Triangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_EquiTriangle)
                {
                    StbSecFoundationRcTaperedRect.Compare(nameof(StbSecFoundation_RC_TaperedRect), nameof(StbSecFoundation_RC_EquiTriangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Octagon)
                {
                    StbSecFoundationRcTaperedRect.Compare(nameof(StbSecFoundation_RC_TaperedRect), nameof(StbSecFoundation_RC_Octagon), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Continuous)
                {
                    StbSecFoundationRcTaperedRect.Compare(nameof(StbSecFoundation_RC_TaperedRect), nameof(StbSecFoundation_RC_Continuous), key,
                        records);
                }
            }
            if (secA.Item is StbSecFoundation_RC_Triangle triangleA)
            {
                if (secB.Item is StbSecFoundation_RC_Rect)
                {
                    StbSecFoundationRcTriangle.Compare(nameof(StbSecFoundation_RC_Triangle), nameof(StbSecFoundation_RC_Rect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_TaperedRect)
                {
                    StbSecFoundationRcTriangle.Compare(nameof(StbSecFoundation_RC_Triangle), nameof(StbSecFoundation_RC_TaperedRect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Triangle triangleB)
                {
                    StbSecFoundationRcTriangleWidthX.Compare(triangleA.width_X, triangleB.width_X, key, records);
                    StbSecFoundationRcTriangleWidthY.Compare(triangleA.width_Y, triangleB.width_Y, key, records);
                    StbSecFoundationRcTriangleDepth.Compare(triangleA.depth, triangleB.depth, key, records);
                }
                else if (secB.Item is StbSecFoundation_RC_EquiTriangle)
                {
                    StbSecFoundationRcTriangle.Compare(nameof(StbSecFoundation_RC_Triangle), nameof(StbSecFoundation_RC_EquiTriangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Octagon)
                {
                    StbSecFoundationRcTriangle.Compare(nameof(StbSecFoundation_RC_Triangle), nameof(StbSecFoundation_RC_Octagon), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Continuous)
                {
                    StbSecFoundationRcTriangle.Compare(nameof(StbSecFoundation_RC_Triangle), nameof(StbSecFoundation_RC_Continuous), key,
                        records);
                }
            }
            if (secA.Item is StbSecFoundation_RC_EquiTriangle equiTriangleA)
            {
                if (secB.Item is StbSecFoundation_RC_Rect)
                {
                    StbSecFoundationRcEquiTriangle.Compare(nameof(StbSecFoundation_RC_EquiTriangle), nameof(StbSecFoundation_RC_Rect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_TaperedRect)
                {
                    StbSecFoundationRcEquiTriangle.Compare(nameof(StbSecFoundation_RC_EquiTriangle), nameof(StbSecFoundation_RC_TaperedRect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Triangle)
                {
                    StbSecFoundationRcEquiTriangle.Compare(nameof(StbSecFoundation_RC_EquiTriangle), nameof(StbSecFoundation_RC_Triangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_EquiTriangle equiTriangleB)
                {
                    StbSecFoundationRcEquiTriangleWidthBase.Compare(equiTriangleA.width_base, equiTriangleB.width_base, key, records);
                    StbSecFoundationRcEquiTriangleWidthChamfer.Compare(equiTriangleA.width_chamfer, equiTriangleB.width_chamfer, key, records);
                    StbSecFoundationRcEquiTriangleDepth.Compare(equiTriangleA.depth, equiTriangleB.depth, key, records);
                }
                else if (secB.Item is StbSecFoundation_RC_Octagon)
                {
                    StbSecFoundationRcEquiTriangle.Compare(nameof(StbSecFoundation_RC_EquiTriangle), nameof(StbSecFoundation_RC_Octagon), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Continuous)
                {
                    StbSecFoundationRcEquiTriangle.Compare(nameof(StbSecFoundation_RC_EquiTriangle), nameof(StbSecFoundation_RC_Continuous), key,
                        records);
                }
            }
            if (secA.Item is StbSecFoundation_RC_Octagon octagonA)
            {
                if (secB.Item is StbSecFoundation_RC_Rect)
                {
                    StbSecFoundationRcOctagon.Compare(nameof(StbSecFoundation_RC_Octagon), nameof(StbSecFoundation_RC_Rect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_TaperedRect)
                {
                    StbSecFoundationRcOctagon.Compare(nameof(StbSecFoundation_RC_Octagon), nameof(StbSecFoundation_RC_TaperedRect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Triangle)
                {
                    StbSecFoundationRcOctagon.Compare(nameof(StbSecFoundation_RC_Octagon), nameof(StbSecFoundation_RC_Triangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_EquiTriangle)
                {
                    StbSecFoundationRcOctagon.Compare(nameof(StbSecFoundation_RC_Octagon), nameof(StbSecFoundation_RC_EquiTriangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Octagon octagonB)
                {
                    StbSecFoundationRcOctagonWidthX.Compare(octagonA.width_X, octagonB.width_X, key, records);
                    StbSecFoundationRcOctagonWidthY.Compare(octagonA.width_Y, octagonB.width_Y, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonWidthChamfer1X.Compare(octagonA.width_chamfer1_X, octagonB.width_chamfer1_X, key, records);
                    StbSecFoundationRcOctagonDepth.Compare(octagonA.depth, octagonB.depth, key, records);
                }
                else if (secB.Item is StbSecFoundation_RC_Continuous)
                {
                    StbSecFoundationRcOctagon.Compare(nameof(StbSecFoundation_RC_Octagon), nameof(StbSecFoundation_RC_Continuous), key,
                        records);
                }
            }
            if (secA.Item is StbSecFoundation_RC_Continuous continuousA)
            {
                if (secB.Item is StbSecFoundation_RC_Rect)
                {
                    StbSecFoundationRcContinuous.Compare(nameof(StbSecFoundation_RC_Continuous), nameof(StbSecFoundation_RC_Rect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_TaperedRect)
                {
                    StbSecFoundationRcContinuous.Compare(nameof(StbSecFoundation_RC_Continuous), nameof(StbSecFoundation_RC_TaperedRect), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Triangle)
                {
                    StbSecFoundationRcContinuous.Compare(nameof(StbSecFoundation_RC_Continuous), nameof(StbSecFoundation_RC_Triangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_EquiTriangle)
                {
                    StbSecFoundationRcContinuous.Compare(nameof(StbSecFoundation_RC_Continuous), nameof(StbSecFoundation_RC_EquiTriangle), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Octagon)
                {
                    StbSecFoundationRcContinuous.Compare(nameof(StbSecFoundation_RC_Continuous), nameof(StbSecFoundation_RC_Octagon), key,
                        records);
                }
                else if (secB.Item is StbSecFoundation_RC_Continuous continuousB)
                {
                    StbSecFoundationRcContinuousWidth.Compare(continuousA.width, continuousB.width, key, records);
                    StbSecFoundationRcContinuousDepthBase.Compare(continuousA.depth_base, continuousB.depth_base, key, records);
                    StbSecFoundationRcContinuousDepthTip.Compare(continuousA.depth_tip, continuousB.depth_tip, key, records);
                    StbSecFoundationRcContinuousType.Compare(continuousA.type.ToString(), continuousB.type.ToString(), key, records);
                }
            }

        }
    }
}

using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    static class SecParapetRc
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stBridgeA?.StbModel?.StbSections?.StbSecParapet_RC;
            var tempB = stBridgeB?.StbModel?.StbSections?.StbSecParapet_RC;
            var secB = tempB != null ? new HashSet<StbSecParapet_RC>(tempB) : new HashSet<StbSecParapet_RC>();

            if (secA != null)
            {
                foreach (var secParapetA in secA)
                {
                    var key = new List<string>() { "Name=" + secParapetA.name };
                    var secParapetB = tempB?.FirstOrDefault(n => n.name == secParapetA.name);
                    if (secParapetB != null)
                    {
                        CompareSecParapetRc(secParapetA, secParapetB, key, records);
                        secB.Remove(secParapetA);
                    }
                    else
                    {
                        StbSecParapetRc.Compare(nameof(StbSecParapet_RC), null, key, records);
                    }
                }
            }

            foreach (var b in secB)
            {
                var key = new List<string> { "Name=" + b.name };
                StbSecParapetRc.Compare(null, nameof(StbSecParapet_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecParapetRc(StbSecParapet_RC secParapetA, StbSecParapet_RC secParapetB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecParapetRcId.Compare(secParapetA.id, secParapetB.id, key, records);
            StbSecParapetRcGuid.Compare(secParapetA.guid, secParapetB.guid, key, records);
            StbSecParapetRcName.Compare(secParapetA.name, secParapetB.name, key, records);
            StbSecParapetRcStrengthConcrete.Compare(secParapetA.strength_concrete, secParapetB.strength_concrete, key, records);

            StbSecFigureParapetRcCompare(secParapetA.StbSecFigureParapet_RC, secParapetB.StbSecFigureParapet_RC, key,
                records);

            if (secParapetA.StbSecBarArrangementParapet_RC != null || secParapetB.StbSecBarArrangementParapet_RC != null)
            {
                if (secParapetB.StbSecBarArrangementParapet_RC == null)
                {
                    StbSecBarArrangementParapetRc.Compare(nameof(StbSecBarArrangementParapet_RC), null, key, records);
                }
                else if (secParapetA.StbSecBarArrangementParapet_RC == null)
                {
                    StbSecBarArrangementParapetRc.Compare(null, nameof(StbSecBarArrangementParapet_RC), key, records);
                }
                else
                {
                    CompareSecBarArrangementParapetRc(secParapetA.StbSecBarArrangementParapet_RC,
                        secParapetB.StbSecBarArrangementParapet_RC, key, records);
                }
            }
        }

        private static void StbSecFigureParapetRcCompare(StbSecFigureParapet_RC secA, StbSecFigureParapet_RC secB, IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Item is StbSecParapet_RC_TypeL typeLA)
            {
                if (secB.Item is StbSecParapet_RC_TypeL typeLB)
                {
                    StbSecParapetRcTypeLTT.Compare(typeLA.t_T, typeLB.t_T, key, records);
                    StbSecParapetRcTypeLDepthH.Compare(typeLA.depth_H, typeLB.depth_H, key, records);
                    StbSecParapetRcTypeLTT1.Compare(typeLA.t_T1, typeLB.t_T1, key, records);
                    StbSecParapetRcTypeLDepthH1.Compare(typeLA.depth_H1, typeLB.depth_H1, key, records);
                    StbSecParapetRcTypeLDepthH2.Compare(typeLA.depth_H2, typeLB.depth_H2, key, records);
                }
                else if (secB.Item is StbSecParapet_RC_TypeT)
                {
                    StbSecParapetRcTypeL.Compare(nameof(StbSecParapet_RC_TypeL), nameof(StbSecParapet_RC_TypeT), key, records);
                }
                else if (secB.Item is StbSecParapet_RC_TypeI)
                {
                    StbSecParapetRcTypeL.Compare(nameof(StbSecParapet_RC_TypeL), nameof(StbSecParapet_RC_TypeI), key, records);
                }
            }
            else if (secA.Item is StbSecParapet_RC_TypeT typeTA)
            {
                if (secB.Item is StbSecParapet_RC_TypeL )
                {
                    StbSecParapetRcTypeT.Compare(nameof(StbSecParapet_RC_TypeT), nameof(StbSecParapet_RC_TypeL), key, records);
                }
                else if (secB.Item is StbSecParapet_RC_TypeT typeTB)
                {
                    StbSecParapetRcTypeTTT.Compare(typeTA.t_T, typeTB.t_T, key, records);
                    StbSecParapetRcTypeTDepthH.Compare(typeTA.depth_H, typeTB.depth_H, key, records);
                    StbSecParapetRcTypeTTT1.Compare(typeTA.t_T1, typeTB.t_T1, key, records);
                    StbSecParapetRcTypeTDepthH1.Compare(typeTA.depth_H1, typeTB.depth_H1, key, records);
                    StbSecParapetRcTypeTDepthH2.Compare(typeTA.depth_H2, typeTB.depth_H2, key, records);
                    StbSecParapetRcTypeTDepthH3.Compare(typeTA.depth_H3, typeTB.depth_H3, key, records);
                }
                else if (secB.Item is StbSecParapet_RC_TypeI)
                {
                    StbSecParapetRcTypeT.Compare(nameof(StbSecParapet_RC_TypeT), nameof(StbSecParapet_RC_TypeI), key, records);
                }
            }
            else if (secA.Item is StbSecParapet_RC_TypeI typeIA)
            {
                if (secB.Item is StbSecParapet_RC_TypeL)
                {
                    StbSecParapetRcTypeI.Compare(nameof(StbSecParapet_RC_TypeI), nameof(StbSecParapet_RC_TypeL), key, records);
                }
                else if (secB.Item is StbSecParapet_RC_TypeT)
                {
                    StbSecParapetRcTypeI.Compare(nameof(StbSecParapet_RC_TypeI), nameof(StbSecParapet_RC_TypeT), key, records);
                }
                else if (secB.Item is StbSecParapet_RC_TypeI typeIB)
                {
                    StbSecParapetRcTypeITT.Compare(typeIA.t_T, typeIB.t_T, key, records);
                    StbSecParapetRcTypeIDepthH.Compare(typeIA.depth_H, typeIB.depth_H, key, records);
                }
            }
        }

        private static void CompareSecBarArrangementParapetRc(StbSecBarArrangementParapet_RC wallA, StbSecBarArrangementParapet_RC wallB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementParapetRcDepthCoverOutside.Compare(wallA.depth_cover_outsideSpecified,
                wallA.depth_cover_outside, wallB.depth_cover_outsideSpecified, wallB.depth_cover_outside, key, records);
            StbSecBarArrangementParapetRcDepthCoverInside.Compare(wallA.depth_cover_insideSpecified,
                wallA.depth_cover_inside, wallB.depth_cover_insideSpecified, wallB.depth_cover_inside, key, records);

            if (wallA.Items.Any(n => n is StbSecBarParapet_RC_Single))
            {
                if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Single))
                {
                    var set = new HashSet<StbSecBarParapet_RC_Single>();
                    foreach (var a in wallA.Items.OfType<StbSecBarParapet_RC_Single>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = wallB.Items.OfType<StbSecBarParapet_RC_Single>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarParapetRcSinglePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarParapetRcSingleStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarParapetRcSingleD.Compare(a.D, b.D, key1, records);
                            StbSecBarParapetRcSinglePitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarParapetRcSingle.Compare(nameof(StbSecBarParapet_RC_Single), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarParapet_RC_Single>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarParapetRcSingle.Compare(null, nameof(StbSecBarParapet_RC_Single), key1, records);
                        }
                    }
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Zigzag))
                {
                    StbSecBarParapetRcSingle.Compare(nameof(StbSecBarParapet_RC_Single), nameof(StbSecBarParapet_RC_Zigzag), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_DoubleNet))
                {
                    StbSecBarParapetRcSingle.Compare(nameof(StbSecBarParapet_RC_Single), nameof(StbSecBarParapet_RC_DoubleNet), key, records);
                }
            }
            else if (wallA.Items.Any(n => n is StbSecBarParapet_RC_Zigzag))
            {
                if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Single))
                {
                    StbSecBarParapetRcZigzag.Compare(nameof(StbSecBarParapet_RC_Zigzag), nameof(StbSecBarParapet_RC_Single), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Zigzag))
                {
                    var set = new HashSet<StbSecBarParapet_RC_Zigzag>();
                    foreach (var a in wallA.Items.OfType<StbSecBarParapet_RC_Zigzag>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = wallB.Items.OfType<StbSecBarParapet_RC_Zigzag>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarParapetRcZigzagPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarParapetRcZigzagStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarParapetRcZigzagD.Compare(a.D, b.D, key1, records);
                            StbSecBarParapetRcZigzagPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarParapetRcZigzag.Compare(nameof(StbSecBarParapet_RC_Zigzag), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarParapet_RC_Zigzag>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarParapetRcZigzag.Compare(null, nameof(StbSecBarParapet_RC_Zigzag), key1, records);
                        }
                    }
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_DoubleNet))
                {
                    StbSecBarParapetRcZigzag.Compare(nameof(StbSecBarParapet_RC_Zigzag), nameof(StbSecBarParapet_RC_DoubleNet), key, records);
                }
            }
            else if (wallA.Items.Any(n => n is StbSecBarParapet_RC_DoubleNet))
            {
                if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Single))
                {
                    StbSecBarParapetRcDoubleNet.Compare(nameof(StbSecBarParapet_RC_DoubleNet),
                        nameof(StbSecBarParapet_RC_Single), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Zigzag))
                {
                    StbSecBarParapetRcDoubleNet.Compare(nameof(StbSecBarParapet_RC_DoubleNet),
                        nameof(StbSecBarParapet_RC_Zigzag), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_DoubleNet))
                {
                    var set = new HashSet<StbSecBarParapet_RC_DoubleNet>();
                    foreach (var a in wallA.Items.OfType<StbSecBarParapet_RC_DoubleNet>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = wallB.Items.OfType<StbSecBarParapet_RC_DoubleNet>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarParapetRcDoubleNetPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarParapetRcDoubleNetStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarParapetRcDoubleNetD.Compare(a.D, b.D, key1, records);
                            StbSecBarParapetRcDoubleNetPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarParapetRcDoubleNet.Compare(nameof(StbSecBarParapet_RC_DoubleNet), null, key1,
                                records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarParapet_RC_DoubleNet>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarParapetRcDoubleNet.Compare(null, nameof(StbSecBarParapet_RC_DoubleNet), key1,
                                records);
                        }
                    }
                }
            }

            if (wallA.Items.Any(n => n is StbSecBarParapet_RC_Tip))
            {
                if (!wallB.Items.Any(n => n is StbSecBarParapet_RC_Tip))
                {
                    StbSecBarParapetRcTip.Compare(nameof(StbSecBarParapet_RC_Tip), null, key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Tip))
                {
                    var set = new HashSet<StbSecBarParapet_RC_Tip>();
                    foreach (var a in wallA.Items.OfType<StbSecBarParapet_RC_Tip>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = wallB.Items.OfType<StbSecBarParapet_RC_Tip>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarParapetRcTipPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarParapetRcTipStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarParapetRcTipD.Compare(a.D, b.D, key1, records);
                            StbSecBarParapetRcTipN.Compare(a.N, b.N, key1, records);
                            StbSecBarParapetRcTipPitch.Compare(a.pitchSpecified, a.pitch, b.pitchSpecified, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarParapetRcTip.Compare(nameof(StbSecBarParapet_RC_Tip), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarParapet_RC_Tip>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarParapetRcTip.Compare(null, nameof(StbSecBarParapet_RC_Tip), key1, records);
                        }
                    }
                }
            }
            else
            {
                if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Tip))
                {
                    StbSecBarParapetRcTip.Compare(null, nameof(StbSecBarParapet_RC_Tip), key, records);
                }
            }

            if (wallA.Items.Any(n => n is StbSecBarParapet_RC_Edge))
            {
                if (!wallB.Items.Any(n => n is StbSecBarParapet_RC_Edge))
                {
                    StbSecBarParapetRcEdge.Compare(nameof(StbSecBarParapet_RC_Edge), null, key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Edge))
                {
                    var set = new HashSet<StbSecBarParapet_RC_Edge>();
                    foreach (var a in wallA.Items.OfType<StbSecBarParapet_RC_Edge>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = wallB.Items.OfType<StbSecBarParapet_RC_Edge>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarParapetRcEdgePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarParapetRcEdgeStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarParapetRcEdgeD.Compare(a.D, b.D, key1, records);
                            StbSecBarParapetRcEdgeN.Compare(a.N, b.N, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarParapetRcEdge.Compare(nameof(StbSecBarParapet_RC_Edge), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarParapet_RC_Edge>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarParapetRcEdge.Compare(null, nameof(StbSecBarParapet_RC_Edge), key1, records);
                        }
                    }
                }
            }
            else
            {
                if (wallB.Items.Any(n => n is StbSecBarParapet_RC_Edge))
                {
                    StbSecBarParapetRcEdge.Compare(null, nameof(StbSecBarParapet_RC_Edge), key, records);
                }
            }

        }
    }
}

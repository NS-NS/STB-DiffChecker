using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecSlabRc
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stbridgeA?.StbModel?.StbSections?.StbSecSlab_RC;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecSlab_RC;
            var setB = secB != null ?  new HashSet<StbSecSlab_RC>(secB) : new HashSet<StbSecSlab_RC>();

            if (secA != null)
            {
                foreach (var secSlabA in secA)
                {
                    var key = new List<string> { "Name=" + secSlabA.name };
                    var secSlabB = secB?.FirstOrDefault(n => n.name == secSlabA.name);
                    if (secSlabB != null)
                    {
                        CompareSecSlabRc(secSlabA, secSlabB, key, records);
                        setB.Remove(secSlabB);
                    }
                    else
                    {
                        StbSecSlabRc.Compare(nameof(StbSecSlab_RC), null , key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> {"Name=" + b.name};
                StbSecSlabRc.Compare(null, nameof(StbSecSlab_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecSlabRc(StbSecSlab_RC slabA, StbSecSlab_RC slabB, IReadOnlyList<string>key, List<Record> records)
        {
            StbSecSlabRcId.Compare(slabA.id, slabB.id, key, records);
            StbSecSlabRcGuid.Compare(slabA.guid, slabB.guid, key, records);
            StbSecSlabRcName.Compare(slabA.name, slabB.name, key, records);
            StbSecSlabRcIsFoundation.Compare(slabA.isFoundation, slabB.isFoundation, key, records);
            StbSecSlabRcIsEarthen.Compare(slabA.isEarthen, slabB.isEarthen, key, records);
            StbSecSlabRcIsCanti.Compare(slabA.isCanti, slabB.isCanti, key, records);
            StbSecSlabRcStrengthConcrete.Compare(slabA.strength_concrete, slabB.strength_concrete, key, records);

            CompareSecFigureSlabRc(slabA.StbSecFigureSlab_RC, slabB.StbSecFigureSlab_RC, key, records);
            if (slabA.StbSecBarArrangementSlab_RC?.Items != null || slabB.StbSecBarArrangementSlab_RC?.Items != null)
            {
                if (slabB.StbSecBarArrangementSlab_RC?.Items == null)
                {
                    if (slabA.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_Standard)
                        StbSecBarArrangementSlabRc.Compare(nameof(StbSecBarSlab_RC_Standard), null, key, records);
                    else if (slabA.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_2Way)
                        StbSecBarArrangementSlabRc.Compare(nameof(StbSecBarSlab_RC_2Way), null, key, records);
                    else if (slabA.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_1Way1)
                        StbSecBarArrangementSlabRc.Compare(nameof(StbSecBarSlab_RC_1Way1), null, key, records);
                    else if (slabA.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_1Way2)
                        StbSecBarArrangementSlabRc.Compare(nameof(StbSecBarSlab_RC_1Way2), null, key, records);
                }
                else if (slabA.StbSecBarArrangementSlab_RC?.Items == null)
                {
                    if (slabB.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_Standard)
                        StbSecBarArrangementSlabRc.Compare(null, nameof(StbSecBarSlab_RC_Standard), key, records);
                    else if (slabB.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_2Way)
                        StbSecBarArrangementSlabRc.Compare(null, nameof(StbSecBarSlab_RC_2Way), key, records);
                    else if (slabB.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_1Way1)
                        StbSecBarArrangementSlabRc.Compare(null, nameof(StbSecBarSlab_RC_1Way1), key, records);
                    else if (slabB.StbSecBarArrangementSlab_RC.Items.First() is StbSecBarSlab_RC_1Way2)
                        StbSecBarArrangementSlabRc.Compare(null, nameof(StbSecBarSlab_RC_1Way2), key, records);
                }
                else
                {
                    CompareSecBarArrangementSlabRc(slabA.StbSecBarArrangementSlab_RC,
                        slabB.StbSecBarArrangementSlab_RC, key, records);
                }
            }
        }

        private static void CompareSecBarArrangementSlabRc(StbSecBarArrangementSlab_RC slabA, StbSecBarArrangementSlab_RC slabB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementSlabRcDepthCoverTop.Compare(slabA.depth_cover_topSpecified, slabA.depth_cover_top,
                slabB.depth_cover_topSpecified, slabB.depth_cover_top, key, records);
            StbSecBarArrangementSlabRcDepthCoverBottom.Compare(slabA.depth_cover_bottomSpecified,
                slabA.depth_cover_bottom, slabB.depth_cover_bottomSpecified, slabB.depth_cover_bottom, key, records);

            if (slabA.Items.Any(n => n is StbSecBarSlab_RC_Standard))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlab_RC_Standard))
                {
                    var set = new HashSet<StbSecBarSlab_RC_Standard>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlab_RC_Standard>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = slabB.Items.OfType<StbSecBarSlab_RC_Standard>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarSlabRcStandardPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarSlabRcStandardStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarSlabRcStandardD.Compare(a.D, b.D, key1, records);
                            StbSecBarSlabRcStandardPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarSlabRcStandard.Compare(nameof(StbSecBarSlab_RC_Standard), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlab_RC_Standard>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarSlabRcStandard.Compare(null, nameof(StbSecBarSlab_RC_Standard), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_2Way))
                {
                    StbSecBarSlabRcStandard.Compare(nameof(StbSecBarSlab_RC_Standard), nameof(StbSecBarSlab_RC_2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way1))
                {
                    StbSecBarSlabRcStandard.Compare(nameof(StbSecBarSlab_RC_Standard), nameof(StbSecBarSlab_RC_1Way1), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way2))
                {
                    StbSecBarSlabRcStandard.Compare(nameof(StbSecBarSlab_RC_Standard), nameof(StbSecBarSlab_RC_1Way2), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlab_RC_2Way))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlab_RC_Standard))
                {
                    StbSecBarSlabRc2Way.Compare(nameof(StbSecBarSlab_RC_2Way), nameof(StbSecBarSlab_RC_Standard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_2Way))
                {
                    var set = new HashSet<StbSecBarSlab_RC_2Way>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlab_RC_2Way>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = slabB.Items.OfType<StbSecBarSlab_RC_2Way>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarSlabRc2WayPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarSlabRc2WayStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarSlabRc2WayD.Compare(a.D, b.D, key1, records);
                            StbSecBarSlabRc2WayPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            StbSecBarSlabRc2Way.Compare(nameof(StbSecBarSlab_RC_2Way), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlab_RC_2Way>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarSlabRc2Way.Compare(null, nameof(StbSecBarSlab_RC_2Way), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way1))
                {
                    StbSecBarSlabRc2Way.Compare(nameof(StbSecBarSlab_RC_2Way), nameof(StbSecBarSlab_RC_1Way1), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way2))
                {
                    StbSecBarSlabRc2Way.Compare(nameof(StbSecBarSlab_RC_2Way), nameof(StbSecBarSlab_RC_1Way2), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlab_RC_1Way1))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlab_RC_Standard))
                {
                    StbSecBarSlabRc1Way1.Compare(nameof(StbSecBarSlab_RC_1Way1), nameof(StbSecBarSlab_RC_Standard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_2Way))
                {
                    StbSecBarSlabRc1Way1.Compare(nameof(StbSecBarSlab_RC_1Way1), nameof(StbSecBarSlab_RC_2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way1))
                {
                    var set = new HashSet<StbSecBarSlab_RC_1Way1>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlab_RC_1Way1>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = slabB.Items.OfType<StbSecBarSlab_RC_1Way1>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarSlabRc1Way1Pos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarSlabRc1Way1Strength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarSlabRc1Way1D.Compare(a.D, b.D, key1, records);
                            StbSecBarSlabRc1Way1Pitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            StbSecBarSlabRc1Way1.Compare(nameof(StbSecBarSlab_RC_1Way1), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlab_RC_1Way1>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarSlabRc1Way1.Compare(null, nameof(StbSecBarSlab_RC_1Way1), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way2))
                {
                    StbSecBarSlabRc1Way1.Compare(nameof(StbSecBarSlab_RC_1Way1), nameof(StbSecBarSlab_RC_1Way2), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlab_RC_1Way2))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlab_RC_Standard))
                {
                    StbSecBarSlabRc1Way2.Compare(nameof(StbSecBarSlab_RC_1Way2), nameof(StbSecBarSlab_RC_Standard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_2Way))
                {
                    StbSecBarSlabRc1Way2.Compare(nameof(StbSecBarSlab_RC_1Way2), nameof(StbSecBarSlab_RC_2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way1))
                {
                    StbSecBarSlabRc1Way2.Compare(nameof(StbSecBarSlab_RC_1Way2), nameof(StbSecBarSlab_RC_1Way1), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlab_RC_1Way2))
                {
                    var set = new HashSet<StbSecBarSlab_RC_1Way2>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlab_RC_1Way2>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = slabB.Items.OfType<StbSecBarSlab_RC_1Way2>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarSlabRc1Way2Pos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarSlabRc1Way2Strength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarSlabRc1Way2D.Compare(a.D, b.D, key1, records);
                            StbSecBarSlabRc1Way2Pitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarSlabRc1Way2.Compare(nameof(StbSecBarSlab_RC_1Way2), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlab_RC_1Way2>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarSlabRc1Way2.Compare(null, nameof(StbSecBarSlab_RC_1Way2), keyB, records);
                        }
                    }
                }
            }

            if (slabA.Items.Any(n => n is StbSecBarSlab_RC_Open))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlab_RC_Open))
                {
                    var set = new HashSet<StbSecBarSlab_RC_Open>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlab_RC_Open>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = slabB.Items.OfType<StbSecBarSlab_RC_Open>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarSlabRcOpenPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarSlabRcOpenStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarSlabRcOpenD.Compare(a.D, b.D, key1, records);
                            StbSecBarSlabRcOpenN.Compare(a.N, b.N, key1, records);
                            StbSecBarSlabRcOpenLength.Compare(a.lengthSpecified, a.length, b.lengthSpecified, b.length, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            StbSecBarSlabRcOpen.Compare(nameof(StbSecBarSlab_RC_Open), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlab_RC_Open>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarSlabRcOpen.Compare(null, nameof(StbSecBarSlab_RC_1Way2), keyB, records);
                        }
                    }

                }
                else
                {
                    StbSecBarSlabRcOpen.Compare(nameof(StbSecBarSlab_RC_Open), null, key, records);
                }
            }
            else
            {
                if (slabB.Items.Any(n => n is StbSecBarSlab_RC_Open))
                {
                    StbSecBarSlabRcOpen.Compare(null, nameof(StbSecBarSlab_RC_Open), key, records);
                }
            }
        }

        private static void CompareSecFigureSlabRc(StbSecFigureSlab_RC slabA, StbSecFigureSlab_RC slabB, IReadOnlyList<string> key, List<Record> records)
        {
            if (slabA.Items.First() is StbSecSlab_RC_Straight  straightA)
            {
                if (slabB.Items.First() is StbSecSlab_RC_Straight straightB)
                {
                    StbSecSlabRcStraightDepth.Compare(straightA.depth, straightB.depth, key, records);
                }
                else if (slabB.Items.First() is StbSecSlab_RC_Taper)
                {
                    StbSecSlabRcStraight.Compare(nameof(StbSecSlab_RC_Straight), nameof(StbSecSlab_RC_Taper), key, records);
                }
                else if (slabB.Items.First() is StbSecSlab_RC_Haunch)
                {
                    StbSecSlabRcStraight.Compare(nameof(StbSecSlab_RC_Straight), nameof(StbSecSlab_RC_Haunch), key, records);
                }
            }
            else if (slabA.Items.First() is StbSecSlab_RC_Taper taperA)
            {
                if (slabB.Items.First() is StbSecSlab_RC_Straight)
                {
                    StbSecSlabRcTaper.Compare(nameof(StbSecSlab_RC_Taper), nameof(StbSecSlab_RC_Straight), key, records);
                }
                else if (slabB.Items.First() is StbSecSlab_RC_Taper)
                {
                    var key1 = new List<string>(key) {"pos=" + taperA.pos};
                    var b = slabB.Items.OfType<StbSecSlab_RC_Taper>().FirstOrDefault(n => n.pos == taperA.pos);
                    if (b != null)
                    {
                        StbSecSlabRcTaperPos.Compare(taperA.pos.ToString(), b.pos.ToString(), key1, records);
                        StbSecSlabRcTaperDepth.Compare(taperA.depth, b.depth, key1, records);
                    }
                    else
                    {
                        StbSecSlabRcTaper.Compare(nameof(StbSecSlab_RC_Taper), null, key, records);
                    }
                }
                else if (slabB.Items.First() is StbSecSlab_RC_Haunch)
                {
                    StbSecSlabRcTaper.Compare(nameof(StbSecSlab_RC_Taper), nameof(StbSecSlab_RC_Haunch), key, records);
                }
            }
            else if (slabA.Items.First() is StbSecSlab_RC_Haunch haunchA)
            {
                if (slabB.Items.First() is StbSecSlab_RC_Straight)
                {
                    StbSecSlabRcHaunch.Compare(nameof(StbSecSlab_RC_Haunch), nameof(StbSecSlab_RC_Straight), key, records);
                }
                else if (slabB.Items.First() is StbSecSlab_RC_Taper)
                {
                    StbSecSlabRcHaunch.Compare(nameof(StbSecSlab_RC_Haunch), nameof(StbSecSlab_RC_Taper), key, records);
                }
                else if (slabB.Items.First() is StbSecSlab_RC_Haunch)
                {
                    var key1 = new List<string>(key) {"pos=" + haunchA.pos};
                    var b = slabB.Items.OfType<StbSecSlab_RC_Haunch>().FirstOrDefault(n => n.pos == haunchA.pos);
                    if (b != null)
                    {
                        StbSecSlabRcTaperPos.Compare(haunchA.pos.ToString(), b.pos.ToString(), key1, records);
                        StbSecSlabRcTaperDepth.Compare(haunchA.depth, b.depth, key1, records);
                    }
                    else
                    {
                        StbSecSlabRcTaper.Compare(nameof(StbSecSlab_RC_Taper), null, key1, records);
                    }
                }
            }
        }
    }
}

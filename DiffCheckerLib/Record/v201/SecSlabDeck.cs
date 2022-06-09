using STBridge201;
using System.Collections.Generic;
using System.Linq;
using StbSecSlabDeck = STBridge201.StbSecSlabDeck;

namespace STBDiffChecker.v201.Records
{
    internal static class SecSlabDeck
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stbridgeA?.StbModel?.StbSections?.StbSecSlabDeck;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecSlabDeck;
            var setB = secB != null ? new HashSet<StbSecSlabDeck>(secB) : new HashSet<StbSecSlabDeck>();

            if (secA != null)
            {
                foreach (var secSlabA in secA)
                {
                    var key = new List<string> { "Name=" + secSlabA.name };
                    var secSlabB = secB?.FirstOrDefault(n => n.name == secSlabA.name);
                    if (secSlabB != null)
                    {
                        CompareSecSlabDeck(secSlabA, secSlabB, key, records);
                        setB.Remove(secSlabB);
                    }
                    else
                    {
                        CheckObjects.StbSecSlabDeck.Compare(nameof(StbSecSlabDeck), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { "Name=" + b.name };
                CheckObjects.StbSecSlabDeck.Compare(null, nameof(StbSecSlabDeck), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecSlabDeck(StbSecSlabDeck slabA, StbSecSlabDeck slabB, IReadOnlyList<string> key, List<Record> records)
        {
            CheckObjects.StbSecSlabDeckId.Compare(slabA.id, slabB.id, key, records);
            CheckObjects.StbSecSlabDeckGuid.Compare(slabA.guid, slabB.guid, key, records);
            CheckObjects.StbSecSlabDeckName.Compare(slabA.name, slabB.name, key, records);
            CheckObjects.StbSecSlabDeckProductType.Compare(slabA.product_type.ToString(), slabB.product_type.ToString(), key, records);
            CheckObjects.StbSecSlabDeckStrengthConcrete.Compare(slabA.strength_concrete, slabB.strength_concrete, key, records);

            CheckObjects.StbSecSlabDeckStraightDepth.Compare(slabA.StbSecFigureSlabDeck.StbSecSlabDeckStraight.depth, slabB.StbSecFigureSlabDeck.StbSecSlabDeckStraight.depth, key, records);
            if (slabA.StbSecBarArrangementSlabDeck?.Items != null || slabB.StbSecBarArrangementSlabDeck?.Items != null)
            {
                if (slabB.StbSecBarArrangementSlabDeck?.Items == null)
                {
                    if (slabA.StbSecBarArrangementSlabDeck.Items.First() is StbSecBarSlabDeckStandard)
                        CheckObjects.StbSecBarArrangementSlabDeck.Compare(nameof(StbSecBarSlabDeckStandard), null, key, records);
                    else if (slabA.StbSecBarArrangementSlabDeck.Items.First() is StbSecBarSlabDeck2Way)
                        CheckObjects.StbSecBarArrangementSlabDeck.Compare(nameof(StbSecBarSlabDeck2Way), null, key, records);
                    else if (slabA.StbSecBarArrangementSlabDeck.Items.First() is StbSecBarSlabDeck1Way)
                        CheckObjects.StbSecBarArrangementSlabDeck.Compare(nameof(StbSecBarSlabDeck1Way), null, key, records);
                }
                else if (slabA.StbSecBarArrangementSlabDeck?.Items == null)
                {
                    if (slabB.StbSecBarArrangementSlabDeck.Items.First() is StbSecBarSlabDeckStandard)
                        CheckObjects.StbSecBarArrangementSlabDeck.Compare(null, nameof(StbSecBarSlabDeckStandard), key, records);
                    else if (slabB.StbSecBarArrangementSlabDeck.Items.First() is StbSecBarSlabDeck2Way)
                        CheckObjects.StbSecBarArrangementSlabDeck.Compare(null, nameof(StbSecBarSlabDeck2Way), key, records);
                    else if (slabB.StbSecBarArrangementSlabDeck.Items.First() is StbSecBarSlabDeck1Way)
                        CheckObjects.StbSecBarArrangementSlabDeck.Compare(null, nameof(StbSecBarSlabDeck1Way), key, records);
                }
                else
                {
                    CompareSecBarArrangementSlabDeck(slabA.StbSecBarArrangementSlabDeck,
                        slabB.StbSecBarArrangementSlabDeck, key, records);
                }
            }
        }

        private static void CompareSecBarArrangementSlabDeck(StbSecBarArrangementSlabDeck slabA, StbSecBarArrangementSlabDeck slabB, IReadOnlyList<string> key, List<Record> records)
        {
            CheckObjects.StbSecBarArrangementSlabDeckDepthCoverTop.Compare(slabA.depth_cover_topSpecified, slabA.depth_cover_top,
                slabB.depth_cover_topSpecified, slabB.depth_cover_top, key, records);
            CheckObjects.StbSecBarArrangementSlabDeckDepthCoverBottom.Compare(slabA.depth_cover_bottomSpecified,
                slabA.depth_cover_bottom, slabB.depth_cover_bottomSpecified, slabB.depth_cover_bottom, key, records);

            if (slabA.Items.Any(n => n is StbSecBarSlabDeckStandard))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlabDeckStandard))
                {
                    var set = new HashSet<StbSecBarSlabDeckStandard>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlabDeckStandard>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = slabB.Items.OfType<StbSecBarSlabDeckStandard>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            CheckObjects.StbSecBarSlabDeckStandardPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            CheckObjects.StbSecBarSlabDeckStandardStrength.Compare(a.strength, b.strength, key1, records);
                            CheckObjects.StbSecBarSlabDeckStandardD.Compare(a.D, b.D, key1, records);
                            CheckObjects.StbSecBarSlabDeckStandardPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            CheckObjects.StbSecBarSlabDeckStandard.Compare(nameof(StbSecBarSlabDeckStandard), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlabDeckStandard>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            CheckObjects.StbSecBarSlabDeckStandard.Compare(null, nameof(StbSecBarSlabDeckStandard), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabDeck2Way))
                {
                    CheckObjects.StbSecBarSlabDeckStandard.Compare(nameof(StbSecBarSlabDeckStandard), nameof(StbSecBarSlabDeck2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabDeck1Way))
                {
                    CheckObjects.StbSecBarSlabDeckStandard.Compare(nameof(StbSecBarSlabDeckStandard), nameof(StbSecBarSlabDeck1Way), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlabDeck2Way))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlabDeckStandard))
                {
                    CheckObjects.StbSecBarSlabDeck2Way.Compare(nameof(StbSecBarSlabDeck2Way), nameof(StbSecBarSlabDeckStandard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabDeck2Way))
                {
                    var set = new HashSet<StbSecBarSlabDeck2Way>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlabDeck2Way>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = slabB.Items.OfType<StbSecBarSlabDeck2Way>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            CheckObjects.StbSecBarSlabDeck2WayPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            CheckObjects.StbSecBarSlabDeck2WayStrength.Compare(a.strength, b.strength, key1, records);
                            CheckObjects.StbSecBarSlabDeck2WayD.Compare(a.D, b.D, key1, records);
                            CheckObjects.StbSecBarSlabDeck2WayPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            CheckObjects.StbSecBarSlabDeck2Way.Compare(nameof(StbSecBarSlabDeck2Way), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlabDeck2Way>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            CheckObjects.StbSecBarSlabDeck2Way.Compare(null, nameof(StbSecBarSlabDeck2Way), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabDeck1Way))
                {
                    CheckObjects.StbSecBarSlabDeck2Way.Compare(nameof(StbSecBarSlabDeck2Way), nameof(StbSecBarSlabDeck1Way), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlabDeck1Way))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlabDeckStandard))
                {
                    CheckObjects.StbSecBarSlabDeck1Way.Compare(nameof(StbSecBarSlabDeck1Way), nameof(StbSecBarSlabDeckStandard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabDeck2Way))
                {
                    CheckObjects.StbSecBarSlabDeck1Way.Compare(nameof(StbSecBarSlabDeck1Way), nameof(StbSecBarSlabDeck2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabDeck1Way))
                {
                    var set = new HashSet<StbSecBarSlabDeck1Way>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlabDeck1Way>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = slabB.Items.OfType<StbSecBarSlabDeck1Way>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            CheckObjects.StbSecBarSlabDeck1WayPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            CheckObjects.StbSecBarSlabDeck1WayStrength.Compare(a.strength, b.strength, key1, records);
                            CheckObjects.StbSecBarSlabDeck1WayD.Compare(a.D, b.D, key1, records);
                            CheckObjects.StbSecBarSlabDeck1WayPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            CheckObjects.StbSecBarSlabDeck1Way.Compare(nameof(StbSecBarSlabDeck1Way), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlabDeck1Way>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            CheckObjects.StbSecBarSlabDeckStandard.Compare(null, nameof(StbSecBarSlabDeck1Way), keyB, records);
                        }
                    }
                }
            }
        }
    }
}

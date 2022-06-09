using STBridge201;
using System.Collections.Generic;
using System.Linq;
using StbSecSlabPrecast = STBridge201.StbSecSlabPrecast;

namespace STBDiffChecker.v201.Records
{
    internal static class SecSlabPrecast
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stbridgeA?.StbModel?.StbSections?.StbSecSlabPrecast;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecSlabPrecast;
            var setB = secB != null ? new HashSet<StbSecSlabPrecast>(secB) : new HashSet<StbSecSlabPrecast>();

            if (secA != null)
            {
                foreach (var secSlabA in secA)
                {
                    var key = new List<string> { "Name=" + secSlabA.name };
                    var secSlabB = secB?.FirstOrDefault(n => n.name == secSlabA.name);
                    if (secSlabB != null)
                    {
                        CompareSecSlabPrecast(secSlabA, secSlabB, key, records);
                        setB.Remove(secSlabB);
                    }
                    else
                    {
                        CheckObjects.StbSecSlabPrecast.Compare(nameof(StbSecSlabPrecast), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { "Name=" + b.name };
                CheckObjects.StbSecSlabPrecast.Compare(null, nameof(StbSecSlabPrecast), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecSlabPrecast(StbSecSlabPrecast slabA, StbSecSlabPrecast slabB, IReadOnlyList<string> key, List<Record> records)
        {
            CheckObjects.StbSecSlabPrecastId.Compare(slabA.id, slabB.id, key, records);
            CheckObjects.StbSecSlabPrecastGuid.Compare(slabA.guid, slabB.guid, key, records);
            CheckObjects.StbSecSlabPrecastName.Compare(slabA.name, slabB.name, key, records);
            CheckObjects.StbSecSlabPrecastPrecastType.Compare(slabA.precast_type.ToString(), slabB.precast_type.ToString(), key, records);
            CheckObjects.StbSecSlabPrecastStrengthConcrete.Compare(slabA.strength_concrete, slabB.strength_concrete, key, records);

            if (slabA.StbSecFigureSlabPrecast != null || slabB.StbSecFigureSlabPrecast != null)
            {
                if (slabB.StbSecFigureSlabPrecast == null)
                {
                    CheckObjects.StbSecFigureSlabPrecast.Compare(nameof(StbSecFigureSlabPrecast), null, key, records);
                }
                else if (slabA.StbSecFigureSlabPrecast == null)
                {
                    CheckObjects.StbSecFigureSlabPrecast.Compare(null, nameof(StbSecFigureSlabPrecast), key, records);
                }
                else
                {
                    CheckObjects.StbSecSlabPrecastStraightDepthConcrete.Compare(
                        slabA.StbSecFigureSlabPrecast.StbSecSlabPrecastStraight.depth_concrete,
                        slabB.StbSecFigureSlabPrecast.StbSecSlabPrecastStraight.depth_concrete, key, records);
                }
            }
            
            if (slabA.StbSecBarArrangementSlabPrecast?.Items != null || slabB.StbSecBarArrangementSlabPrecast?.Items != null)
            {
                if (slabB.StbSecBarArrangementSlabPrecast?.Items == null)
                {
                    if (slabA.StbSecBarArrangementSlabPrecast.Items.First() is StbSecBarSlabPrecastStandard)
                        CheckObjects.StbSecBarArrangementSlabPrecast.Compare(nameof(StbSecBarSlabPrecastStandard), null, key, records);
                    else if (slabA.StbSecBarArrangementSlabPrecast.Items.First() is StbSecBarSlabPrecast2Way)
                        CheckObjects.StbSecBarArrangementSlabPrecast.Compare(nameof(StbSecBarSlabPrecast2Way), null, key, records);
                    else if (slabA.StbSecBarArrangementSlabPrecast.Items.First() is StbSecBarSlabPrecast1Way)
                        CheckObjects.StbSecBarArrangementSlabPrecast.Compare(nameof(StbSecBarSlabPrecast1Way), null, key, records);
                }
                else if (slabA.StbSecBarArrangementSlabPrecast?.Items == null)
                {
                    if (slabB.StbSecBarArrangementSlabPrecast.Items.First() is StbSecBarSlabPrecastStandard)
                        CheckObjects.StbSecBarArrangementSlabPrecast.Compare(null, nameof(StbSecBarSlabPrecastStandard), key, records);
                    else if (slabB.StbSecBarArrangementSlabPrecast.Items.First() is StbSecBarSlabPrecast2Way)
                        CheckObjects.StbSecBarArrangementSlabPrecast.Compare(null, nameof(StbSecBarSlabPrecast2Way), key, records);
                    else if (slabB.StbSecBarArrangementSlabPrecast.Items.First() is StbSecBarSlabPrecast1Way)
                        CheckObjects.StbSecBarArrangementSlabPrecast.Compare(null, nameof(StbSecBarSlabPrecast1Way), key, records);
                }
                else
                {
                    CompareSecBarArrangementSlabPrecast(slabA.StbSecBarArrangementSlabPrecast,
                        slabB.StbSecBarArrangementSlabPrecast, key, records);
                }
            }

            CheckObjects.StbSecProductSlabPrecastProductCompany.Compare(slabA.StbSecProductSlabPrecast.product_company,
                slabB.StbSecProductSlabPrecast.product_company, key, records);
            CheckObjects.StbSecProductSlabPrecastProductName.Compare(slabA.StbSecProductSlabPrecast.product_name,
                slabB.StbSecProductSlabPrecast.product_name, key, records);
            CheckObjects.StbSecProductSlabPrecastProductCode.Compare(slabA.StbSecProductSlabPrecast.product_code,
                slabB.StbSecProductSlabPrecast.product_code, key, records);
            CheckObjects.StbSecProductSlabPrecastDepth.Compare(slabA.StbSecProductSlabPrecast.depth,
                slabB.StbSecProductSlabPrecast.depth, key, records);
        }

        private static void CompareSecBarArrangementSlabPrecast(StbSecBarArrangementSlabPrecast slabA, StbSecBarArrangementSlabPrecast slabB, IReadOnlyList<string> key, List<Record> records)
        {
            CheckObjects.StbSecBarArrangementSlabPrecastDepthCoverTop.Compare(slabA.depth_cover_topSpecified, slabA.depth_cover_top,
                slabB.depth_cover_topSpecified, slabB.depth_cover_top, key, records);

            if (slabA.Items.Any(n => n is StbSecBarSlabPrecastStandard))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlabPrecastStandard))
                {
                    var set = new HashSet<StbSecBarSlabPrecastStandard>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlabPrecastStandard>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = slabB.Items.OfType<StbSecBarSlabPrecastStandard>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            CheckObjects.StbSecBarSlabPrecastStandardPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            CheckObjects.StbSecBarSlabPrecastStandardStrength.Compare(a.strength, b.strength, key1, records);
                            CheckObjects.StbSecBarSlabPrecastStandardD.Compare(a.D, b.D, key1, records);
                            CheckObjects.StbSecBarSlabPrecastStandardPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            CheckObjects.StbSecBarSlabPrecastStandard.Compare(nameof(StbSecBarSlabPrecastStandard), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlabPrecastStandard>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            CheckObjects.StbSecBarSlabPrecastStandard.Compare(null, nameof(StbSecBarSlabPrecastStandard), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabPrecast2Way))
                {
                    CheckObjects.StbSecBarSlabPrecastStandard.Compare(nameof(StbSecBarSlabPrecastStandard), nameof(StbSecBarSlabPrecast2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabPrecast1Way))
                {
                    CheckObjects.StbSecBarSlabPrecastStandard.Compare(nameof(StbSecBarSlabPrecastStandard), nameof(StbSecBarSlabPrecast1Way), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlabPrecast2Way))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlabPrecastStandard))
                {
                    CheckObjects.StbSecBarSlabPrecast2Way.Compare(nameof(StbSecBarSlabPrecast2Way), nameof(StbSecBarSlabPrecastStandard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabPrecast2Way))
                {
                    var set = new HashSet<StbSecBarSlabPrecast2Way>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlabPrecast2Way>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = slabB.Items.OfType<StbSecBarSlabPrecast2Way>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            CheckObjects.StbSecBarSlabPrecast2WayPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            CheckObjects.StbSecBarSlabPrecast2WayStrength.Compare(a.strength, b.strength, key1, records);
                            CheckObjects.StbSecBarSlabPrecast2WayD.Compare(a.D, b.D, key1, records);
                            CheckObjects.StbSecBarSlabPrecast2WayPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            CheckObjects.StbSecBarSlabPrecast2Way.Compare(nameof(StbSecBarSlabPrecast2Way), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlabPrecast2Way>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            CheckObjects.StbSecBarSlabPrecast2Way.Compare(null, nameof(StbSecBarSlabPrecast2Way), keyB, records);
                        }
                    }
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabPrecast1Way))
                {
                    CheckObjects.StbSecBarSlabPrecast2Way.Compare(nameof(StbSecBarSlabPrecast2Way), nameof(StbSecBarSlabPrecast1Way), key, records);
                }
            }
            else if (slabA.Items.Any(n => n is StbSecBarSlabPrecast1Way))
            {
                if (slabB.Items.Any(n => n is StbSecBarSlabPrecastStandard))
                {
                    CheckObjects.StbSecBarSlabPrecast1Way.Compare(nameof(StbSecBarSlabPrecast1Way), nameof(StbSecBarSlabPrecastStandard), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabPrecast2Way))
                {
                    CheckObjects.StbSecBarSlabPrecast1Way.Compare(nameof(StbSecBarSlabPrecast1Way), nameof(StbSecBarSlabPrecast2Way), key, records);
                }
                else if (slabB.Items.Any(n => n is StbSecBarSlabPrecast1Way))
                {
                    var set = new HashSet<StbSecBarSlabPrecast1Way>();
                    foreach (var a in slabA.Items.OfType<StbSecBarSlabPrecast1Way>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = slabB.Items.OfType<StbSecBarSlabPrecast1Way>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            CheckObjects.StbSecBarSlabPrecast1WayPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            CheckObjects.StbSecBarSlabPrecast1WayStrength.Compare(a.strength, b.strength, key1, records);
                            CheckObjects.StbSecBarSlabPrecast1WayD.Compare(a.D, b.D, key1, records);
                            CheckObjects.StbSecBarSlabPrecast1WayPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                            break;
                        }
                        else
                        {
                            CheckObjects.StbSecBarSlabPrecast1Way.Compare(nameof(StbSecBarSlabPrecast1Way), null, key1, records);
                        }
                    }

                    foreach (var b in slabB.Items.OfType<StbSecBarSlabPrecast1Way>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            CheckObjects.StbSecBarSlabPrecastStandard.Compare(null, nameof(StbSecBarSlabPrecast1Way), keyB, records);
                        }
                    }
                }
            }
        }
    }
}

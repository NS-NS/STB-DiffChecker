using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecBraceS
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stBridgeA?.StbModel?.StbSections?.StbSecBrace_S;
            var secB = stBridgeB?.StbModel?.StbSections?.StbSecBrace_S;
            var setB = secB != null ? new HashSet<StbSecBrace_S>(secB) : new HashSet<StbSecBrace_S>();

            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name, "floor=" + secColumnA.floor };
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name && n.floor == secColumnA.floor);
                    if (secColumnB != null)
                    {
                        CompareSecBraceS(stBridgeA, stBridgeB, secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecBraceS.Compare(nameof(StbSecBrace_S), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var keyB = new List<string> { "Name=" + b.name, "floor=" + b.floor };
                StbSecBraceS.Compare(null, nameof(StbSecBrace_S), keyB, records);

            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecBraceS(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecBrace_S secA, StbSecBrace_S secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBraceSId.Compare(secA.id, secB.id, key, records);
            StbSecBraceSGuid.Compare(secA.guid, secB.guid, key, records);
            StbSecBraceSName.Compare(secA.name, secB.name, key, records);
            StbSecBraceSFloor.Compare(secA.floor, secB.floor, key, records);
            StbSecBraceSKindBrace.Compare(secA.kind_brace.ToString(), secB.kind_brace.ToString(), key, records);

            CompareSecSteelFigureBraceS(stBridgeA, stBridgeB, secA.StbSecSteelFigureBrace_S,
                secB.StbSecSteelFigureBrace_S, key, records);
        }

        private static void CompareSecSteelFigureBraceS(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecSteelFigureBrace_S secA, StbSecSteelFigureBrace_S secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecSteelFigureBraceSJointIdStart.Compare(secA.joint_id_start, stBridgeA, secB.joint_id_start, stBridgeB, key, records);
            StbSecSteelFigureBraceSJointIdEnd.Compare(secA.joint_id_end, stBridgeA, secB.joint_id_end, stBridgeB, key, records);

            if (secA.Items.Any(n => n is StbSecSteelBrace_S_Same))
            {
                if (secB.Items.Any(n => n is StbSecSteelBrace_S_Same))
                {
                    var a = secA.Items.OfType<StbSecSteelBrace_S_Same>().First();
                    var b = secB.Items.OfType<StbSecSteelBrace_S_Same>().First();
                    StbSecSteelBraceSSameShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key, records);
                    StbSecSteelBraceSSameStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                    StbSecSteelBraceSSameStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBrace_S_NotSame))
                {
                    StbSecSteelBraceSSame.Compare(nameof(StbSecSteelBrace_S_Same), nameof(StbSecSteelBrace_S_NotSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBrace_S_ThreeTypes))
                {
                    StbSecSteelBraceSSame.Compare(nameof(StbSecSteelBrace_S_Same), nameof(StbSecSteelBrace_S_ThreeTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBrace_S_NotSame))
            {
                if (secB.Items.Any(n => n is StbSecSteelBrace_S_Same))
                {
                    StbSecSteelBraceSNotSame.Compare(nameof(StbSecSteelBrace_S_NotSame), nameof(StbSecSteelBrace_S_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBrace_S_NotSame))
                {
                    var set = new HashSet<StbSecSteelBrace_S_NotSame>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBrace_S_NotSame>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBrace_S_NotSame>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBraceSNotSamePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBraceSNotSameShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBraceSNotSameStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBraceSNotSameStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBraceSNotSame.Compare(nameof(StbSecSteelBraceSNotSame), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBrace_S_NotSame>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBraceSNotSame.Compare(null, nameof(StbSecSteelBraceSNotSame), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBrace_S_ThreeTypes))
                {
                    StbSecSteelBraceSNotSame.Compare(nameof(StbSecSteelBrace_S_NotSame), nameof(StbSecSteelBrace_S_ThreeTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBrace_S_ThreeTypes))
            {
                if (secB.Items.Any(n => n is StbSecSteelBrace_S_Same))
                {
                    StbSecSteelBraceSThreeTypes.Compare(nameof(StbSecSteelBrace_S_ThreeTypes), nameof(StbSecSteelBrace_S_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBrace_S_NotSame))
                {
                    StbSecSteelBraceSThreeTypes.Compare(nameof(StbSecSteelBrace_S_ThreeTypes), nameof(StbSecSteelBrace_S_NotSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBrace_S_ThreeTypes))
                {
                    var set = new HashSet<StbSecSteelBrace_S_ThreeTypes>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBrace_S_ThreeTypes>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBrace_S_ThreeTypes>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBraceSThreeTypesPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBraceSThreeTypesShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBraceSThreeTypesStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBraceSThreeTypesStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBraceSThreeTypes.Compare(nameof(StbSecSteelBrace_S_ThreeTypes), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBrace_S_ThreeTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBraceSThreeTypes.Compare(null, nameof(StbSecSteelBrace_S_ThreeTypes), keyB, records);
                        }
                    }
                }
            }
        }
    }
}

using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecPileS
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stBridgeA?.StbModel?.StbSections?.StbSecPile_S;
            var secB = stBridgeB?.StbModel?.StbSections?.StbSecPile_S;
            var setB = secB != null ? new HashSet<StbSecPile_S>(secB) : new HashSet<StbSecPile_S>();

            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name};
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name);
                    if (secColumnB != null)
                    {
                        CompareSecPileS(secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecPileS.Compare(nameof(StbSecPile_S), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var keyB = new List<string> { "Name=" + b.name};
                StbSecPileS.Compare(null, nameof(StbSecPile_S), keyB, records);

            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecPileS(StbSecPile_S secA, StbSecPile_S secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecPileSId.Compare(secA.id, secB.id, key, records);
            StbSecPileSGuid.Compare(secA.guid, secB.guid, key, records);
            StbSecPileSName.Compare(secA.name, secB.name, key, records);

            CompareSecSteelFigurePileS(secA.StbSecFigurePile_S,
                secB.StbSecFigurePile_S, key, records);
        }

        private static void CompareSecSteelFigurePileS(StbSecFigurePile_S secA, StbSecFigurePile_S secB, IReadOnlyList<string> key, List<Record> records)
        {
            // id_orderで整理
            var pileA = SortPileS(secA);
            var pileB = SortPileS(secB);
            var order = new HashSet<int>();

            foreach (var a in pileA)
            {
                var key1 = new List<string>(key) {"id_order=" + a.Key};
                bool hasItem = false;
                foreach (var b in pileB)
                {
                    if (a.Key == b.Key)
                    {
                        CompareSecSteelFigurePileS(a.Value, b.Value, key1, records);
                        order.Add(a.Key);
                    }
                }

                if (!hasItem)
                {

                    StbSecFigurePileS.Compare(GetPileSName(a), null, key1, records);
                }
            }

            foreach (var b in pileB)
            {
                if (!order.Contains(b.Key))
                {
                    var key1 = new List<string>(key) { "id_order=" + b.Key };
                    StbSecFigurePileS.Compare(null, GetPileSName(b), key1, records);
                }
            }
        }

        private static void CompareSecSteelFigurePileS(object secA, object secB, List<string> key, List<Record> records)
        {
            if (secA is StbSecPile_S_Straight straightA)
            {
                if (secB is StbSecPile_S_Straight straightB)
                {
                    StbSecPileSStraightIdOrder.Compare(straightA.id_order, straightB.id_order, key, records);
                    StbSecPileSStraightProductCompany.Compare(straightA.product_company, straightB.product_company, key, records);
                    StbSecPileSStraightProductCode.Compare(straightA.product_code, straightB.product_code, key, records);
                    StbSecPileSStraightLengthPile.Compare(straightA.length_pile, straightB.length_pile, key, records);
                    StbSecPileSStraightD.Compare(straightA.D, straightB.D, key, records);
                    StbSecPileSStraightT.Compare(straightA.t, straightB.t, key, records);
                    StbSecPileSStraightStrength.Compare(straightA.strength, straightB.strength, key, records);
                }
                else if (secB is StbSecPile_S_Rotational)
                {
                    StbSecPileSStraight.Compare(nameof(StbSecPile_S_Straight), nameof(StbSecPile_S_Rotational), key, records);
                }
                else if (secB is StbSecPile_S_Taper)
                {
                    StbSecPileSStraight.Compare(nameof(StbSecPile_S_Straight), nameof(StbSecPile_S_Taper), key, records);
                }

            }
            else if (secA is StbSecPile_S_Rotational rotationalA)
            {
                if (secB is StbSecPile_S_Straight)
                {
                    StbSecPileSRotational.Compare(nameof(StbSecPile_S_Rotational), nameof(StbSecPile_S_Straight), key, records);
                }
                else if (secB is StbSecPile_S_Rotational rotationalB)
                {
                    StbSecPileSRotationalIdOrder.Compare(rotationalA.id_order, rotationalB.id_order, key, records);
                    StbSecPileSRotationalProductCompany.Compare(rotationalA.product_company, rotationalB.product_company, key, records);
                    StbSecPileSRotationalProductCode.Compare(rotationalA.product_code, rotationalB.product_code, key, records);
                    StbSecPileSRotationalLengthPile.Compare(rotationalA.length_pile, rotationalB.length_pile, key, records);
                    StbSecPileSRotationalD1.Compare(rotationalA.D1, rotationalB.D1, key, records);
                    StbSecPileSRotationalD2.Compare(rotationalA.D2, rotationalB.D2, key, records);
                    StbSecPileSRotationalT.Compare(rotationalA.t, rotationalB.t, key, records);
                    StbSecPileSRotationalStrength.Compare(rotationalA.strength, rotationalB.strength, key, records);
                }
                else if (secB is StbSecPile_S_Taper)
                {
                    StbSecPileSRotational.Compare(nameof(StbSecPile_S_Rotational), nameof(StbSecPile_S_Taper), key, records);
                }

            }
            else if (secA is StbSecPile_S_Taper taperA)
            {
                if (secB is StbSecPile_S_Straight)
                {
                    StbSecPileSTaper.Compare(nameof(StbSecPile_S_Taper), nameof(StbSecPile_S_Straight), key, records);
                }
                else if (secB is StbSecPile_S_Rotational)
                {
                    StbSecPileSTaper.Compare(nameof(StbSecPile_S_Taper), nameof(StbSecPile_S_Straight), key, records);
                }
                else if (secB is StbSecPile_S_Taper taperB)
                {
                    StbSecPileSTaperIdOrder.Compare(taperA.id_order, taperB.id_order, key, records);
                    StbSecPileSTaperProductCompany.Compare(taperA.product_company, taperB.product_company, key, records);
                    StbSecPileSTaperProductCode.Compare(taperA.product_code, taperB.product_code, key, records);
                    StbSecPileSTaperLengthPile.Compare(taperA.length_pile, taperB.length_pile, key, records);
                    StbSecPileSTaperD1.Compare(taperA.D1, taperB.D1, key, records);
                    StbSecPileSTaperD2.Compare(taperA.D2, taperB.D2, key, records);
                    StbSecPileSTaperT.Compare(taperA.t, taperB.t, key, records);
                    StbSecPileSTaperStrength.Compare(taperA.strength, taperB.strength, key, records);
                }
            }
        }

        private static string GetPileSName(object obj)
        {
            if (obj is StbSecPile_S_Straight)
                return nameof(StbSecPile_S_Straight);
            else if (obj is StbSecPile_S_Rotational)
                return nameof(StbSecPile_S_Rotational);
            else if (obj is StbSecPile_S_Taper)
                return nameof(StbSecPile_S_Taper);
            throw new Exception();
        }

        private static Dictionary<int, object> SortPileS(StbSecFigurePile_S secA)
        {
            var pile = new Dictionary<int, object>();
            if (secA.StbSecPile_S_Straight != null)
            {
                foreach (var straight in secA.StbSecPile_S_Straight)
                {
                    pile.Add(int.Parse(straight.id_order), straight);
                }
            }

            if (secA.StbSecPile_S_Rotational != null)
            {
                foreach (var rotational in secA.StbSecPile_S_Rotational)
                {
                    pile.Add(int.Parse(rotational.id_order), rotational);
                }
            }

            if (secA.StbSecPile_S_Taper != null)
            {
                foreach (var taper in secA.StbSecPile_S_Taper)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }

            return pile;
        }
    }
}

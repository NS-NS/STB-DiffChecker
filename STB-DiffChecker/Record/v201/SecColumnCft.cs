using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecColumnCft
    {
        public static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_CFT;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_CFT;
            var setB = secB != null ? new HashSet<StbSecColumn_CFT>(secB) : new HashSet<StbSecColumn_CFT>();

            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name, "floor=" + secColumnA.floor };
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name && n.floor == secColumnA.floor);
                    if (secColumnB != null)
                    {
                        CompareSecColumnCft(stbridgeA, stbridgeB, secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecColumnCft.Compare(nameof(StbSecColumn_CFT), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string>() { "Name=" + b.name, "floor=" + b.floor };
                StbSecColumnCft.Compare(null, nameof(StbSecColumn_CFT), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecColumnCft(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB, StbSecColumn_CFT secColumnA,
            StbSecColumn_CFT secColumnB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecColumnCftId.Compare(secColumnA.id, secColumnB.id, key, records);
            StbSecColumnCftGuid.Compare(secColumnA.guid, secColumnB.guid, key, records);
            StbSecColumnCftName.Compare(secColumnA.name, secColumnB.name, key, records);
            StbSecColumnCftFloor.Compare(secColumnA.floor, secColumnB.floor, key, records);
            StbSecColumnCftKindColumn.Compare(secColumnA.kind_column.ToString(), secColumnB.kind_column.ToString(), key, records);
            StbSecColumnCftStrengthConcrete.Compare(secColumnA.strength_concrete, secColumnB.strength_concrete, key, records);
            StbSecColumnCftIsReferenceDirection.Compare(secColumnA.isReferenceDirection, secColumnB.isReferenceDirection, key, records);

            StbSecSteelFigureColumnCftBaseType.Compare(secColumnA.StbSecSteelFigureColumn_CFT.base_type.ToString(), secColumnB.StbSecSteelFigureColumn_CFT.base_type.ToString(), key, records);

            CompareSecSteelFigureColumnCft(stbridgeA, stbridgeB, secColumnA.StbSecSteelFigureColumn_CFT, secColumnB.StbSecSteelFigureColumn_CFT, key, records);

            if (secColumnA.Item != null && secColumnB.Item != null)
            {
                if (secColumnB.Item == null)
                {
                    if (secColumnA.Item is StbSecBaseProduct_CFT product)
                        StbSecBaseProductCft.Compare(product, null, key, records);
                    else if (secColumnA.Item is StbSecBaseConventional_CFT conventional)
                        StbSecBaseConventionalCft.Compare(conventional, null, key, records);
                }
                else if (secColumnA.Item == null)
                {
                    if (secColumnB.Item is StbSecBaseProduct_CFT product)
                        StbSecBaseProductCft.Compare(null, product, key, records);
                    else if (secColumnB.Item is StbSecBaseConventional_CFT conventional)
                        StbSecBaseConventionalCft.Compare(null, conventional, key, records);
                }
                else if (secColumnA.Item is StbSecBaseProduct_CFT productA)
                {
                    if (secColumnB.Item is StbSecBaseProduct_CFT productB)
                    {
                        StbSecBaseProductCftProductCompany.Compare(productA.product_company, productB.product_company, key, records);
                        StbSecBaseProductCftProductCode.Compare(productA.product_code, productB.product_company, key, records);
                        StbSecBaseProductCftDirectionType.Compare(productA.direction_type.ToString(), productB.direction_type.ToString(), key, records);
                        StbSecBaseProductCftHeightMortar.Compare(productA.height_mortar, productB.height_mortar, key, records);
                    }
                    else
                    {
                        StbSecBaseProductCft.Compare(nameof(StbSecBaseProduct_CFT), nameof(StbSecBaseConventional_CFT), key,
                            records);
                    }
                }
                else if (secColumnA.Item is StbSecBaseConventional_CFT conventionalA)
                {
                    if (secColumnB.Item is StbSecBaseProduct_CFT)
                    {
                        StbSecBaseConventionalCft.Compare(nameof(StbSecBaseConventional_CFT), nameof(StbSecBaseProduct_CFT), key,
                            records);
                    }
                    else if (secColumnA.Item is StbSecBaseConventional_CFT conventionalB)
                    {
                        StbSecBaseConventionalCftHeightMortar.Compare(conventionalA.height_mortar,
                            conventionalB.height_mortar, key, records);

                        CompareBaseConventionalCftPlate(conventionalA, conventionalB, key, records);
                        CompareBaseConventionalCftAnchorBolt(conventionalA, conventionalB, key, records);
                        CompareBaseConventionalCftRibPlate(conventionalA, conventionalB, key, records);
                    }
                }
            }

        }

        private static void CompareBaseConventionalCftRibPlate(StbSecBaseConventional_CFT conventionalA,
            StbSecBaseConventional_CFT conventionalB, IReadOnlyList<string> key, List<Record> records)
        {
            if (conventionalA.StbSecBaseConventional_CFT_RibPlate != null &&
                conventionalB.StbSecBaseConventional_CFT_RibPlate != null)
            {
                var ribA = conventionalA.StbSecBaseConventional_CFT_RibPlate;
                var ribB = conventionalB.StbSecBaseConventional_CFT_RibPlate;
                StbSecBaseConventionalCftRibPlateA1.Compare(ribA.A1, ribB.A1, key, records);
                StbSecBaseConventionalCftRibPlateA2.Compare(ribA.A2, ribB.A2, key, records);
                StbSecBaseConventionalCftRibPlateB1.Compare(ribA.B1, ribB.B1, key, records);
                StbSecBaseConventionalCftRibPlateB2.Compare(ribA.B2, ribB.B2, key, records);
                StbSecBaseConventionalCftRibPlateT.Compare(ribA.t, ribB.t, key, records);
                StbSecBaseConventionalCftRibPlateStrength.Compare(ribA.strength, ribB.strength, key, records);
                StbSecBaseConventionalCftRibPlateNX.Compare(ribA.N_X, ribB.N_X, key, records);
                StbSecBaseConventionalCftRibPlateNY.Compare(ribA.N_Y, ribB.N_Y, key, records);
                StbSecBaseConventionalCftRibPlateLengthEX.Compare(ribA.length_e_X, ribB.length_e_X, key, records);
                StbSecBaseConventionalCftRibPlateLengthEY.Compare(ribA.length_e_Y, ribB.length_e_Y, key, records);
            }
            else
            {
                StbSecBaseConventionalCftRibPlate.Compare(conventionalA.StbSecBaseConventional_CFT_RibPlate,
                    conventionalB.StbSecBaseConventional_CFT_RibPlate, key, records);
            }
        }

        private static void CompareBaseConventionalCftAnchorBolt(StbSecBaseConventional_CFT conventionalA,
            StbSecBaseConventional_CFT conventionalB, IReadOnlyList<string> key, List<Record> records)
        {
            var boltA = conventionalA.StbSecBaseConventional_CFT_AnchorBolt;
            var boltB = conventionalB.StbSecBaseConventional_CFT_AnchorBolt;
            StbSecBaseConventionalCftAnchorBoltKindBolt.Compare(boltA.kind_bolt.ToString(),
                boltB.kind_bolt.ToString(), key, records);
            StbSecBaseConventionalCftAnchorBoltNameBolt.Compare(boltA.name_bolt, boltB.name_bolt, key, records);
            StbSecBaseConventionalCftAnchorBoltLengthBolt.Compare(boltA.length_bolt, boltB.length_bolt, key,
                records);
            StbSecBaseConventionalCftAnchorBoltStrengthBolt.Compare(boltA.strength_bolt, boltB.strength_bolt, key,
                records);
            StbSecBaseConventionalCftAnchorBoltArrangementBolt.Compare(boltA.arrangement_bolt.ToString(),
                boltB.arrangement_bolt.ToString(), key, records);
            StbSecBaseConventionalCftAnchorBoltD1X.Compare(boltA.D1_X, boltB.D1_X, key, records);
            StbSecBaseConventionalCftAnchorBoltD2X.Compare(boltA.D2_X, boltB.D1_Y, key, records);
            StbSecBaseConventionalCftAnchorBoltD1Y.Compare(boltA.D1_Y, boltB.D2_X, key, records);
            StbSecBaseConventionalCftAnchorBoltD2Y.Compare(boltA.D2_Y, boltB.D2_Y, key, records);
            StbSecBaseConventionalCftAnchorBoltNX.Compare(boltA.N_X, boltB.N_X, key, records);
            StbSecBaseConventionalCftAnchorBoltNY.Compare(boltA.N_Y, boltB.N_Y, key, records);
        }

        private static void CompareBaseConventionalCftPlate(StbSecBaseConventional_CFT conventionalA,
            StbSecBaseConventional_CFT conventionalB, IReadOnlyList<string> key, List<Record> records)
        {
            var plateA = conventionalA.StbSecBaseConventional_CFT_Plate;
            var plateB = conventionalB.StbSecBaseConventional_CFT_Plate;
            StbSecBaseConventionalCftPlateBX.Compare(plateA.B_X, plateB.B_X, key, records);
            StbSecBaseConventionalCftPlateBY.Compare(plateA.B_Y, plateB.B_Y, key, records);
            StbSecBaseConventionalCftPlateC1X.Compare(plateA.C1_X, plateB.C1_X, key, records);
            StbSecBaseConventionalCftPlateC1Y.Compare(plateA.C1_Y, plateB.C1_Y, key, records);
            StbSecBaseConventionalCftPlateC2X.Compare(plateA.C2_X, plateB.C2_X, key, records);
            StbSecBaseConventionalCftPlateC2Y.Compare(plateA.C2_Y, plateB.C2_Y, key, records);
            StbSecBaseConventionalCftPlateC3X.Compare(plateA.C3_X, plateB.C3_X, key, records);
            StbSecBaseConventionalCftPlateC3Y.Compare(plateA.C3_Y, plateB.C3_Y, key, records);
            StbSecBaseConventionalCftPlateC4X.Compare(plateA.C4_X, plateB.C4_X, key, records);
            StbSecBaseConventionalCftPlateC4Y.Compare(plateA.C4_Y, plateB.C4_Y, key, records);
            StbSecBaseConventionalCftPlateT.Compare(plateA.t, plateB.t, key, records);
            StbSecBaseConventionalCftPlateStrength.Compare(plateA.strength, plateB.strength, key, records);
            StbSecBaseConventionalCftPlateDBoltHole.Compare(plateA.D_bolthole, plateB.D_bolthole, key, records);
            StbSecBaseConventionalCftPlateOffsetX.Compare(plateA.offset_X, plateB.offset_X, key, records);
            StbSecBaseConventionalCftPlateOffsetY.Compare(plateA.offset_Y, plateB.offset_Y, key, records);
        }

        private static void CompareSecSteelFigureColumnCft(
            ST_BRIDGE stbridgeA,
            ST_BRIDGE stbridgeB,
            StbSecSteelFigureColumn_CFT secColumnA,
            StbSecSteelFigureColumn_CFT secColumnB,
            IReadOnlyList<string> key,
            List<Record> records)
        {
            if (secColumnA.Items.Any(n => n is StbSecSteelColumn_CFT_Same))
            {
                if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_Same))
                {
                    var sameA = secColumnA.Items.OfType<StbSecSteelColumn_CFT_Same>().First();
                    var sameB = secColumnB.Items.OfType<StbSecSteelColumn_CFT_Same>().First();
                    StbSecSteelColumnCftSameShape.Compare(sameA.shape, stbridgeA, sameB.shape, stbridgeB, key, records);
                    StbSecSteelColumnCftSameStrength.Compare(sameA.strength, sameB.strength, key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_NotSame))
                {
                    StbSecSteelColumnCftSame.Compare(nameof(StbSecSteelColumn_CFT_Same), nameof(StbSecSteelColumn_CFT_NotSame), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_ThreeTypes))
                {
                    StbSecSteelColumnCftSame.Compare(nameof(StbSecSteelColumn_CFT_Same), nameof(StbSecSteelColumn_CFT_ThreeTypes), key, records);
                }

            }
            else if (secColumnA.Items.Any(n => n is StbSecSteelColumn_CFT_NotSame))
            {
                if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_Same))
                {
                    StbSecSteelColumnCftNotSame.Compare(nameof(StbSecSteelColumn_CFT_NotSame), nameof(StbSecSteelColumn_CFT_Same), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_NotSame))
                {
                    var set = new HashSet<StbSecSteelColumn_CFT_NotSame>();
                    foreach (var a in secColumnA.Items.OfType<StbSecSteelColumn_CFT_NotSame>())
                    {
                        List<string> key2 = new List<string>(key) { "pos=" + a.pos.ToString() };
                        var notSameB = secColumnB.Items.OfType<StbSecSteelColumn_CFT_NotSame>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (notSameB != null)
                        {
                            StbSecSteelColumnCftNotSamePos.Compare(a.pos.ToString(), notSameB.pos.ToString(), key2, records);
                            StbSecSteelColumnCftNotSameShape.Compare(a.shape, stbridgeA, notSameB.shape,
                                stbridgeB, key2, records);
                            StbSecSteelColumnCftNotSameStrength.Compare(a.strength, notSameB.strength, key2, records);
                            set.Add(notSameB);
                        }
                        else
                        {
                            StbSecSteelColumnCftNotSame.Compare(a, null, key2, records);
                        }

                    }

                    foreach (var b in secColumnB.Items.OfType<StbSecSteelColumn_CFT_NotSame>())
                    {
                        if (!set.Contains(b))
                        {
                            List<string> key2 = new List<string>(key) { "pos=" + b.pos };
                            StbSecSteelColumnCftNotSame.Compare(null, b, key2, records);
                        }
                    }
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_ThreeTypes))
                {
                    StbSecSteelColumnCftNotSame.Compare(nameof(StbSecSteelColumn_CFT_NotSame), nameof(StbSecSteelColumn_CFT_ThreeTypes), key, records);
                }
            }
            else if (secColumnA.Items.Any(n => n is StbSecSteelColumn_CFT_ThreeTypes))
            {
                if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_Same))
                {
                    StbSecSteelColumnCftNotSame.Compare(nameof(StbSecSteelColumn_CFT_ThreeTypes),
                        nameof(StbSecSteelColumn_CFT_Same), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_NotSame))
                {
                    StbSecSteelColumnCftSame.Compare(nameof(StbSecSteelColumn_CFT_ThreeTypes),
                        nameof(StbSecSteelColumn_CFT_NotSame), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_CFT_ThreeTypes))
                {
                    var set = new HashSet<StbSecSteelColumn_CFT_ThreeTypes>();

                    foreach (var a in secColumnA.Items.OfType<StbSecSteelColumn_CFT_ThreeTypes>())
                    {
                        List<string> key2 = new List<string>(key) { "pos=" + a.pos.ToString() };
                        var threeTypesB = secColumnB.Items.OfType<StbSecSteelColumn_CFT_ThreeTypes>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (threeTypesB != null)
                        {
                            StbSecSteelColumnCftThreeTypesPos.Compare(a.pos.ToString(),
                                threeTypesB.pos.ToString(), key2, records);
                            StbSecSteelColumnCftThreeTypesShape.Compare(a.shape, stbridgeA,
                                threeTypesB.shape, stbridgeB, key2, records);
                            StbSecSteelColumnCftThreeTypesStrength.Compare(a.strength,
                                threeTypesB.strength, key2, records);
                            set.Add(threeTypesB);
                        }
                        else
                        {
                            StbSecSteelColumnCftThreeTypes.Compare(a, null, key2, records);
                        }
                    }

                    foreach (var b in secColumnA.Items.OfType<StbSecSteelColumn_CFT_ThreeTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            List<string> key2 = new List<string>(key) { "pos=" + b.pos };
                            StbSecSteelColumnCftThreeTypes.Compare(null, b, key2, records);
                        }
                    }
                }

            }
        }
    }
}

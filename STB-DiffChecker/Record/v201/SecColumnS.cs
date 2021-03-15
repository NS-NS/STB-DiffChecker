using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecColumnS
    {
        public static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stbridgeA?.StbModel?.StbSections?.StbSecColumn_S;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecColumn_S;
            var setB = secB != null ? new HashSet<StbSecColumn_S>(secB) : new HashSet<StbSecColumn_S>();

            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name, "floor=" + secColumnA.floor };
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name && n.floor == secColumnA.floor);
                    if (secColumnB != null)
                    {
                        CompareSecColumnS(stbridgeA, stbridgeB, secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecColumnS.Compare(nameof(StbSecColumn_S), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string>() { "Name=" + b.name, "floor=" + b.floor };
                StbSecColumnS.Compare(null, nameof(StbSecColumn_S), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecColumnS(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB, StbSecColumn_S secColumnA,
            StbSecColumn_S secColumnB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecColumnSId.Compare(secColumnA.id, secColumnB.id, key, records);
            StbSecColumnSGuid.Compare(secColumnA.guid, secColumnB.guid, key, records);
            StbSecColumnSName.Compare(secColumnA.name, secColumnB.name, key, records);
            StbSecColumnSFloor.Compare(secColumnA.floor, secColumnB.floor, key, records);
            StbSecColumnSKindColumn.Compare(secColumnA.kind_column.ToString(), secColumnB.kind_column.ToString(), key, records);
            StbSecColumnSIsReferenceDirection.Compare(secColumnA.isReferenceDirection, secColumnA.isReferenceDirection, key, records);
            StbSecSteelFigureColumnSBaseType.Compare(secColumnA.StbSecSteelFigureColumn_S.base_type.ToString(), secColumnB.StbSecSteelFigureColumn_S.base_type.ToString(), key, records);

            CompareSecSteelFigureColumnS(stbridgeA, stbridgeB, secColumnA.StbSecSteelFigureColumn_S, secColumnB.StbSecSteelFigureColumn_S, key, records);

            if (secColumnA.Item != null && secColumnB.Item != null)
            {
                if (secColumnB.Item == null)
                {
                    if (secColumnA.Item is StbSecBaseProduct_S product)
                        StbSecBaseProductS.Compare(product, null, key, records);
                    else if (secColumnA.Item is StbSecBaseConventional_S conventional)
                        StbSecBaseConventionalS.Compare(conventional, null, key, records);
                }
                else if (secColumnA.Item == null)
                {
                    if (secColumnB.Item is StbSecBaseProduct_S product)
                        StbSecBaseProductS.Compare(null, product, key, records);
                    else if (secColumnB.Item is StbSecBaseConventional_S conventional)
                        StbSecBaseConventionalS.Compare(null, conventional, key, records);
                }
                else if (secColumnA.Item is StbSecBaseProduct_S productA)
                {
                    if (secColumnB.Item is StbSecBaseProduct_S productB)
                    {
                        StbSecBaseProductSProductCompany.Compare(productA.product_company, productB.product_company, key, records);
                        StbSecBaseProductSProductCode.Compare(productA.product_code, productB.product_company, key, records);
                        StbSecBaseProductSDirectionType.Compare(productA.direction_type.ToString(), productB.direction_type.ToString(), key, records);
                        StbSecBaseProductSHeightMortar.Compare(productA.height_mortar, productB.height_mortar, key, records);
                    }
                    else
                    {
                        StbSecBaseProductS.Compare(nameof(StbSecBaseProduct_S), nameof(StbSecBaseConventional_S), key,
                            records);
                    }
                }
                else if (secColumnA.Item is StbSecBaseConventional_S conventionalA)
                {
                    if (secColumnB.Item is StbSecBaseProduct_S)
                    {
                        StbSecBaseConventionalS.Compare(nameof(StbSecBaseConventional_S), nameof(StbSecBaseProduct_S), key,
                            records);
                    }
                    else if (secColumnA.Item is StbSecBaseConventional_S conventionalB)
                    {
                        StbSecBaseConventionalSHeightMortar.Compare(conventionalA.height_mortar,
                            conventionalB.height_mortar, key, records);

                        CompareBaseConventionalSPlate(conventionalA, conventionalB, key, records);
                        CompareBaseConventionalSAnchorBolt(conventionalA, conventionalB, key, records);
                        CompareBaseConventionalSRibPlate(conventionalA, conventionalB, key, records);
                    }
                }
            }

        }

        private static void CompareBaseConventionalSRibPlate(StbSecBaseConventional_S conventionalA,
            StbSecBaseConventional_S conventionalB, IReadOnlyList<string> key, List<Record> records)
        {
            if (conventionalA.StbSecBaseConventional_S_RibPlate != null &&
                conventionalB.StbSecBaseConventional_S_RibPlate != null)
            {
                var ribA = conventionalA.StbSecBaseConventional_S_RibPlate;
                var ribB = conventionalB.StbSecBaseConventional_S_RibPlate;
                StbSecBaseConventionalSRibPlateA1.Compare(ribA.A1, ribB.A1, key, records);
                StbSecBaseConventionalSRibPlateA2.Compare(ribA.A2, ribB.A2, key, records);
                StbSecBaseConventionalSRibPlateB1.Compare(ribA.B1, ribB.B1, key, records);
                StbSecBaseConventionalSRibPlateB2.Compare(ribA.B2, ribB.B2, key, records);
                StbSecBaseConventionalSRibPlateT.Compare(ribA.t, ribB.t, key, records);
                StbSecBaseConventionalSRibPlateStrength.Compare(ribA.strength, ribB.strength, key, records);
                StbSecBaseConventionalSRibPlateNX.Compare(ribA.N_X, ribB.N_X, key, records);
                StbSecBaseConventionalSRibPlateNY.Compare(ribA.N_Y, ribB.N_Y, key, records);
                StbSecBaseConventionalSRibPlateLengthEX.Compare(ribA.length_e_X, ribB.length_e_X, key, records);
                StbSecBaseConventionalSRibPlateLengthEY.Compare(ribA.length_e_Y, ribB.length_e_Y, key, records);
            }
            else
            {
                StbSecBaseConventionalSRibPlate.Compare(conventionalA.StbSecBaseConventional_S_RibPlate,
                    conventionalB.StbSecBaseConventional_S_RibPlate, key, records);
            }
        }

        private static void CompareBaseConventionalSAnchorBolt(StbSecBaseConventional_S conventionalA,
            StbSecBaseConventional_S conventionalB, IReadOnlyList<string> key, List<Record> records)
        {
            var boltA = conventionalA.StbSecBaseConventional_S_AnchorBolt;
            var boltB = conventionalB.StbSecBaseConventional_S_AnchorBolt;
            StbSecBaseConventionalSAnchorBoltKindBolt.Compare(boltA.kind_bolt.ToString(),
                boltB.kind_bolt.ToString(), key, records);
            StbSecBaseConventionalSAnchorBoltNameBolt.Compare(boltA.name_bolt, boltB.name_bolt, key, records);
            StbSecBaseConventionalSAnchorBoltLengthBolt.Compare(boltA.length_bolt, boltB.length_bolt, key,
                records);
            StbSecBaseConventionalSAnchorBoltStrengthBolt.Compare(boltA.strength_bolt, boltB.strength_bolt, key,
                records);
            StbSecBaseConventionalSAnchorBoltArrangementBolt.Compare(boltA.arrangement_bolt.ToString(),
                boltB.arrangement_bolt.ToString(), key, records);
            StbSecBaseConventionalSAnchorBoltD1X.Compare(boltA.D1_X, boltB.D1_X, key, records);
            StbSecBaseConventionalSAnchorBoltD2X.Compare(boltA.D2_X, boltB.D1_Y, key, records);
            StbSecBaseConventionalSAnchorBoltD1Y.Compare(boltA.D1_Y, boltB.D2_X, key, records);
            StbSecBaseConventionalSAnchorBoltD2Y.Compare(boltA.D2_Y, boltB.D2_Y, key, records);
            StbSecBaseConventionalSAnchorBoltNX.Compare(boltA.N_X, boltB.N_X, key, records);
            StbSecBaseConventionalSAnchorBoltNY.Compare(boltA.N_Y, boltB.N_Y, key, records);
        }

        private static void CompareBaseConventionalSPlate(StbSecBaseConventional_S conventionalA,
            StbSecBaseConventional_S conventionalB, IReadOnlyList<string> key, List<Record> records)
        {
            var plateA = conventionalA.StbSecBaseConventional_S_Plate;
            var plateB = conventionalB.StbSecBaseConventional_S_Plate;
            StbSecBaseConventionalSPlateBX.Compare(plateA.B_X, plateB.B_X, key, records);
            StbSecBaseConventionalSPlateBY.Compare(plateA.B_Y, plateB.B_Y, key, records);
            StbSecBaseConventionalSPlateC1X.Compare(plateA.C1_X, plateB.C1_X, key, records);
            StbSecBaseConventionalSPlateC1Y.Compare(plateA.C1_Y, plateB.C1_Y, key, records);
            StbSecBaseConventionalSPlateC2X.Compare(plateA.C2_X, plateB.C2_X, key, records);
            StbSecBaseConventionalSPlateC2Y.Compare(plateA.C2_Y, plateB.C2_Y, key, records);
            StbSecBaseConventionalSPlateC3X.Compare(plateA.C3_X, plateB.C3_X, key, records);
            StbSecBaseConventionalSPlateC3Y.Compare(plateA.C3_Y, plateB.C3_Y, key, records);
            StbSecBaseConventionalSPlateC4X.Compare(plateA.C4_X, plateB.C4_X, key, records);
            StbSecBaseConventionalSPlateC4Y.Compare(plateA.C4_Y, plateB.C4_Y, key, records);
            StbSecBaseConventionalSPlateT.Compare(plateA.t, plateB.t, key, records);
            StbSecBaseConventionalSPlateStrength.Compare(plateA.strength, plateB.strength, key, records);
            StbSecBaseConventionalSPlateDBoltHole.Compare(plateA.D_bolthole, plateB.D_bolthole, key, records);
            StbSecBaseConventionalSPlateOffsetX.Compare(plateA.offset_X, plateB.offset_X, key, records);
            StbSecBaseConventionalSPlateOffsetY.Compare(plateA.offset_Y, plateB.offset_Y, key, records);
        }

        private static void CompareSecSteelFigureColumnS(
            ST_BRIDGE stbridgeA,
            ST_BRIDGE stbridgeB,
            StbSecSteelFigureColumn_S secColumnA,
            StbSecSteelFigureColumn_S secColumnB,
            IReadOnlyList<string> key,
            List<Record> records)
        {
            StbSecSteelFigureColumnSJointIdTop.Compare(secColumnA.joint_id_top, stbridgeA,
                secColumnB.joint_id_top, stbridgeB, key, records);
            StbSecSteelFigureColumnSJointIdBottom.Compare(secColumnA.joint_id_bottom,
                stbridgeA, secColumnB.joint_id_bottom, stbridgeB, key, records);


            if (secColumnA.Items.Any(n => n is StbSecSteelColumn_S_Same))
            {
                if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_Same))
                {
                    var sameA = secColumnA.Items.OfType<StbSecSteelColumn_S_Same>().First();
                    var sameB = secColumnB.Items.OfType<StbSecSteelColumn_S_Same>().First();
                    StbSecSteelColumnSSameShape.Compare(sameA.shape, stbridgeA, sameB.shape, stbridgeB, key, records);
                    StbSecSteelColumnSSameStrengthMain.Compare(sameA.strength_main, sameB.strength_main, key, records);
                    StbSecSteelColumnSSameStrengthWeb.Compare(sameA.strength_web, sameB.strength_web, key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_NotSame))
                {
                    StbSecSteelColumnSSame.Compare(nameof(StbSecSteelColumn_S_Same), nameof(StbSecSteelColumn_S_NotSame), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_ThreeTypes))
                {
                    StbSecSteelColumnSSame.Compare(nameof(StbSecSteelColumn_S_Same), nameof(StbSecSteelColumn_S_ThreeTypes), key, records);
                }

            }
            else if (secColumnA.Items.Any(n => n is StbSecSteelColumn_S_NotSame))
            {
                if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_Same))
                {
                    StbSecSteelColumnSNotSame.Compare(nameof(StbSecSteelColumn_S_NotSame), nameof(StbSecSteelColumn_S_Same), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_NotSame))
                {
                    var set = new HashSet<StbSecSteelColumn_S_NotSame>();
                    foreach (var a in secColumnA.Items.OfType<StbSecSteelColumn_S_NotSame>())
                    {
                        List<string> key2 = new List<string>(key) {"pos=" + a.pos.ToString()};
                        var notSameB = secColumnB.Items.OfType<StbSecSteelColumn_S_NotSame>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (notSameB != null)
                        {
                            StbSecSteelColumnSNotSamePos.Compare(a.pos.ToString(), notSameB.pos.ToString(), key2, records);
                            StbSecSteelColumnSNotSameShape.Compare(a.shape, stbridgeA, notSameB.shape,
                                stbridgeB, key2, records);
                            StbSecSteelColumnSNotSameStrengthMain.Compare(a.strength_main, notSameB.strength_main, key2, records);
                            StbSecSteelColumnSNotSameStrengthWeb.Compare(a.strength_web, notSameB.strength_web, key2, records);
                            set.Add(notSameB);
                        }
                        else
                        {
                            StbSecSteelColumnSNotSame.Compare(a, null, key2, records);
                        }

                    }

                    foreach (var b in secColumnB.Items.OfType<StbSecSteelColumn_S_NotSame>())
                    {
                        if (!set.Contains(b))
                        {
                            List<string> key2 = new List<string>(key) {"pos=" + b.pos};
                            StbSecSteelColumnSNotSame.Compare(null, b, key2, records);
                        }
                    }
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_ThreeTypes))
                {
                    StbSecSteelColumnSNotSame.Compare(nameof(StbSecSteelColumn_S_NotSame), nameof(StbSecSteelColumn_S_ThreeTypes), key, records);
                }
            }
            else if (secColumnA.Items.Any(n => n is StbSecSteelColumn_S_ThreeTypes))
            {
                if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_Same))
                {
                    StbSecSteelColumnSNotSame.Compare(nameof(StbSecSteelColumn_S_ThreeTypes),
                        nameof(StbSecSteelColumn_S_Same), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_NotSame))
                {
                    StbSecSteelColumnSSame.Compare(nameof(StbSecSteelColumn_S_ThreeTypes),
                        nameof(StbSecSteelColumn_S_NotSame), key, records);
                }
                else if (secColumnB.Items.Any(n => n is StbSecSteelColumn_S_ThreeTypes))
                {
                    var set = new HashSet<StbSecSteelColumn_S_ThreeTypes>();

                    foreach (var a in secColumnA.Items.OfType<StbSecSteelColumn_S_ThreeTypes>())
                    {
                        List<string> key2 = new List<string>(key) {"pos=" + a.pos.ToString()};
                        var threeTypesB = secColumnB.Items.OfType<StbSecSteelColumn_S_ThreeTypes>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (threeTypesB != null)
                        {
                            StbSecSteelColumnSThreeTypesPos.Compare(a.pos.ToString(),
                                threeTypesB.pos.ToString(), key2, records);
                            StbSecSteelColumnSThreeTypesShape.Compare(a.shape, stbridgeA,
                                threeTypesB.shape, stbridgeB, key2, records);
                            StbSecSteelColumnSThreeTypesStrengthMain.Compare(a.strength_main,
                                threeTypesB.strength_main, key2, records);
                            StbSecSteelColumnSThreeTypesStrengthWeb.Compare(a.strength_web,
                                threeTypesB.strength_web, key2, records);
                            set.Add(threeTypesB);
                        }
                        else
                        {
                            StbSecSteelColumnSThreeTypes.Compare(a, null, key2, records);
                        }
                    }

                    foreach (var b in secColumnA.Items.OfType<StbSecSteelColumn_S_ThreeTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            List<string> key2 = new List<string>(key) {"pos=" + b.pos};
                            StbSecSteelColumnSThreeTypes.Compare(null, b, key2, records);
                        }
                    }
                }

            }
        }
    }
}

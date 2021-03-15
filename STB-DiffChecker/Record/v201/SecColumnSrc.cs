using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecColumnSrc
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stBridgeA?.StbModel?.StbSections?.StbSecColumn_SRC;
            var secB = stBridgeB?.StbModel?.StbSections?.StbSecColumn_SRC;
            var setB = secB != null ? new HashSet<StbSecColumn_SRC>(secB) : new HashSet<StbSecColumn_SRC>();


            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name, "floor=" + secColumnA.floor };
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name && n.floor == secColumnA.floor);
                    if (secColumnB != null)
                    {
                        CompareSecColumnSrc(stBridgeA, stBridgeB, secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecColumnRc.Compare(nameof(StbSecColumn_SRC), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { "Name=" + b.name, "floor=" + b.floor };
                StbSecColumnRc.Compare(null, nameof(StbSecColumn_SRC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecColumnSrc(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecColumn_SRC secA, StbSecColumn_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecColumnSrcId.Compare(secA.id, secB.id, key, records);
            StbSecColumnSrcGuid.Compare(secA.guid, secB.guid, key, records);
            StbSecColumnSrcName.Compare(secA.name, secB.name, key, records);
            StbSecColumnSrcFloor.Compare(secA.floor, secB.floor, key, records);
            StbSecColumnSrcKindColumn.Compare(secA.kind_column.ToString(), secB.kind_column.ToString(), key, records);
            StbSecColumnSrcStrengthConcrete.Compare(secA.strength_concrete, secB.strength_concrete, key, records);

            CompareSecFigureColumnSrc(secA.StbSecFigureColumn_SRC, secB.StbSecFigureColumn_SRC, key, records);
            if (secA.StbSecBarArrangementColumn_SRC != null || secB.StbSecBarArrangementColumn_SRC != null)
            {
                if (secB.StbSecBarArrangementColumn_SRC == null)
                {
                    StbSecBarArrangementColumnSrc.Compare(nameof(StbSecBarArrangementColumn_SRC), null, key, records);
                }
                else if (secA.StbSecBarArrangementColumn_SRC == null)
                {
                    StbSecBarArrangementColumnSrc.Compare(null, nameof(StbSecBarArrangementColumn_SRC), key, records);
                }
                else
                {
                    CompareSecBarArrangementColumnSrc(secA.StbSecBarArrangementColumn_SRC,
                        secB.StbSecBarArrangementColumn_SRC, key, records);
                }
            }

            if (secA.StbSecSteelFigureColumn_SRC != null || secB.StbSecSteelFigureColumn_SRC != null)
            {
                if (secB.StbSecSteelFigureColumn_SRC == null)
                {
                    StbSecSteelFigureColumnSrc.Compare(nameof(StbSecSteelFigureColumn_SRC), null, key, records);
                }
                else if (secA.StbSecSteelFigureColumn_SRC == null)
                {
                    StbSecSteelFigureColumnSrc.Compare(null, nameof(StbSecSteelFigureColumn_SRC), key, records);
                }
                else
                {
                    CompareSecSteelFigureColumnSrc(stBridgeA, stBridgeB, secA.StbSecSteelFigureColumn_SRC,
                        secB.StbSecSteelFigureColumn_SRC, key, records);
                }
            }

            if (secA.Item != null)
            {
                if (secA.Item is StbSecBaseProduct_SRC productA)
                {
                    if (secB.Item == null)
                    {
                        StbSecBaseProductSrc.Compare(nameof(StbSecBaseProduct_SRC), null, key, records);
                    }
                    else if (secB.Item is StbSecBaseProduct_SRC productB)
                    {
                        StbSecBaseProductSrcProductCompany.Compare(productA.product_company, productB.product_company, key, records);
                        StbSecBaseProductSrcProductCode.Compare(productA.product_code, productB.product_code, key, records);
                        StbSecBaseProductSrcDirectionType.Compare(productA.direction_type.ToString(), productB.direction_type.ToString(), key, records);
                        StbSecBaseProductSrcHeightMortar.Compare(productA.height_mortar, productB.height_mortar, key, records);
                    }
                    else if (secB.Item is StbSecBaseConventional_SRC)
                    {
                        StbSecBaseProductSrc.Compare(nameof(StbSecBaseProduct_SRC), nameof(StbSecBaseConventional_SRC), key, records);
                    }
                }
                else if (secA.Item is StbSecBaseConventional_SRC conventionalA)
                {
                    if (secB.Item == null)
                    {
                        StbSecBaseConventionalSrc.Compare(nameof(StbSecBaseConventional_SRC), null, key, records);
                    }
                    else if (secB.Item is StbSecBaseProduct_SRC)
                    {
                        StbSecBaseConventionalSrc.Compare(nameof(StbSecBaseConventional_SRC), nameof(StbSecBaseProduct_SRC), key, records);
                    }
                    else if (secB.Item is StbSecBaseConventional_SRC conventionalB)
                    {
                        StbSecBaseProductSrcHeightMortar.Compare(conventionalA.height_mortar, conventionalB.height_mortar, key, records);
                        CompareStbSecBaseConventionalSrcPlate(conventionalA.StbSecBaseConventional_SRC_Plate,
                            conventionalB.StbSecBaseConventional_SRC_Plate, key, records);
                        CompareStbSecBaseConventionalSrcAnchorBolt(conventionalA.StbSecBaseConventional_SRC_AnchorBolt,
                            conventionalB.StbSecBaseConventional_SRC_AnchorBolt, key, records);
                        if (conventionalA.StbSecBaseConventional_SRC_RibPlate != null ||
                            conventionalB.StbSecBaseConventional_SRC_RibPlate != null)
                        {
                            if (conventionalB.StbSecBaseConventional_SRC_RibPlate == null)
                            {
                                StbSecBaseConventionalSrcRibPlate.Compare(nameof(StbSecBaseConventional_SRC_RibPlate), null, key, records);
                            }
                            else if (conventionalA.StbSecBaseConventional_SRC_RibPlate == null)
                            {
                                StbSecBaseConventionalSrcRibPlate.Compare(null, nameof(StbSecBaseConventional_SRC_RibPlate), key, records);
                            }
                            else
                            {
                                CompareStbSecBaseConventionalSrcRibPlate(
                                    conventionalA.StbSecBaseConventional_SRC_RibPlate,
                                    conventionalB.StbSecBaseConventional_SRC_RibPlate, key, records);
                            }
                        }
                    }
                }
            }
            else
            {
                if (secB.Item != null)
                {
                    if (secB.Item is StbSecBaseProduct_SRC)
                    {
                        StbSecBaseProductSrc.Compare(null, nameof(StbSecBaseProduct_SRC), key, records);
                    }
                    else
                    {
                        StbSecBaseConventionalSrc.Compare(null, nameof(StbSecBaseConventional_SRC), key, records);
                    }
                }
            }

        }

        private static void CompareStbSecBaseConventionalSrcRibPlate(StbSecBaseConventional_SRC_RibPlate ribA, StbSecBaseConventional_SRC_RibPlate ribB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBaseConventionalSrcRibPlateA1.Compare(ribA.A1, ribB.A1, key, records);
            StbSecBaseConventionalSrcRibPlateA2.Compare(ribA.A2, ribB.A2, key, records);
            StbSecBaseConventionalSrcRibPlateB1.Compare(ribA.B1, ribB.B1, key, records);
            StbSecBaseConventionalSrcRibPlateB2.Compare(ribA.B2, ribB.B2, key, records);
            StbSecBaseConventionalSrcRibPlateT.Compare(ribA.t, ribB.t, key, records);
            StbSecBaseConventionalSrcRibPlateStrength.Compare(ribA.strength, ribB.strength, key, records);
            StbSecBaseConventionalSrcRibPlateNX.Compare(ribA.N_X, ribB.N_X, key, records);
            StbSecBaseConventionalSrcRibPlateNY.Compare(ribA.N_Y, ribB.N_Y, key, records);
            StbSecBaseConventionalSrcRibPlateLengthEX.Compare(ribA.length_e_X, ribB.length_e_X, key, records);
            StbSecBaseConventionalSrcRibPlateLengthEY.Compare(ribA.length_e_Y, ribB.length_e_Y, key, records);
        }

        private static void CompareStbSecBaseConventionalSrcAnchorBolt(StbSecBaseConventional_SRC_AnchorBolt boltA, StbSecBaseConventional_SRC_AnchorBolt boltB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBaseConventionalSrcAnchorBoltKindBolt.Compare(boltA.kind_bolt.ToString(),
                boltB.kind_bolt.ToString(), key, records);
            StbSecBaseConventionalSrcAnchorBoltNameBolt.Compare(boltA.name_bolt, boltB.name_bolt, key, records);
            StbSecBaseConventionalSrcAnchorBoltLengthBolt.Compare(boltA.length_bolt, boltB.length_bolt, key, records);
            StbSecBaseConventionalSrcAnchorBoltStrengthBolt.Compare(boltA.strength_bolt, boltB.strength_bolt, key, records);
            StbSecBaseConventionalSrcAnchorBoltArrangementBolt.Compare(boltA.arrangement_bolt.ToString(),
                boltB.arrangement_bolt.ToString(), key, records);
            StbSecBaseConventionalSrcAnchorBoltD1X.Compare(boltA.D1_X, boltB.D1_X, key, records);
            StbSecBaseConventionalSrcAnchorBoltD2X.Compare(boltA.D2_X, boltB.D1_Y, key, records);
            StbSecBaseConventionalSrcAnchorBoltD1Y.Compare(boltA.D1_Y, boltB.D2_X, key, records);
            StbSecBaseConventionalSrcAnchorBoltD2Y.Compare(boltA.D2_Y, boltB.D2_Y, key, records);
            StbSecBaseConventionalSrcAnchorBoltNX.Compare(boltA.N_X, boltB.N_X, key, records);
            StbSecBaseConventionalSrcAnchorBoltNY.Compare(boltA.N_Y, boltB.N_Y, key, records);
        }

        private static void CompareStbSecBaseConventionalSrcPlate(StbSecBaseConventional_SRC_Plate plateA, StbSecBaseConventional_SRC_Plate plateB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBaseConventionalSrcPlateBX.Compare(plateA.B_X, plateB.B_X, key, records);
            StbSecBaseConventionalSrcPlateBY.Compare(plateA.B_Y, plateB.B_Y, key, records);
            StbSecBaseConventionalSrcPlateC1X.Compare(plateA.C1_X, plateB.C1_X, key, records);
            StbSecBaseConventionalSrcPlateC1Y.Compare(plateA.C1_Y, plateB.C1_Y, key, records);
            StbSecBaseConventionalSrcPlateC2X.Compare(plateA.C2_X, plateB.C2_X, key, records);
            StbSecBaseConventionalSrcPlateC2Y.Compare(plateA.C2_Y, plateB.C2_Y, key, records);
            StbSecBaseConventionalSrcPlateC3X.Compare(plateA.C3_X, plateB.C3_X, key, records);
            StbSecBaseConventionalSrcPlateC3Y.Compare(plateA.C3_Y, plateB.C3_Y, key, records);
            StbSecBaseConventionalSrcPlateC4X.Compare(plateA.C4_X, plateB.C4_X, key, records);
            StbSecBaseConventionalSrcPlateC4Y.Compare(plateA.C4_Y, plateB.C4_Y, key, records);
            StbSecBaseConventionalSrcPlateT.Compare(plateA.t, plateB.t, key, records);
            StbSecBaseConventionalSrcPlateStrength.Compare(plateA.strength, plateB.strength, key, records);
            StbSecBaseConventionalSrcPlateDBoltHole.Compare(plateA.D_bolthole, plateB.D_bolthole, key, records);
            StbSecBaseConventionalSrcPlateOffsetX.Compare(plateA.offset_X, plateB.offset_X, key, records);
            StbSecBaseConventionalSrcPlateOffsetY.Compare(plateA.offset_Y, plateB.offset_Y, key, records);
        }

        private static void CompareSecSteelFigureColumnSrc(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecSteelFigureColumn_SRC secA, StbSecSteelFigureColumn_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecSteelFigureColumnSrcBaseType.Compare(secA.base_type.ToString(), secB.base_type.ToString(), key, records);
            StbSecSteelFigureColumnSrcJointIdTop.Compare(secA.joint_id_top, stBridgeA, secB.joint_id_top, stBridgeB, key, records);
            StbSecSteelFigureColumnSrcJointIdBottom.Compare(secA.joint_id_bottom, stBridgeA,secB.joint_id_bottom, stBridgeB, key, records);
            StbSecSteelFigureColumnSrcLengthEmbedded.Compare(secA.length_embeddedSpecified, secA.length_embedded,
                secB.length_embeddedSpecified, secB.length_embedded, key, records);

            if (secA.Items.Any(n => n is StbSecSteelColumn_SRC_Same))
            {
                if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_Same))
                {
                    var a = secA.Items.OfType<StbSecSteelColumn_SRC_Same>().First();
                    var b = secB.Items.OfType<StbSecSteelColumn_SRC_Same>().First();
                    CompareStbSecSteelColumnSrcSame(a, stBridgeA, b, stBridgeB, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_NotSame))
                {
                    StbSecSteelColumnSrcSame.Compare(nameof(StbSecSteelColumn_SRC_Same), nameof(StbSecSteelColumn_SRC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_ThreeTypes))
                {
                    StbSecSteelColumnSrcSame.Compare(nameof(StbSecSteelColumn_SRC_Same), nameof(StbSecSteelColumn_SRC_ThreeTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelColumn_SRC_NotSame))
            {
                if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_Same))
                {
                    StbSecSteelColumnSrcNotSame.Compare(nameof(StbSecSteelColumn_SRC_NotSame), nameof(StbSecSteelColumn_SRC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_NotSame))
                {
                    var setB = new HashSet<StbSecSteelColumn_SRC_NotSame>();
                    foreach (var a in secA.Items.OfType<StbSecSteelColumn_SRC_NotSame>())
                    {
                        var key2 = new List<string>(key) {"pos=" + a.pos};
                        bool hasItem = false;
                        foreach (var b in secB.Items.OfType<StbSecSteelColumn_SRC_NotSame>())
                        {
                            if (b.pos == a.pos)
                            {
                                StbSecSteelColumnSrcNotSamePos.Compare(a.pos, b.pos, key2, records);
                                CompareStbSecSteelColumnSrcNotSame(a, stBridgeA, b, stBridgeB, key2, records);
                                setB.Add(b);
                                hasItem = true;
                            }
                        }

                        if (!hasItem)
                        {
                            StbSecSteelColumnSrcNotSame.Compare(nameof(StbSecSteelColumn_SRC_NotSame), null, key2, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelColumn_SRC_NotSame>())
                    {
                        if (!setB.Contains(b))
                        {
                            var key2 = new List<string>(key) { "pos=" + b.pos };
                            StbSecSteelColumnSrcNotSame.Compare(null, nameof(StbSecSteelColumn_SRC_NotSame), key2, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_ThreeTypes))
                {
                    StbSecSteelColumnSrcNotSame.Compare(nameof(StbSecSteelColumn_SRC_NotSame), nameof(StbSecSteelColumn_SRC_ThreeTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelColumn_SRC_ThreeTypes))
            {
                if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_Same))
                {
                    StbSecSteelColumnSrcThreeTypes.Compare(nameof(StbSecSteelColumn_SRC_ThreeTypes), nameof(StbSecSteelColumn_SRC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_NotSame))
                {
                    StbSecSteelColumnSrcThreeTypes.Compare(nameof(StbSecSteelColumn_SRC_ThreeTypes), nameof(StbSecSteelColumn_SRC_NotSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelColumn_SRC_ThreeTypes))
                {
                    var setB = new HashSet<StbSecSteelColumn_SRC_ThreeTypes>();
                    foreach (var a in secA.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>())
                    {
                        var key2 = new List<string>(key) { "pos=" + a.pos };
                        bool hasItem = false;
                        foreach (var b in secB.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>())
                        {
                            if (b.pos == a.pos)
                            {
                                StbSecSteelColumnSrcNotSamePos.Compare(a.pos, b.pos, key2, records);
                                CompareStbSecSteelColumnSrcThreeTypes(a, stBridgeA, b, stBridgeB, key2, records);
                                setB.Add(b);
                                hasItem = true;
                            }
                        }

                        if (!hasItem)
                        {
                            StbSecSteelColumnSrcNotSame.Compare(nameof(StbSecSteelColumn_SRC_ThreeTypes), null, key2, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>())
                    {
                        if (!setB.Contains(b))
                        {
                            var key2 = new List<string>(key) { "pos=" + b.pos };
                            StbSecSteelColumnSrcNotSame.Compare(null, nameof(StbSecSteelColumn_SRC_ThreeTypes), key2, records);
                        }
                    }
                }
            }
        }

        private static void CompareStbSecSteelColumnSrcThreeTypes(StbSecSteelColumn_SRC_ThreeTypes secA, ST_BRIDGE stBridgeA, StbSecSteelColumn_SRC_ThreeTypes secB, ST_BRIDGE stBridgeB, List<string> key, List<Record> records)
        {
            if (secA.Item is StbSecColumn_SRC_ThreeTypesShapeH shapeHA)
            {
                if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeH shapeHB)
                {
                    StbSecColumnSrcThreeTypesShapeHDirectionType.Compare(shapeHA.direction_type.ToString(), shapeHB.direction_type.ToString(), key, records);
                    StbSecColumnSrcThreeTypesShapeHShape.Compare(shapeHA.shape, stBridgeA, shapeHB.shape, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapeHStrengthMain.Compare(shapeHA.strength_main, shapeHB.strength_main, key, records);
                    StbSecColumnSrcThreeTypesShapeHStrengthWeb.Compare(shapeHA.strength_web, shapeHB.strength_web, key, records);
                    StbSecColumnSrcThreeTypesShapeHOffsetX.Compare(shapeHA.offset_XSpecified, shapeHA.offset_X,
                        shapeHB.offset_XSpecified, shapeHB.offset_X, key, records);
                    StbSecColumnSrcThreeTypesShapeHOffsetY.Compare(shapeHA.offset_YSpecified, shapeHA.offset_Y,
                        shapeHB.offset_YSpecified, shapeHB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeBox)
                {
                    StbSecColumnSrcThreeTypesShapeH.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeH), nameof(StbSecColumn_SRC_ThreeTypesShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapePipe)
                {
                    StbSecColumnSrcThreeTypesShapeH.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeH), nameof(StbSecColumn_SRC_ThreeTypesShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeCross)
                {
                    StbSecColumnSrcThreeTypesShapeH.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeH), nameof(StbSecColumn_SRC_ThreeTypesShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeT)
                {
                    StbSecColumnSrcThreeTypesShapeH.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeH), nameof(StbSecColumn_SRC_ThreeTypesShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_ThreeTypesShapeBox shapeBoxA)
            {
                if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeH)
                {
                    StbSecColumnSrcThreeTypesShapeBox.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeBox), nameof(StbSecColumn_SRC_ThreeTypesShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeBox shapeBoxB)
                {
                    StbSecColumnSrcThreeTypesShapeBoxShape.Compare(shapeBoxA.shape, stBridgeA, shapeBoxB.shape, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapeBoxEncaseType.Compare(shapeBoxA.encase_type.ToString(), shapeBoxB.encase_type.ToString(), key, records);
                    StbSecColumnSrcThreeTypesShapeBoxStrength.Compare(shapeBoxA.strength, shapeBoxB.strength, key, records);
                    StbSecColumnSrcThreeTypesShapeBoxOffsetX.Compare(shapeBoxA.offset_XSpecified, shapeBoxA.offset_X,
                        shapeBoxB.offset_XSpecified, shapeBoxB.offset_X, key, records);
                    StbSecColumnSrcThreeTypesShapeBoxOffsetY.Compare(shapeBoxA.offset_YSpecified, shapeBoxA.offset_Y,
                        shapeBoxB.offset_YSpecified, shapeBoxB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapePipe)
                {
                    StbSecColumnSrcThreeTypesShapeBox.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeBox), nameof(StbSecColumn_SRC_ThreeTypesShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeCross)
                {
                    StbSecColumnSrcThreeTypesShapeBox.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeBox), nameof(StbSecColumn_SRC_ThreeTypesShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeT)
                {
                    StbSecColumnSrcThreeTypesShapeBox.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeBox), nameof(StbSecColumn_SRC_ThreeTypesShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_ThreeTypesShapePipe pipeA)
            {
                if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeH)
                {
                    StbSecColumnSrcThreeTypesShapePipe.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapePipe), nameof(StbSecColumn_SRC_ThreeTypesShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeBox)
                {
                    StbSecColumnSrcThreeTypesShapePipe.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapePipe), nameof(StbSecColumn_SRC_ThreeTypesShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapePipe pipeB)
                {
                    StbSecColumnSrcThreeTypesShapePipeShape.Compare(pipeA.shape, stBridgeA, pipeB.shape, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapePipeEncaseType.Compare(pipeA.encase_type.ToString(), pipeB.encase_type.ToString(), key, records);
                    StbSecColumnSrcThreeTypesShapePipeStrengthMain.Compare(pipeA.strength, pipeB.strength, key, records);
                    StbSecColumnSrcThreeTypesShapePipeOffsetX.Compare(pipeA.offset_XSpecified, pipeA.offset_X,
                        pipeB.offset_XSpecified, pipeB.offset_X, key, records);
                    StbSecColumnSrcThreeTypesShapePipeOffsetY.Compare(pipeA.offset_YSpecified, pipeA.offset_Y,
                        pipeB.offset_YSpecified, pipeB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeCross)
                {
                    StbSecColumnSrcThreeTypesShapePipe.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapePipe), nameof(StbSecColumn_SRC_ThreeTypesShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeT)
                {
                    StbSecColumnSrcThreeTypesShapePipe.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapePipe), nameof(StbSecColumn_SRC_ThreeTypesShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_ThreeTypesShapeCross crossA)
            {
                if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeH)
                {
                    StbSecColumnSrcThreeTypesShapeCross.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeCross), nameof(StbSecColumn_SRC_ThreeTypesShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeBox)
                {
                    StbSecColumnSrcThreeTypesShapeCross.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeCross), nameof(StbSecColumn_SRC_ThreeTypesShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapePipe)
                {
                    StbSecColumnSrcThreeTypesShapeCross.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeCross), nameof(StbSecColumn_SRC_ThreeTypesShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeCross crossB)
                {
                    StbSecColumnSrcThreeTypesShapeCrossShapeX.Compare(crossA.shape_X, stBridgeA, crossB.shape_X, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossShapeY.Compare(crossA.shape_Y, stBridgeA, crossB.shape_Y, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossStrengthMainX.Compare(crossA.strength_main_X, crossB.strength_main_X, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossStrengthWebX.Compare(crossA.strength_web_X, crossB.strength_web_X, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossStrengthMainY.Compare(crossA.strength_main_Y, crossB.strength_main_Y, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossStrengthWebY.Compare(crossA.strength_web_Y, crossB.strength_web_Y, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossOffsetXX.Compare(crossA.offset_XXSpecified, crossA.offset_XX,
                        crossB.offset_XXSpecified, crossB.offset_XX, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossOffsetXY.Compare(crossA.offset_XYSpecified, crossA.offset_XY,
                        crossB.offset_XYSpecified, crossB.offset_XY, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossOffsetYX.Compare(crossA.offset_YXSpecified, crossA.offset_YX,
                        crossB.offset_YXSpecified, crossB.offset_YX, key, records);
                    StbSecColumnSrcThreeTypesShapeCrossOffsetYY.Compare(crossA.offset_YYSpecified, crossA.offset_YY,
                        crossB.offset_YYSpecified, crossB.offset_YY, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeT)
                {
                    StbSecColumnSrcThreeTypesShapeCross.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeCross), nameof(StbSecColumn_SRC_ThreeTypesShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_ThreeTypesShapeT shapeTA)
            {
                if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeH)
                {
                    StbSecColumnSrcThreeTypesShapeT.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeT), nameof(StbSecColumn_SRC_ThreeTypesShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeBox)
                {
                    StbSecColumnSrcThreeTypesShapeT.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeT), nameof(StbSecColumn_SRC_ThreeTypesShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapePipe)
                {
                    StbSecColumnSrcThreeTypesShapeT.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeT), nameof(StbSecColumn_SRC_ThreeTypesShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeCross)
                {
                    StbSecColumnSrcThreeTypesShapeT.Compare(nameof(StbSecColumn_SRC_ThreeTypesShapeT), nameof(StbSecColumn_SRC_ThreeTypesShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_ThreeTypesShapeT shapeTB)
                {
                    StbSecColumnSrcThreeTypesShapeTDirectionType.Compare(shapeTA.direction_type.ToString(), shapeTB.direction_type.ToString(), key, records);
                    StbSecColumnSrcThreeTypesShapeTShapeH.Compare(shapeTA.shape_H, stBridgeA, shapeTB.shape_H, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapeTShapeT.Compare(shapeTA.shape_T, stBridgeA, shapeTB.shape_T, stBridgeB, key, records);
                    StbSecColumnSrcThreeTypesShapeTStrengthMainH.Compare(shapeTA.strength_main_H, shapeTB.strength_main_H, key, records);
                    StbSecColumnSrcThreeTypesShapeTStrengthWebH.Compare(shapeTA.strength_web_H, shapeTB.strength_web_H, key, records);
                    StbSecColumnSrcThreeTypesShapeTStrengthMainT.Compare(shapeTA.strength_main_T, shapeTB.strength_main_T, key, records);
                    StbSecColumnSrcThreeTypesShapeTStrengthWebT.Compare(shapeTA.strength_web_T, shapeTB.strength_web_T, key, records);
                    StbSecColumnSrcThreeTypesShapeTOffsetHX.Compare(shapeTA.offset_HXSpecified, shapeTA.offset_HX,
                        shapeTB.offset_HXSpecified, shapeTB.offset_HX, key, records);
                    StbSecColumnSrcThreeTypesShapeTOffsetHY.Compare(shapeTA.offset_HYSpecified, shapeTA.offset_HY,
                        shapeTB.offset_HYSpecified, shapeTB.offset_HY, key, records);
                    StbSecColumnSrcThreeTypesShapeTOffsetT.Compare(shapeTA.offset_TSpecified, shapeTA.offset_T,
                        shapeTB.offset_TSpecified, shapeTB.offset_T, key, records);
                }
            }
        }

        private static void CompareStbSecSteelColumnSrcNotSame(StbSecSteelColumn_SRC_NotSame secA, ST_BRIDGE stBridgeA, StbSecSteelColumn_SRC_NotSame secB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Item is StbSecColumn_SRC_NotSameShapeH shapeHA)
            {
                if (secB.Item is StbSecColumn_SRC_NotSameShapeH shapeHB)
                {
                    StbSecColumnSrcNotSameShapeHDirectionType.Compare(shapeHA.direction_type.ToString(), shapeHB.direction_type.ToString(), key, records);
                    StbSecColumnSrcNotSameShapeHShape.Compare(shapeHA.shape, stBridgeA, shapeHB.shape, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapeHStrengthMain.Compare(shapeHA.strength_main, shapeHB.strength_main, key, records);
                    StbSecColumnSrcNotSameShapeHStrengthWeb.Compare(shapeHA.strength_web, shapeHB.strength_web, key, records);
                    StbSecColumnSrcNotSameShapeHOffsetX.Compare(shapeHA.offset_XSpecified, shapeHA.offset_X,
                        shapeHB.offset_XSpecified, shapeHB.offset_X, key, records);
                    StbSecColumnSrcNotSameShapeHOffsetY.Compare(shapeHA.offset_YSpecified, shapeHA.offset_Y,
                        shapeHB.offset_YSpecified, shapeHB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeBox)
                {
                    StbSecColumnSrcNotSameShapeH.Compare(nameof(StbSecColumn_SRC_NotSameShapeH), nameof(StbSecColumn_SRC_NotSameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapePipe)
                {
                    StbSecColumnSrcNotSameShapeH.Compare(nameof(StbSecColumn_SRC_NotSameShapeH), nameof(StbSecColumn_SRC_NotSameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeCross)
                {
                    StbSecColumnSrcNotSameShapeH.Compare(nameof(StbSecColumn_SRC_NotSameShapeH), nameof(StbSecColumn_SRC_NotSameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeT)
                {
                    StbSecColumnSrcNotSameShapeH.Compare(nameof(StbSecColumn_SRC_NotSameShapeH), nameof(StbSecColumn_SRC_NotSameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_NotSameShapeBox shapeBoxA)
            {
                if (secB.Item is StbSecColumn_SRC_NotSameShapeH)
                {
                    StbSecColumnSrcNotSameShapeBox.Compare(nameof(StbSecColumn_SRC_NotSameShapeBox), nameof(StbSecColumn_SRC_NotSameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeBox shapeBoxB)
                {
                    StbSecColumnSrcNotSameShapeBoxShape.Compare(shapeBoxA.shape, stBridgeA, shapeBoxB.shape, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapeBoxEncaseType.Compare(shapeBoxA.encase_type.ToString(), shapeBoxB.encase_type.ToString(), key, records);
                    StbSecColumnSrcNotSameShapeBoxStrength.Compare(shapeBoxA.strength, shapeBoxB.strength, key, records);
                    StbSecColumnSrcNotSameShapeBoxOffsetX.Compare(shapeBoxA.offset_XSpecified, shapeBoxA.offset_X,
                        shapeBoxB.offset_XSpecified, shapeBoxB.offset_X, key, records);
                    StbSecColumnSrcNotSameShapeBoxOffsetY.Compare(shapeBoxA.offset_YSpecified, shapeBoxA.offset_Y,
                        shapeBoxB.offset_YSpecified, shapeBoxB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapePipe)
                {
                    StbSecColumnSrcNotSameShapeBox.Compare(nameof(StbSecColumn_SRC_NotSameShapeBox), nameof(StbSecColumn_SRC_NotSameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeCross)
                {
                    StbSecColumnSrcNotSameShapeBox.Compare(nameof(StbSecColumn_SRC_NotSameShapeBox), nameof(StbSecColumn_SRC_NotSameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeT)
                {
                    StbSecColumnSrcNotSameShapeBox.Compare(nameof(StbSecColumn_SRC_NotSameShapeBox), nameof(StbSecColumn_SRC_NotSameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_NotSameShapePipe pipeA)
            {
                if (secB.Item is StbSecColumn_SRC_NotSameShapeH)
                {
                    StbSecColumnSrcNotSameShapePipe.Compare(nameof(StbSecColumn_SRC_NotSameShapePipe), nameof(StbSecColumn_SRC_NotSameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeBox)
                {
                    StbSecColumnSrcNotSameShapePipe.Compare(nameof(StbSecColumn_SRC_NotSameShapePipe), nameof(StbSecColumn_SRC_NotSameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapePipe pipeB)
                {
                    StbSecColumnSrcNotSameShapePipeShape.Compare(pipeA.shape, stBridgeA, pipeB.shape, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapePipeEncaseType.Compare(pipeA.encase_type.ToString(), pipeB.encase_type.ToString(), key, records);
                    StbSecColumnSrcNotSameShapePipeStrengthMain.Compare(pipeA.strength, pipeB.strength, key, records);
                    StbSecColumnSrcNotSameShapePipeOffsetX.Compare(pipeA.offset_XSpecified, pipeA.offset_X,
                        pipeB.offset_XSpecified, pipeB.offset_X, key, records);
                    StbSecColumnSrcNotSameShapePipeOffsetY.Compare(pipeA.offset_YSpecified, pipeA.offset_Y,
                        pipeB.offset_YSpecified, pipeB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeCross)
                {
                    StbSecColumnSrcNotSameShapePipe.Compare(nameof(StbSecColumn_SRC_NotSameShapePipe), nameof(StbSecColumn_SRC_NotSameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeT)
                {
                    StbSecColumnSrcNotSameShapePipe.Compare(nameof(StbSecColumn_SRC_NotSameShapePipe), nameof(StbSecColumn_SRC_NotSameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_NotSameShapeCross crossA)
            {
                if (secB.Item is StbSecColumn_SRC_NotSameShapeH)
                {
                    StbSecColumnSrcNotSameShapeCross.Compare(nameof(StbSecColumn_SRC_NotSameShapeCross), nameof(StbSecColumn_SRC_NotSameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeBox)
                {
                    StbSecColumnSrcNotSameShapeCross.Compare(nameof(StbSecColumn_SRC_NotSameShapeCross), nameof(StbSecColumn_SRC_NotSameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapePipe)
                {
                    StbSecColumnSrcNotSameShapeCross.Compare(nameof(StbSecColumn_SRC_NotSameShapeCross), nameof(StbSecColumn_SRC_NotSameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeCross crossB)
                {
                    StbSecColumnSrcNotSameShapeCrossShapeX.Compare(crossA.shape_X, stBridgeA, crossB.shape_X, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapeCrossShapeY.Compare(crossA.shape_Y, stBridgeA, crossB.shape_Y, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapeCrossStrengthMainX.Compare(crossA.strength_main_X, crossB.strength_main_X, key, records);
                    StbSecColumnSrcNotSameShapeCrossStrengthWebX.Compare(crossA.strength_web_X, crossB.strength_web_X, key, records);
                    StbSecColumnSrcNotSameShapeCrossStrengthMainY.Compare(crossA.strength_main_Y, crossB.strength_main_Y, key, records);
                    StbSecColumnSrcNotSameShapeCrossStrengthWebY.Compare(crossA.strength_web_Y, crossB.strength_web_Y, key, records);
                    StbSecColumnSrcNotSameShapeCrossOffsetXX.Compare(crossA.offset_XXSpecified, crossA.offset_XX,
                        crossB.offset_XXSpecified, crossB.offset_XX, key, records);
                    StbSecColumnSrcNotSameShapeCrossOffsetXY.Compare(crossA.offset_XYSpecified, crossA.offset_XY,
                        crossB.offset_XYSpecified, crossB.offset_XY, key, records);
                    StbSecColumnSrcNotSameShapeCrossOffsetYX.Compare(crossA.offset_YXSpecified, crossA.offset_YX,
                        crossB.offset_YXSpecified, crossB.offset_YX, key, records);
                    StbSecColumnSrcNotSameShapeCrossOffsetYY.Compare(crossA.offset_YYSpecified, crossA.offset_YY,
                        crossB.offset_YYSpecified, crossB.offset_YY, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeT)
                {
                    StbSecColumnSrcNotSameShapeCross.Compare(nameof(StbSecColumn_SRC_NotSameShapeCross), nameof(StbSecColumn_SRC_NotSameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_NotSameShapeT shapeTA)
            {
                if (secB.Item is StbSecColumn_SRC_NotSameShapeH)
                {
                    StbSecColumnSrcNotSameShapeT.Compare(nameof(StbSecColumn_SRC_NotSameShapeT), nameof(StbSecColumn_SRC_NotSameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeBox)
                {
                    StbSecColumnSrcNotSameShapeT.Compare(nameof(StbSecColumn_SRC_NotSameShapeT), nameof(StbSecColumn_SRC_NotSameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapePipe)
                {
                    StbSecColumnSrcNotSameShapeT.Compare(nameof(StbSecColumn_SRC_NotSameShapeT), nameof(StbSecColumn_SRC_NotSameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeCross)
                {
                    StbSecColumnSrcNotSameShapeT.Compare(nameof(StbSecColumn_SRC_NotSameShapeT), nameof(StbSecColumn_SRC_NotSameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_NotSameShapeT shapeTB)
                {
                    StbSecColumnSrcNotSameShapeTDirectionType.Compare(shapeTA.direction_type.ToString(), shapeTB.direction_type.ToString(), key, records);
                    StbSecColumnSrcNotSameShapeTShapeH.Compare(shapeTA.shape_H, stBridgeA, shapeTB.shape_H, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapeTShapeT.Compare(shapeTA.shape_T, stBridgeA, shapeTB.shape_T, stBridgeB, key, records);
                    StbSecColumnSrcNotSameShapeTStrengthMainH.Compare(shapeTA.strength_main_H, shapeTB.strength_main_H, key, records);
                    StbSecColumnSrcNotSameShapeTStrengthWebH.Compare(shapeTA.strength_web_H, shapeTB.strength_web_H, key, records);
                    StbSecColumnSrcNotSameShapeTStrengthMainT.Compare(shapeTA.strength_main_T, shapeTB.strength_main_T, key, records);
                    StbSecColumnSrcNotSameShapeTStrengthWebT.Compare(shapeTA.strength_web_T, shapeTB.strength_web_T, key, records);
                    StbSecColumnSrcNotSameShapeTOffsetHX.Compare(shapeTA.offset_HXSpecified, shapeTA.offset_HX,
                        shapeTB.offset_HXSpecified, shapeTB.offset_HX, key, records);
                    StbSecColumnSrcNotSameShapeTOffsetHY.Compare(shapeTA.offset_HYSpecified, shapeTA.offset_HY,
                        shapeTB.offset_HYSpecified, shapeTB.offset_HY, key, records);
                    StbSecColumnSrcNotSameShapeTOffsetT.Compare(shapeTA.offset_TSpecified, shapeTA.offset_T,
                        shapeTB.offset_TSpecified, shapeTB.offset_T, key, records);
                }
            }
        }

        private static void CompareStbSecSteelColumnSrcSame(StbSecSteelColumn_SRC_Same secA, ST_BRIDGE stBridgeA, StbSecSteelColumn_SRC_Same secB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Item is StbSecColumn_SRC_SameShapeH shapeHA)
            {
                if (secB.Item is StbSecColumn_SRC_SameShapeH shapeHB)
                {
                    StbSecColumnSrcSameShapeHDirectionType.Compare(shapeHA.direction_type.ToString(), shapeHB.direction_type.ToString(), key, records);
                    StbSecColumnSrcSameShapeHShape.Compare(shapeHA.shape, stBridgeA, shapeHB.shape, stBridgeB, key, records);
                    StbSecColumnSrcSameShapeHStrengthMain.Compare(shapeHA.strength_main, shapeHB.strength_main, key, records);
                    StbSecColumnSrcSameShapeHStrengthWeb.Compare(shapeHA.strength_web, shapeHB.strength_web, key, records);
                    StbSecColumnSrcSameShapeHOffsetX.Compare(shapeHA.offset_XSpecified, shapeHA.offset_X,
                        shapeHB.offset_XSpecified, shapeHB.offset_X, key, records);
                    StbSecColumnSrcSameShapeHOffsetY.Compare(shapeHA.offset_YSpecified, shapeHA.offset_Y,
                        shapeHB.offset_YSpecified, shapeHB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeBox)
                {
                    StbSecColumnSrcSameShapeH.Compare(nameof(StbSecColumn_SRC_SameShapeH), nameof(StbSecColumn_SRC_SameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapePipe)
                {
                    StbSecColumnSrcSameShapeH.Compare(nameof(StbSecColumn_SRC_SameShapeH), nameof(StbSecColumn_SRC_SameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeCross)
                {
                    StbSecColumnSrcSameShapeH.Compare(nameof(StbSecColumn_SRC_SameShapeH), nameof(StbSecColumn_SRC_SameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeT)
                {
                    StbSecColumnSrcSameShapeH.Compare(nameof(StbSecColumn_SRC_SameShapeH), nameof(StbSecColumn_SRC_SameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_SameShapeBox shapeBoxA)
            {
                if (secB.Item is StbSecColumn_SRC_SameShapeH)
                {
                    StbSecColumnSrcSameShapeBox.Compare(nameof(StbSecColumn_SRC_SameShapeBox), nameof(StbSecColumn_SRC_SameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeBox shapeBoxB)
                {
                    StbSecColumnSrcSameShapeBoxShape.Compare(shapeBoxA.shape, stBridgeA, shapeBoxB.shape, stBridgeB, key, records);
                    StbSecColumnSrcSameShapeBoxEncaseType.Compare(shapeBoxA.encase_type.ToString(), shapeBoxB.encase_type.ToString(), key, records);
                    StbSecColumnSrcSameShapeBoxStrength.Compare(shapeBoxA.strength, shapeBoxB.strength, key, records);
                    StbSecColumnSrcSameShapeBoxOffsetX.Compare(shapeBoxA.offset_XSpecified, shapeBoxA.offset_X,
                        shapeBoxB.offset_XSpecified, shapeBoxB.offset_X, key, records);
                    StbSecColumnSrcSameShapeBoxOffsetY.Compare(shapeBoxA.offset_YSpecified, shapeBoxA.offset_Y,
                        shapeBoxB.offset_YSpecified, shapeBoxB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapePipe)
                {
                    StbSecColumnSrcSameShapeBox.Compare(nameof(StbSecColumn_SRC_SameShapeBox), nameof(StbSecColumn_SRC_SameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeCross)
                {
                    StbSecColumnSrcSameShapeBox.Compare(nameof(StbSecColumn_SRC_SameShapeBox), nameof(StbSecColumn_SRC_SameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeT)
                {
                    StbSecColumnSrcSameShapeBox.Compare(nameof(StbSecColumn_SRC_SameShapeBox), nameof(StbSecColumn_SRC_SameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_SameShapePipe pipeA)
            {
                if (secB.Item is StbSecColumn_SRC_SameShapeH)
                {
                    StbSecColumnSrcSameShapePipe.Compare(nameof(StbSecColumn_SRC_SameShapePipe), nameof(StbSecColumn_SRC_SameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeBox)
                {
                    StbSecColumnSrcSameShapePipe.Compare(nameof(StbSecColumn_SRC_SameShapePipe), nameof(StbSecColumn_SRC_SameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapePipe pipeB)
                {
                    StbSecColumnSrcSameShapePipeShape.Compare(pipeA.shape, stBridgeA, pipeB.shape, stBridgeB, key, records);
                    StbSecColumnSrcSameShapePipeEncaseType.Compare(pipeA.encase_type.ToString(), pipeB.encase_type.ToString(), key, records);
                    StbSecColumnSrcSameShapePipeStrengthMain.Compare(pipeA.strength, pipeB.strength, key, records);
                    StbSecColumnSrcSameShapePipeOffsetX.Compare(pipeA.offset_XSpecified, pipeA.offset_X,
                        pipeB.offset_XSpecified, pipeB.offset_X, key, records);
                    StbSecColumnSrcSameShapePipeOffsetY.Compare(pipeA.offset_YSpecified, pipeA.offset_Y,
                        pipeB.offset_YSpecified, pipeB.offset_Y, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeCross)
                {
                    StbSecColumnSrcSameShapePipe.Compare(nameof(StbSecColumn_SRC_SameShapePipe), nameof(StbSecColumn_SRC_SameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeT)
                {
                    StbSecColumnSrcSameShapePipe.Compare(nameof(StbSecColumn_SRC_SameShapePipe), nameof(StbSecColumn_SRC_SameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_SameShapeCross crossA)
            {
                if (secB.Item is StbSecColumn_SRC_SameShapeH)
                {
                    StbSecColumnSrcSameShapeCross.Compare(nameof(StbSecColumn_SRC_SameShapeCross), nameof(StbSecColumn_SRC_SameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeBox)
                {
                    StbSecColumnSrcSameShapeCross.Compare(nameof(StbSecColumn_SRC_SameShapeCross), nameof(StbSecColumn_SRC_SameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapePipe)
                {
                    StbSecColumnSrcSameShapeCross.Compare(nameof(StbSecColumn_SRC_SameShapeCross), nameof(StbSecColumn_SRC_SameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeCross crossB)
                {
                    StbSecColumnSrcSameShapeCrossShapeX.Compare(crossA.shape_X, stBridgeA, crossB.shape_X, stBridgeB, key, records);
                    StbSecColumnSrcSameShapeCrossShapeY.Compare(crossA.shape_Y, stBridgeA, crossB.shape_Y, stBridgeB, key, records);
                    StbSecColumnSrcSameShapeCrossStrengthMainX.Compare(crossA.strength_main_X, crossB.strength_main_X, key, records);
                    StbSecColumnSrcSameShapeCrossStrengthWebX.Compare(crossA.strength_web_X, crossB.strength_web_X, key, records);
                    StbSecColumnSrcSameShapeCrossStrengthMainY.Compare(crossA.strength_main_Y, crossB.strength_main_Y, key, records);
                    StbSecColumnSrcSameShapeCrossStrengthWebY.Compare(crossA.strength_web_Y, crossB.strength_web_Y, key, records);
                    StbSecColumnSrcSameShapeCrossOffsetXX.Compare(crossA.offset_XXSpecified, crossA.offset_XX,
                        crossB.offset_XXSpecified, crossB.offset_XX, key, records);
                    StbSecColumnSrcSameShapeCrossOffsetXY.Compare(crossA.offset_XYSpecified, crossA.offset_XY,
                        crossB.offset_XYSpecified, crossB.offset_XY, key, records);
                    StbSecColumnSrcSameShapeCrossOffsetYX.Compare(crossA.offset_YXSpecified, crossA.offset_YX,
                        crossB.offset_YXSpecified, crossB.offset_YX, key, records);
                    StbSecColumnSrcSameShapeCrossOffsetYY.Compare(crossA.offset_YYSpecified, crossA.offset_YY,
                        crossB.offset_YYSpecified, crossB.offset_YY, key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeT)
                {
                    StbSecColumnSrcSameShapeCross.Compare(nameof(StbSecColumn_SRC_SameShapeCross), nameof(StbSecColumn_SRC_SameShapeT), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_SameShapeT shapeTA)
            {
                if (secB.Item is StbSecColumn_SRC_SameShapeH)
                {
                    StbSecColumnSrcSameShapeT.Compare(nameof(StbSecColumn_SRC_SameShapeT), nameof(StbSecColumn_SRC_SameShapeH), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeBox)
                {
                    StbSecColumnSrcSameShapeT.Compare(nameof(StbSecColumn_SRC_SameShapeT), nameof(StbSecColumn_SRC_SameShapeBox), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapePipe)
                {
                    StbSecColumnSrcSameShapeT.Compare(nameof(StbSecColumn_SRC_SameShapeT), nameof(StbSecColumn_SRC_SameShapePipe), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeCross)
                {
                    StbSecColumnSrcSameShapeT.Compare(nameof(StbSecColumn_SRC_SameShapeT), nameof(StbSecColumn_SRC_SameShapeCross), key, records);
                }
                else if (secB.Item is StbSecColumn_SRC_SameShapeT shapeTB)
                {
                    StbSecColumnSrcSameShapeTDirectionType.Compare(shapeTA.direction_type.ToString(), shapeTB.direction_type.ToString(), key, records);
                    StbSecColumnSrcSameShapeTShapeH.Compare(shapeTA.shape_H, stBridgeA, shapeTB.shape_H, stBridgeB, key, records);
                    StbSecColumnSrcSameShapeTShapeT.Compare(shapeTA.shape_T, stBridgeA, shapeTB.shape_T, stBridgeB, key, records);
                    StbSecColumnSrcSameShapeTStrengthMainH.Compare(shapeTA.strength_main_H, shapeTB.strength_main_H, key, records);
                    StbSecColumnSrcSameShapeTStrengthWebH.Compare(shapeTA.strength_web_H, shapeTB.strength_web_H, key, records);
                    StbSecColumnSrcSameShapeTStrengthMainT.Compare(shapeTA.strength_main_T, shapeTB.strength_main_T, key, records);
                    StbSecColumnSrcSameShapeTStrengthWebT.Compare(shapeTA.strength_web_T, shapeTB.strength_web_T, key, records);
                    StbSecColumnSrcSameShapeTOffsetHX.Compare(shapeTA.offset_HXSpecified, shapeTA.offset_HX,
                        shapeTB.offset_HXSpecified, shapeTB.offset_HX, key, records);
                    StbSecColumnSrcSameShapeTOffsetHY.Compare(shapeTA.offset_HYSpecified, shapeTA.offset_HY,
                        shapeTB.offset_HYSpecified, shapeTB.offset_HY, key, records);
                    StbSecColumnSrcSameShapeTOffsetT.Compare(shapeTA.offset_TSpecified, shapeTA.offset_T,
                        shapeTB.offset_TSpecified, shapeTB.offset_T, key, records);
                }
            }
        }

        private static void CompareSecBarArrangementColumnSrc(StbSecBarArrangementColumn_SRC secA, StbSecBarArrangementColumn_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementColumnSrcDepthCoverStartX.Compare(secA.depth_cover_start_XSpecified,
                secA.depth_cover_start_X, secB.depth_cover_start_XSpecified, secB.depth_cover_start_X, key, records);
            StbSecBarArrangementColumnSrcDepthCoverEndX.Compare(secA.depth_cover_end_XSpecified,
                secA.depth_cover_end_X, secB.depth_cover_end_XSpecified, secB.depth_cover_end_X, key, records);
            StbSecBarArrangementColumnSrcDepthCoverStartY.Compare(secA.depth_cover_start_YSpecified,
                secA.depth_cover_start_Y, secB.depth_cover_start_YSpecified, secB.depth_cover_start_Y, key, records);
            StbSecBarArrangementColumnSrcDepthCoverEndY.Compare(secA.depth_cover_start_YSpecified,
                secA.depth_cover_start_Y, secB.depth_cover_start_YSpecified, secB.depth_cover_start_Y, key, records);
            StbSecBarArrangementColumnSrcInterval.Compare(secA.intervalSpecified, secA.interval, secB.intervalSpecified,
                secB.interval, key, records);
            StbSecBarArrangementColumnSrcKindCorner.Compare(secA.kind_corner.ToString(), secB.kind_corner.ToString(), key, records);
            StbSecBarArrangementColumnSrcIsSpiral.Compare(secA.isSpiral, secB.isSpiral, key, records);
            StbSecBarArrangementColumnSrcCenterStartX.Compare(secA.center_start_XSpecified, secA.center_start_X,
                secB.center_start_XSpecified, secB.center_start_X, key, records);
            StbSecBarArrangementColumnSrcCenterEndX.Compare(secA.center_end_XSpecified, secA.center_end_X,
                secB.center_end_XSpecified, secB.center_end_X, key, records);
            StbSecBarArrangementColumnSrcCenterStartY.Compare(secA.center_start_YSpecified, secA.center_start_Y,
                secB.center_start_YSpecified, secB.center_start_Y, key, records);
            StbSecBarArrangementColumnSrcCenterEndY.Compare(secA.center_end_XSpecified, secA.center_end_X,
                secB.center_end_XSpecified, secB.center_end_X, key, records);
            StbSecBarArrangementColumnSrcCenterInterval.Compare(secA.center_intervalSpecified, secA.center_interval,
                secB.center_intervalSpecified, secB.center_interval, key, records);

            if (secA.Items.Any(n => n is StbSecBarColumn_SRC_RectSame))
            {
                if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectSame))
                {
                    var a = secA.Items.OfType<StbSecBarColumn_SRC_RectSame>().First();
                    var b = secB.Items.OfType<StbSecBarColumn_SRC_RectSame>().First();
                    if (b != null)
                    {
                        StbSecBarColumnSrcRectSameDMain.Compare(a.D_main, b.D_main, key, records);
                        StbSecBarColumnSrcRectSameD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key, records);
                        StbSecBarColumnSrcRectSameDAxial.Compare(a.D_axial, b.D_axial, key, records);
                        StbSecBarColumnSrcRectSameDBand.Compare(a.D_band, b.D_band, key, records);
                        StbSecBarColumnSrcRectSameDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key, records);
                        StbSecBarColumnSrcRectSameStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                        StbSecBarColumnSrcRectSameStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key, records);
                        StbSecBarColumnSrcRectSameStrengthAxial.Compare(a.strength_axial, b.strength_axial, key, records);
                        StbSecBarColumnSrcRectSameStrengthBand.Compare(a.strength_band, b.strength_band, key, records);
                        StbSecBarColumnSrcRectSameStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key, records);
                        StbSecBarColumnSrcRectSameNMainX1st.Compare(a.N_main_X_1st, b.N_main_X_1st, key, records);
                        StbSecBarColumnSrcRectSameNMainX2nd.Compare(a.N_main_X_2nd, b.N_main_X_2nd, key, records);
                        StbSecBarColumnSrcRectSameNMainY1st.Compare(a.N_main_Y_1st, b.N_main_Y_1st, key, records);
                        StbSecBarColumnSrcRectSameNMainY2nd.Compare(a.N_main_Y_2nd, b.N_main_Y_2nd, key, records);
                        StbSecBarColumnSrcRectSameN2ndMainX1st.Compare(a.N_2nd_main_X_1st, b.N_2nd_main_X_1st, key, records);
                        StbSecBarColumnSrcRectSameN2ndMainX2nd.Compare(a.N_2nd_main_X_2nd, b.N_2nd_main_X_2nd, key, records);
                        StbSecBarColumnSrcRectSameN2ndMainY1st.Compare(a.N_2nd_main_Y_1st, b.N_2nd_main_Y_1st, key, records);
                        StbSecBarColumnSrcRectSameN2ndMainY2nd.Compare(a.N_2nd_main_Y_2nd, b.N_2nd_main_Y_2nd, key, records);
                        StbSecBarColumnSrcRectSameNMainTotal.Compare(a.N_main_total, b.N_main_total, key, records);
                        StbSecBarColumnSrcRectSamePitchBand.Compare(a.pitch_band, b.pitch_band, key, records);
                        StbSecBarColumnSrcRectSameNBandDirectionX.Compare(a.N_band_direction_X, b.N_band_direction_X, key, records);
                        StbSecBarColumnSrcRectSameNBandDirectionY.Compare(a.N_band_direction_Y, b.N_band_direction_Y, key, records);
                        StbSecBarColumnSrcRectSamePitchBarSpacing.Compare(a.pitch_bar_spacingSpecified,
                            a.pitch_bar_spacing, b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key,
                            records);
                        StbSecBarColumnSrcRectSameNBarSpacingX.Compare(a.N_bar_spacing_X, b.N_bar_spacing_X, key, records);
                        StbSecBarColumnSrcRectSameNBarSpacingY.Compare(a.N_bar_spacing_Y, b.N_bar_spacing_Y, key, records);
                    }
                    else
                    {
                        StbSecBarColumnSrcRectSame.Compare(nameof(StbSecBarColumn_RC_RectSame), null, key, records);
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectNotSame))
                {
                    StbSecBarColumnSrcRectSame.Compare(nameof(StbSecBarColumn_SRC_RectSame), nameof(StbSecBarColumn_SRC_RectNotSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleSame))
                {
                    StbSecBarColumnSrcRectSame.Compare(nameof(StbSecBarColumn_SRC_RectSame), nameof(StbSecBarColumn_SRC_CircleSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleNotSame))
                {
                    StbSecBarColumnSrcRectSame.Compare(nameof(StbSecBarColumn_SRC_RectSame), nameof(StbSecBarColumn_SRC_CircleNotSame), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarColumn_SRC_RectNotSame))
            {
                if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectSame))
                {
                    StbSecBarColumnSrcRectNotSame.Compare(nameof(StbSecBarColumn_SRC_RectNotSame), nameof(StbSecBarColumn_SRC_RectSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectNotSame))
                {
                    var set = new HashSet<StbSecBarColumn_SRC_RectNotSame>();
                    foreach (var rectNotSameA in secA.Items.OfType<StbSecBarColumn_SRC_RectNotSame>())
                    {
                        var key1 = new List<string>(key) { "pos=" + rectNotSameA.pos };
                        var notSameB = secB.Items.OfType<StbSecBarColumn_SRC_RectNotSame>()
                            .FirstOrDefault(n => n.pos == rectNotSameA.pos);
                        if (notSameB != null)
                        {
                            StbSecBarColumnSrcRectNotSamePos.Compare(rectNotSameA.pos.ToString(), notSameB.pos.ToString(), key1, records);
                            StbSecBarColumnSrcRectNotSameDMain.Compare(rectNotSameA.D_main, notSameB.D_main, key1, records);
                            StbSecBarColumnSrcRectNotSameD2ndMain.Compare(rectNotSameA.D_2nd_main, notSameB.D_2nd_main, key1, records);
                            StbSecBarColumnSrcRectNotSameDAxial.Compare(rectNotSameA.D_axial, notSameB.D_axial, key1, records);
                            StbSecBarColumnSrcRectNotSameDBand.Compare(rectNotSameA.D_band, notSameB.D_band, key1, records);
                            StbSecBarColumnSrcRectNotSameDBarSpacing.Compare(rectNotSameA.D_bar_spacing, notSameB.D_bar_spacing, key1, records);
                            StbSecBarColumnSrcRectNotSameStrengthMain.Compare(rectNotSameA.strength_main, notSameB.strength_main, key1, records);
                            StbSecBarColumnSrcRectNotSameStrength2ndMain.Compare(rectNotSameA.strength_2nd_main, notSameB.strength_2nd_main, key1, records);
                            StbSecBarColumnSrcRectNotSameStrengthAxial.Compare(rectNotSameA.strength_axial, notSameB.strength_axial, key1, records);
                            StbSecBarColumnSrcRectNotSameStrengthBand.Compare(rectNotSameA.strength_band, notSameB.strength_band, key1, records);
                            StbSecBarColumnSrcRectNotSameStrengthBarSpacing.Compare(rectNotSameA.strength_bar_spacing, notSameB.strength_bar_spacing, key1, records);
                            StbSecBarColumnSrcRectNotSameNMainX1st.Compare(rectNotSameA.N_main_X_1st, notSameB.N_main_X_1st, key1, records);
                            StbSecBarColumnSrcRectNotSameNMainX2nd.Compare(rectNotSameA.N_main_X_2nd, notSameB.N_main_X_2nd, key1, records);
                            StbSecBarColumnSrcRectNotSameNMainY1st.Compare(rectNotSameA.N_main_Y_1st, notSameB.N_main_Y_1st, key1, records);
                            StbSecBarColumnSrcRectNotSameNMainY2nd.Compare(rectNotSameA.N_main_Y_2nd, notSameB.N_main_Y_2nd, key1, records);
                            StbSecBarColumnSrcRectNotSameN2ndMainX1st.Compare(rectNotSameA.N_2nd_main_X_1st, notSameB.N_2nd_main_X_1st, key1, records);
                            StbSecBarColumnSrcRectNotSameN2ndMainX2nd.Compare(rectNotSameA.N_2nd_main_X_2nd, notSameB.N_2nd_main_X_2nd, key1, records);
                            StbSecBarColumnSrcRectNotSameN2ndMainY1st.Compare(rectNotSameA.N_2nd_main_Y_1st, notSameB.N_2nd_main_Y_1st, key1, records);
                            StbSecBarColumnSrcRectNotSameN2ndMainY2nd.Compare(rectNotSameA.N_2nd_main_Y_2nd, notSameB.N_2nd_main_Y_2nd, key1, records);
                            StbSecBarColumnSrcRectNotSameNMainTotal.Compare(rectNotSameA.N_main_total, notSameB.N_main_total, key1, records);
                            StbSecBarColumnSrcRectNotSamePitchBand.Compare(rectNotSameA.pitch_band, notSameB.pitch_band, key1, records);
                            StbSecBarColumnSrcRectNotSameNBandDirectionX.Compare(rectNotSameA.N_band_direction_X, notSameB.N_band_direction_X, key1, records);
                            StbSecBarColumnSrcRectNotSameNBandDirectionY.Compare(rectNotSameA.N_band_direction_Y, notSameB.N_band_direction_Y, key1, records);
                            StbSecBarColumnSrcRectNotSamePitchBarSpacing.Compare(rectNotSameA.pitch_bar_spacingSpecified,
                                rectNotSameA.pitch_bar_spacing, notSameB.pitch_bar_spacingSpecified, notSameB.pitch_bar_spacing, key1,
                                records);
                            StbSecBarColumnSrcRectNotSameNBarSpacingX.Compare(rectNotSameA.N_bar_spacing_X, notSameB.N_bar_spacing_X, key1, records);
                            StbSecBarColumnSrcRectNotSameNBarSpacingY.Compare(rectNotSameA.N_bar_spacing_Y, notSameB.N_bar_spacing_Y, key1, records);
                            set.Add(notSameB);
                        }
                        else
                        {
                            StbSecBarColumnSrcRectNotSame.Compare(nameof(StbSecBarColumn_SRC_RectNotSame), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarColumn_SRC_RectNotSame>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarColumnSrcRectNotSame.Compare(nameof(StbSecBarColumn_SRC_RectNotSame), null, keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleSame))
                {
                    StbSecBarColumnSrcRectNotSame.Compare(nameof(StbSecBarColumn_SRC_RectNotSame), nameof(StbSecBarColumn_SRC_CircleSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleNotSame))
                {
                    StbSecBarColumnSrcRectNotSame.Compare(nameof(StbSecBarColumn_SRC_RectNotSame), nameof(StbSecBarColumn_SRC_CircleNotSame), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarColumn_SRC_CircleSame))
            {
                if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectSame))
                {
                    StbSecBarColumnSrcCircleSame.Compare(nameof(StbSecBarColumn_SRC_CircleSame), nameof(StbSecBarColumn_SRC_RectSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectNotSame))
                {
                    StbSecBarColumnSrcCircleSame.Compare(nameof(StbSecBarColumn_SRC_CircleSame), nameof(StbSecBarColumn_SRC_RectNotSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleSame))
                {
                    var circleSameA = secA.Items.OfType<StbSecBarColumn_SRC_CircleSame>().First();
                    var circleSameB = secB.Items.OfType<StbSecBarColumn_SRC_CircleSame>().FirstOrDefault();
                    if (circleSameB != null)
                    {
                        StbSecBarColumnSrcCircleSameDMain.Compare(circleSameA.D_main, circleSameB.D_main, key, records);
                        StbSecBarColumnSrcCircleSameDAxial.Compare(circleSameA.D_axial, circleSameB.D_axial, key, records);
                        StbSecBarColumnSrcCircleSameDBand.Compare(circleSameA.D_band, circleSameB.D_band, key, records);
                        StbSecBarColumnSrcCircleSameDBarSpacing.Compare(circleSameA.D_bar_spacing, circleSameB.D_bar_spacing, key, records);
                        StbSecBarColumnSrcCircleSameStrengthMain.Compare(circleSameA.strength_main, circleSameB.strength_main, key, records);
                        StbSecBarColumnSrcCircleSameStrengthAxial.Compare(circleSameA.strength_axial, circleSameB.strength_axial, key, records);
                        StbSecBarColumnSrcCircleSameStrengthBand.Compare(circleSameA.strength_band, circleSameB.strength_band, key, records);
                        StbSecBarColumnSrcCircleSameStrengthBarSpacing.Compare(circleSameA.strength_bar_spacing, circleSameB.strength_bar_spacing, key, records);
                        StbSecBarColumnSrcCircleSameNMain.Compare(circleSameA.N_main, circleSameB.N_main, key, records);
                        StbSecBarColumnSrcCircleSameNAxial.Compare(circleSameA.N_axial, circleSameB.N_axial, key, records);
                        StbSecBarColumnSrcCircleSameNBand.Compare(circleSameA.N_band, circleSameB.N_band, key, records);
                        StbSecBarColumnSrcCircleSamePitchBand.Compare(circleSameA.pitch_band, circleSameB.pitch_band, key, records);
                        StbSecBarColumnSrcCircleSamePitchBarSpacing.Compare(circleSameA.pitch_bar_spacingSpecified,
                            circleSameA.pitch_bar_spacing, circleSameB.pitch_bar_spacingSpecified,
                            circleSameB.pitch_bar_spacing, key, records);
                        StbSecBarColumnSrcCircleSameNBarSpacingX.Compare(circleSameA.N_bar_spacing_X, circleSameB.N_bar_spacing_X, key, records);
                        StbSecBarColumnSrcCircleSameNBarSpacingY.Compare(circleSameA.N_bar_spacing_Y, circleSameB.N_bar_spacing_Y, key, records);
                    }
                    else
                    {
                        StbSecBarColumnSrcCircleSame.Compare(nameof(StbSecBarColumn_SRC_CircleSame), null, key, records);
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleNotSame))
                {
                    StbSecBarColumnSrcCircleSame.Compare(nameof(StbSecBarColumn_SRC_CircleSame), nameof(StbSecBarColumn_SRC_CircleNotSame), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarColumn_SRC_CircleNotSame))
            {
                if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectSame))
                {
                    StbSecBarColumnSrcCircleNotSame.Compare(nameof(StbSecBarColumn_SRC_CircleNotSame), nameof(StbSecBarColumn_SRC_RectSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_RectNotSame))
                {
                    StbSecBarColumnSrcCircleNotSame.Compare(nameof(StbSecBarColumn_SRC_CircleNotSame), nameof(StbSecBarColumn_SRC_RectNotSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleSame))
                {
                    StbSecBarColumnSrcCircleNotSame.Compare(nameof(StbSecBarColumn_SRC_CircleNotSame), nameof(StbSecBarColumn_SRC_CircleSame), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarColumn_SRC_CircleNotSame))
                {
                    var set = new HashSet<StbSecBarColumn_SRC_CircleNotSame>();
                    foreach (var circleNotSameA in secA.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>())
                    {
                        var key1 = new List<string>(key) { "pos=" + circleNotSameA.pos };
                        var circleNotSameB = secB.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>()
                            .FirstOrDefault(n => n.pos == circleNotSameA.pos);
                        if (circleNotSameB != null)
                        {
                            StbSecBarColumnSrcCircleNotSamePos.Compare(circleNotSameA.pos.ToString(), circleNotSameB.pos.ToString(), key1, records);
                            StbSecBarColumnSrcCircleNotSameDMain.Compare(circleNotSameA.D_main, circleNotSameB.D_main, key1, records);
                            StbSecBarColumnSrcCircleNotSameDAxial.Compare(circleNotSameA.D_axial, circleNotSameB.D_axial, key1, records);
                            StbSecBarColumnSrcCircleNotSameDBand.Compare(circleNotSameA.D_band, circleNotSameB.D_band, key1, records);
                            StbSecBarColumnSrcCircleNotSameDBarSpacing.Compare(circleNotSameA.D_bar_spacing, circleNotSameB.D_bar_spacing, key1, records);
                            StbSecBarColumnSrcCircleNotSameStrengthMain.Compare(circleNotSameA.strength_main, circleNotSameB.strength_main, key1, records);
                            StbSecBarColumnSrcCircleNotSameStrengthAxial.Compare(circleNotSameA.strength_axial, circleNotSameB.strength_axial, key1, records);
                            StbSecBarColumnSrcCircleNotSameStrengthBand.Compare(circleNotSameA.strength_band, circleNotSameB.strength_band, key1, records);
                            StbSecBarColumnSrcCircleNotSameStrengthBarSpacing.Compare(circleNotSameA.strength_bar_spacing, circleNotSameB.strength_bar_spacing, key1, records);
                            StbSecBarColumnSrcCircleNotSameNMain.Compare(circleNotSameA.N_main, circleNotSameB.N_main, key1, records);
                            StbSecBarColumnSrcCircleNotSameNAxial.Compare(circleNotSameA.N_axial, circleNotSameB.N_axial, key1, records);
                            StbSecBarColumnSrcCircleNotSameNBand.Compare(circleNotSameA.N_band, circleNotSameB.N_band, key1, records);
                            StbSecBarColumnSrcCircleNotSamePitchBand.Compare(circleNotSameA.pitch_band, circleNotSameB.pitch_band, key1, records);
                            StbSecBarColumnSrcCircleNotSamePitchBarSpacing.Compare(circleNotSameA.pitch_bar_spacingSpecified,
                                circleNotSameA.pitch_bar_spacing, circleNotSameB.pitch_bar_spacingSpecified,
                                circleNotSameB.pitch_bar_spacing, key1, records);
                            StbSecBarColumnSrcCircleNotSameNBarSpacingX.Compare(circleNotSameA.N_bar_spacing_X, circleNotSameB.N_bar_spacing_X, key1, records);
                            StbSecBarColumnSrcCircleNotSameNBarSpacingY.Compare(circleNotSameA.N_bar_spacing_Y, circleNotSameB.N_bar_spacing_Y, key1, records);
                            set.Add(circleNotSameB);
                        }
                        else
                        {
                            StbSecBarColumnSrcCircleNotSame.Compare(nameof(StbSecBarColumn_SRC_CircleNotSame), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarColumnSrcCircleNotSame.Compare(nameof(StbSecBarColumn_SRC_CircleNotSame), null, keyB, records);
                        }
                    }
                }
            }
        }

        private static void CompareSecFigureColumnSrc(StbSecFigureColumn_SRC secA, StbSecFigureColumn_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Item is StbSecColumn_SRC_Rect rectA)
            {
                if (secB.Item is StbSecColumn_SRC_Rect rectB)
                {
                    StbSecColumnSrcRectWidthX.Compare(rectA.width_X, rectB.width_X, key, records);
                    StbSecColumnSrcRectWidthY.Compare(rectA.width_Y, rectB.width_Y, key, records);
                }
                else
                {
                    StbSecColumnSrcRect.Compare(nameof(StbSecColumn_SRC_Rect), nameof(StbSecColumn_SRC_Circle), key, records);
                }
            }
            else if (secA.Item is StbSecColumn_SRC_Circle circleA)
            {
                if (secB.Item is StbSecColumn_SRC_Circle circleB)
                {
                    StbSecColumnSrcCircleD.Compare(circleA.D, circleB.D, key, records);
                }
                else
                {
                    StbSecColumnSrcRect.Compare(nameof(StbSecColumn_SRC_Circle), nameof(StbSecColumn_SRC_Rect), key, records);
                }
            }
        }
    }
}

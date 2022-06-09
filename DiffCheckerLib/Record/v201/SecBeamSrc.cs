using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;
using StbSecBarBeamXReinforced = STBridge201.StbSecBarBeamXReinforced;

namespace STBDiffChecker.v201.Records
{
    internal static class SecBeamSrc
    {
        internal static List<Record> Check(ST_BRIDGE stbridgeA, ST_BRIDGE stbridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stbridgeA?.StbModel?.StbSections?.StbSecBeam_SRC;
            var secB = stbridgeB?.StbModel?.StbSections?.StbSecBeam_SRC;
            var setB = secB != null ? new HashSet<StbSecBeam_SRC>(secB) : new HashSet<StbSecBeam_SRC>();

            if (secA != null)
            {
                foreach (var secBeamA in secA)
                {
                    var key = new List<string> { $"Name={secBeamA.name}", $"floor={secBeamA.floor}" };
                    var secBeamB = secB?.FirstOrDefault(n => n.name == secBeamA.name && n.floor == secBeamA.floor);
                    if (secBeamB != null)
                    {
                        CompareSecBeamSrc(stbridgeA, stbridgeB, secBeamA, secBeamB, key, records);
                        setB.Remove(secBeamB);
                    }
                    else
                    {
                        StbSecBeamSrc.Compare(nameof(StbSecBeam_SRC), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"Name={b.name}", $"floor={b.floor}"};
                StbSecBeamSrc.Compare(null, nameof(StbSecBeam_SRC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecBeamSrc(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecBeam_SRC secBeamA, StbSecBeam_SRC secBeamB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBeamSrcId.Compare(secBeamA.id, secBeamB.id, key, records);
            StbSecBeamSrcGuid.Compare(secBeamA.guid, secBeamB.guid, key, records);
            StbSecBeamSrcName.Compare(secBeamA.name, secBeamB.name, key, records);
            StbSecBeamSrcFloor.Compare(secBeamA.floor, secBeamB.floor, key, records);
            StbSecBeamSrcKindBeam.Compare(secBeamA.kind_beam.ToString(), secBeamB.kind_beam.ToString(), key, records);
            StbSecBeamSrcIsFoundation.Compare(secBeamA.isFoundation, secBeamB.isFoundation, key, records);
            StbSecBeamSrcIsCanti.Compare(secBeamA.isCanti, secBeamB.isCanti, key, records);
            StbSecBeamSrcIsOutin.Compare(secBeamA.isOutin, secBeamB.isOutin, key, records);
            StbSecBeamSrcStrengthConcrete.Compare(secBeamA.strength_concrete, secBeamB.strength_concrete, key, records);

            CompareStbSecFigureBeamSrc(secBeamA.StbSecFigureBeam_SRC, secBeamB.StbSecFigureBeam_SRC, key, records);

            if (secBeamA.StbSecBarArrangementBeam_SRC != null || secBeamB.StbSecBarArrangementBeam_SRC != null)
            {
                if (secBeamB.StbSecBarArrangementBeam_SRC == null)
                {
                    StbSecBarArrangementBeamSrc.Compare(nameof(StbSecBarArrangementBeam_SRC), null, key, records);
                }
                else if (secBeamA.StbSecBarArrangementBeam_SRC == null)
                {
                    StbSecBarArrangementBeamSrc.Compare(null, nameof(StbSecBarArrangementBeam_SRC), key, records);
                }
                else
                {
                    CompareStbSecBarArrangementBeamSrc(secBeamA.StbSecBarArrangementBeam_SRC,
                        secBeamB.StbSecBarArrangementBeam_SRC, key, records);
                }
            }

            if (secBeamA.StbSecSteelFigureBeam_SRC != null || secBeamB.StbSecSteelFigureBeam_SRC != null)
            {
                if (secBeamB.StbSecSteelFigureBeam_SRC == null)
                {
                    StbSecSteelFigureBeamSrc.Compare(nameof(StbSecSteelFigureBeam_SRC), null, key, records);
                }
                else if (secBeamA.StbSecSteelFigureBeam_SRC == null)
                {
                    StbSecSteelFigureBeamSrc.Compare(null, nameof(StbSecSteelFigureBeam_SRC), key, records);
                }
                else
                {
                    CompareStbSecSteelFigureBeamSrc(stBridgeA, stBridgeB, secBeamA.StbSecSteelFigureBeam_SRC,
                        secBeamB.StbSecSteelFigureBeam_SRC, key, records);
                }
            }

        }

        private static void CompareStbSecSteelFigureBeamSrc(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecSteelFigureBeam_SRC secA, StbSecSteelFigureBeam_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecSteelFigureBeamSrcOffset.Compare(secA.offsetSpecified, secA.offset, secB.offsetSpecified, secB.offset,
                key, records);
            StbSecSteelFigureBeamSrcLevel.Compare(secA.levelSpecified, secA.level, secB.levelSpecified, secB.level, key, records);
            StbSecSteelFigureBeamSrcJointIdStart.Compare(secA.joint_id_start, stBridgeA, secB.joint_id_start, stBridgeB, key, records);
            StbSecSteelFigureBeamSrcJointIdEnd.Compare(secA.joint_id_end, stBridgeA, secB.joint_id_end, stBridgeB, key, records);

            if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    var a = secA.Items.OfType<StbSecSteelBeam_SRC_Straight>().First();
                    var b = secB.Items.OfType<StbSecSteelBeam_SRC_Straight>().First();
                    StbSecSteelBeamSStraightShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key, records);
                    StbSecSteelBeamSStraightStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                    StbSecSteelBeamSStraightStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_Taper>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_Taper>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_Taper>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { $"pos={a.pos}"};
                        if (b != null)
                        {
                            StbSecSteelBeamSTaperPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSTaperPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSTaperShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSTaperStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSTaperStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_Taper>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { $"pos={b.pos}"};
                            StbSecSteelBeamSTaper.Compare(null, nameof(StbSecSteelBeam_SRC_Taper), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_Joint>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_Joint>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_Joint>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { $"pos={a.pos}"};
                        if (b != null)
                        {
                            StbSecSteelBeamSJointPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSJointPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSJointShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSJointStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSJointStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_Joint>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSJoint.Compare(null, nameof(StbSecSteelBeam_SRC_Joint), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_Haunch>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_Haunch>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_Haunch>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBeamSHaunchPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSHaunchPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSHaunchShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSHaunchStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSHaunchStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_Haunch>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSHaunch.Compare(null, nameof(StbSecSteelBeam_SRC_Haunch), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_FiveTypes>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_FiveTypes>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_FiveTypes>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBeamSFiveTypesPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSFiveTypesPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSFiveTypesShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSFiveTypesStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSFiveTypesStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_FiveTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSFiveTypes.Compare(null, nameof(StbSecSteelBeam_SRC_FiveTypes), keyB, records);
                        }
                    }
                }
            }
        }

        private static void CompareSecSteelFigureBeamSrc(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecSteelFigureBeam_SRC secA, StbSecSteelFigureBeam_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    var a = secA.Items.OfType<StbSecSteelBeam_SRC_Straight>().First();
                    var b = secB.Items.OfType<StbSecSteelBeam_SRC_Straight>().First();
                    StbSecSteelBeamSStraightShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key, records);
                    StbSecSteelBeamSStraightStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                    StbSecSteelBeamSStraightStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_SRC_Straight), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_Taper>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_Taper>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_Taper>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBeamSTaperPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSTaperPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSTaperShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSTaperStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSTaperStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_Taper>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSTaper.Compare(null, nameof(StbSecSteelBeam_SRC_Taper), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_SRC_Taper), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_Joint>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_Joint>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_Joint>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBeamSJointPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSJointPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSJointShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSJointStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSJointStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_Joint>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSJoint.Compare(null, nameof(StbSecSteelBeam_SRC_Joint), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_SRC_Joint), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_Haunch>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_Haunch>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_Haunch>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBeamSHaunchPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSHaunchPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSHaunchShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSHaunchStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSHaunchStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_Haunch>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSHaunch.Compare(null, nameof(StbSecSteelBeam_SRC_Haunch), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_SRC_Haunch), nameof(StbSecSteelBeam_SRC_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Straight))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Taper))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Joint))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_Haunch))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), nameof(StbSecSteelBeam_SRC_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_SRC_FiveTypes))
                {
                    var set = new HashSet<StbSecSteelBeam_SRC_FiveTypes>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_SRC_FiveTypes>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_SRC_FiveTypes>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        if (b != null)
                        {
                            StbSecSteelBeamSFiveTypesPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecSteelBeamSFiveTypesPosName.Compare(a.pos_name, b.pos_name, key1, records);
                            StbSecSteelBeamSFiveTypesShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key1, records);
                            StbSecSteelBeamSFiveTypesStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecSteelBeamSFiveTypesStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_SRC_FiveTypes), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_SRC_FiveTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { "pos=" + b.pos };
                            StbSecSteelBeamSFiveTypes.Compare(null, nameof(StbSecSteelBeam_SRC_FiveTypes), keyB, records);
                        }
                    }
                }
            }
        }

        private static void CompareStbSecBarArrangementBeamSrc(StbSecBarArrangementBeam_SRC secA, StbSecBarArrangementBeam_SRC secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementBeamSrcDepthCoverLeft.Compare(secA.depth_cover_leftSpecified, secA.depth_cover_left,
                secB.depth_cover_leftSpecified, secB.depth_cover_left, key, records);
            StbSecBarArrangementBeamSrcDepthCoverRight.Compare(secA.depth_cover_rightSpecified, secA.depth_cover_right,
                secB.depth_cover_rightSpecified, secB.depth_cover_right, key, records);
            StbSecBarArrangementBeamSrcDepthCoverTop.Compare(secA.depth_cover_topSpecified, secA.depth_cover_top,
                secB.depth_cover_topSpecified, secB.depth_cover_top, key, records);
            StbSecBarArrangementBeamSrcDepthCoverBottom.Compare(secA.depth_cover_bottomSpecified, secA.depth_cover_bottom,
                secB.depth_cover_bottomSpecified, secB.depth_cover_bottom, key, records);
            StbSecBarArrangementBeamSrcInterval.Compare(secA.intervalSpecified, secA.interval, secB.intervalSpecified,
                secB.interval, key, records);
            StbSecBarArrangementBeamSrcCenterTop.Compare(secA.center_topSpecified, secA.center_top,
                secB.center_topSpecified, secB.center_top, key, records);
            StbSecBarArrangementBeamSrcCenterBottom.Compare(secA.center_bottomSpecified, secA.center_bottom,
                secB.center_bottomSpecified, secB.center_bottom, key, records);
            StbSecBarArrangementBeamSrcCenterSide.Compare(secA.center_sideSpecified, secA.center_side,
                secB.center_sideSpecified, secB.center_side, key, records);
            StbSecBarArrangementBeamSrcCenterInterval.Compare(secA.center_intervalSpecified, secA.center_interval,
                secB.center_intervalSpecified, secB.center_interval, key, records);
            StbSecBarArrangementBeamSrcLengthBarStart.Compare(secA.bar_length_startSpecified, secA.bar_length_start,
                secB.bar_length_startSpecified, secB.bar_length_start, key, records);
            StbSecBarArrangementBeamSrcLengthBarEnd.Compare(secA.bar_length_endSpecified, secA.bar_length_end,
                secB.bar_length_endSpecified, secB.bar_length_end, key, records);

            if (secA.Items.Any(n => n is StbSecBarBeam_SRC_Same))
            {
                if (secB.Items.Any(n => n is StbSecBarBeam_SRC_Same))
                {
                    var a = secA.Items.OfType<StbSecBarBeam_SRC_Same>().First();
                    var b = secB.Items.OfType<StbSecBarBeam_SRC_Same>().First();
                    StbSecBarBeamSrcSameDMain.Compare(a.D_main, b.D_main, key, records);
                    StbSecBarBeamSrcSameD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key, records);
                    StbSecBarBeamSrcSameDStirrup.Compare(a.D_stirrup, b.D_stirrup, key, records);
                    StbSecBarBeamSrcSameDWeb.Compare(a.D_web, b.D_web, key, records);
                    StbSecBarBeamSrcSameDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key, records);
                    StbSecBarBeamSrcSameStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                    StbSecBarBeamSrcSameStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key, records);
                    StbSecBarBeamSrcSameStrengthStirrup.Compare(a.strength_stirrup, b.strength_stirrup, key, records);
                    StbSecBarBeamSrcSameStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                    StbSecBarBeamSrcSameStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key, records);
                    StbSecBarBeamSrcSameNMainTop1st.Compare(a.N_main_top_1st, b.N_main_top_1st, key, records);
                    StbSecBarBeamSrcSameNMainTop2nd.Compare(a.N_main_top_2nd, b.N_main_top_2nd, key, records);
                    StbSecBarBeamSrcSameNMainTop3rd.Compare(a.N_main_top_3rd, b.N_main_top_3rd, key, records);
                    StbSecBarBeamSrcSameNMainBottom1st.Compare(a.N_main_bottom_1st, b.N_main_bottom_1st, key, records);
                    StbSecBarBeamSrcSameNMainBottom2nd.Compare(a.N_main_bottom_2nd, b.N_main_bottom_2nd, key, records);
                    StbSecBarBeamSrcSameNMainBottom3rd.Compare(a.N_main_bottom_3rd, b.N_main_bottom_3rd, key, records);
                    StbSecBarBeamSrcSameNMain2ndTop1st.Compare(a.N_2nd_main_top_1st, b.N_2nd_main_top_1st, key, records);
                    StbSecBarBeamSrcSameNMain2ndTop2nd.Compare(a.N_2nd_main_top_2nd, b.N_2nd_main_top_2nd, key, records);
                    StbSecBarBeamSrcSameNMain2ndTop3rd.Compare(a.N_2nd_main_top_3rd, b.N_2nd_main_top_3rd, key, records);
                    StbSecBarBeamSrcSameNMain2ndBottom1st.Compare(a.N_2nd_main_bottom_1st, b.N_2nd_main_bottom_1st, key, records);
                    StbSecBarBeamSrcSameNMain2ndBottom2nd.Compare(a.N_2nd_main_bottom_2nd, b.N_2nd_main_bottom_2nd, key, records);
                    StbSecBarBeamSrcSameNMain2ndBottom3rd.Compare(a.N_2nd_main_bottom_3rd, b.N_2nd_main_bottom_3rd, key, records);
                    StbSecBarBeamSrcSameNStirrup.Compare(a.N_stirrup, b.N_stirrup, key, records);
                    StbSecBarBeamSrcSamePitchStirrup.Compare(a.pitch_stirrup, b.pitch_stirrup, key, records);
                    StbSecBarBeamSrcSameNWeb.Compare(a.N_web, b.N_web, key, records);
                    StbSecBarBeamSrcSameNBarSpacing.Compare(a.N_bar_spacing, b.N_bar_spacing, key, records);
                    StbSecBarBeamSrcSamePitchBarSpacing.Compare(a.pitch_bar_spacingSpecified, a.pitch_bar_spacing,
                        b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_SRC_ThreeTypes))
                {
                    StbSecBarBeamSrcSame.Compare(nameof(StbSecBarBeam_SRC_Same), nameof(StbSecBarBeam_SRC_ThreeTypes), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_SRC_StartEnd))
                {
                    StbSecBarBeamSrcSame.Compare(nameof(StbSecBarBeam_SRC_Same), nameof(StbSecBarBeam_SRC_StartEnd), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarBeam_SRC_ThreeTypes))
            {
                if (secB.Items.Any(n => n is StbSecBarBeam_SRC_Same))
                {
                    StbSecBarBeamSrcThreeTypes.Compare(nameof(StbSecBarBeam_SRC_ThreeTypes), nameof(StbSecBarBeam_SRC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_SRC_ThreeTypes))
                {
                    var set = new HashSet<StbSecBarBeam_SRC_ThreeTypes>();
                    foreach (var a in secA.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarBeamSrcThreeTypesPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarBeamSrcThreeTypesDMain.Compare(a.D_main, b.D_main, key1, records);
                            StbSecBarBeamSrcThreeTypesD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key1, records);
                            StbSecBarBeamSrcThreeTypesDStirrup.Compare(a.D_stirrup, b.D_stirrup, key1, records);
                            StbSecBarBeamSrcThreeTypesDWeb.Compare(a.D_web, b.D_web, key1, records);
                            StbSecBarBeamSrcThreeTypesDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key1, records);
                            StbSecBarBeamSrcThreeTypesStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecBarBeamSrcThreeTypesStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key1, records);
                            StbSecBarBeamSrcThreeTypesStrengthStirrup.Compare(a.strength_stirrup, b.strength_stirrup, key1, records);
                            StbSecBarBeamSrcThreeTypesStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            StbSecBarBeamSrcThreeTypesStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key1, records);
                            StbSecBarBeamSrcThreeTypesNMainTop1st.Compare(a.N_main_top_1st, b.N_main_top_1st, key1, records);
                            StbSecBarBeamSrcThreeTypesNMainTop2nd.Compare(a.N_main_top_2nd, b.N_main_top_2nd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMainTop3rd.Compare(a.N_main_top_3rd, b.N_main_top_3rd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMainBottom1st.Compare(a.N_main_bottom_1st, b.N_main_bottom_1st, key1, records);
                            StbSecBarBeamSrcThreeTypesNMainBottom2nd.Compare(a.N_main_bottom_2nd, b.N_main_bottom_2nd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMainBottom3rd.Compare(a.N_main_bottom_3rd, b.N_main_bottom_3rd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMain2ndTop1st.Compare(a.N_2nd_main_top_1st, b.N_2nd_main_top_1st, key1, records);
                            StbSecBarBeamSrcThreeTypesNMain2ndTop2nd.Compare(a.N_2nd_main_top_2nd, b.N_2nd_main_top_2nd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMain2ndTop3rd.Compare(a.N_2nd_main_top_3rd, b.N_2nd_main_top_3rd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMain2ndBottom1st.Compare(a.N_2nd_main_bottom_1st, b.N_2nd_main_bottom_1st, key1, records);
                            StbSecBarBeamSrcThreeTypesNMain2ndBottom2nd.Compare(a.N_2nd_main_bottom_2nd, b.N_2nd_main_bottom_2nd, key1, records);
                            StbSecBarBeamSrcThreeTypesNMain2ndBottom3rd.Compare(a.N_2nd_main_bottom_3rd, b.N_2nd_main_bottom_3rd, key1, records);
                            StbSecBarBeamSrcThreeTypesNStirrup.Compare(a.N_stirrup, b.N_stirrup, key1, records);
                            StbSecBarBeamSrcThreeTypesPitchStirrup.Compare(a.pitch_stirrup, b.pitch_stirrup, key1, records);
                            StbSecBarBeamSrcThreeTypesNWeb.Compare(a.N_web, b.N_web, key1, records);
                            StbSecBarBeamSrcThreeTypesNBarSpacing.Compare(a.N_bar_spacing, b.N_bar_spacing, key1, records);
                            StbSecBarBeamSrcThreeTypesPitchBarSpacing.Compare(a.pitch_bar_spacingSpecified, a.pitch_bar_spacing,
                                b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarBeamSrcThreeTypes.Compare(null, nameof(StbSecBarBeam_SRC_ThreeTypes), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_SRC_StartEnd))
                {
                    StbSecBarBeamSrcThreeTypes.Compare(nameof(StbSecBarBeam_SRC_ThreeTypes), nameof(StbSecBarBeam_SRC_StartEnd), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBarBeam_SRC_StartEnd))
            {
                if (secB.Items.Any(n => n is StbSecBarBeam_SRC_Same))
                {
                    StbSecBarBeamSrcStartEnd.Compare(nameof(StbSecBarBeam_SRC_StartEnd), nameof(StbSecBarBeam_SRC_Same), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_SRC_ThreeTypes))
                {
                    StbSecBarBeamSrcStartEnd.Compare(nameof(StbSecBarBeam_SRC_StartEnd), nameof(StbSecBarBeam_SRC_ThreeTypes), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBarBeam_SRC_StartEnd))
                {
                    var set = new HashSet<StbSecBarBeam_SRC_StartEnd>();
                    foreach (var a in secA.Items.OfType<StbSecBarBeam_SRC_StartEnd>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBarBeam_SRC_StartEnd>()
                            .FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarBeamSrcStartEndPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarBeamSrcStartEndDMain.Compare(a.D_main, b.D_main, key1, records);
                            StbSecBarBeamSrcStartEndD2ndMain.Compare(a.D_2nd_main, b.D_2nd_main, key1, records);
                            StbSecBarBeamSrcStartEndDStirrup.Compare(a.D_stirrup, b.D_stirrup, key1, records);
                            StbSecBarBeamSrcStartEndDWeb.Compare(a.D_web, b.D_web, key1, records);
                            StbSecBarBeamSrcStartEndDBarSpacing.Compare(a.D_bar_spacing, b.D_bar_spacing, key1, records);
                            StbSecBarBeamSrcStartEndStrengthMain.Compare(a.strength_main, b.strength_main, key1, records);
                            StbSecBarBeamSrcStartEndStrength2ndMain.Compare(a.strength_2nd_main, b.strength_2nd_main, key1, records);
                            StbSecBarBeamSrcStartEndStrengthStirrup.Compare(a.strength_stirrup, b.strength_stirrup, key1, records);
                            StbSecBarBeamSrcStartEndStrengthWeb.Compare(a.strength_web, b.strength_web, key1, records);
                            StbSecBarBeamSrcStartEndStrengthBarSpacing.Compare(a.strength_bar_spacing, b.strength_bar_spacing, key1, records);
                            StbSecBarBeamSrcStartEndNMainTop1st.Compare(a.N_main_top_1st, b.N_main_top_1st, key1, records);
                            StbSecBarBeamSrcStartEndNMainTop2nd.Compare(a.N_main_top_2nd, b.N_main_top_2nd, key1, records);
                            StbSecBarBeamSrcStartEndNMainTop3rd.Compare(a.N_main_top_3rd, b.N_main_top_3rd, key1, records);
                            StbSecBarBeamSrcStartEndNMainBottom1st.Compare(a.N_main_bottom_1st, b.N_main_bottom_1st, key1, records);
                            StbSecBarBeamSrcStartEndNMainBottom2nd.Compare(a.N_main_bottom_2nd, b.N_main_bottom_2nd, key1, records);
                            StbSecBarBeamSrcStartEndNMainBottom3rd.Compare(a.N_main_bottom_3rd, b.N_main_bottom_3rd, key1, records);
                            StbSecBarBeamSrcStartEndNMain2ndTop1st.Compare(a.N_2nd_main_top_1st, b.N_2nd_main_top_1st, key1, records);
                            StbSecBarBeamSrcStartEndNMain2ndTop2nd.Compare(a.N_2nd_main_top_2nd, b.N_2nd_main_top_2nd, key1, records);
                            StbSecBarBeamSrcStartEndNMain2ndTop3rd.Compare(a.N_2nd_main_top_3rd, b.N_2nd_main_top_3rd, key1, records);
                            StbSecBarBeamSrcStartEndNMain2ndBottom1st.Compare(a.N_2nd_main_bottom_1st, b.N_2nd_main_bottom_1st, key1, records);
                            StbSecBarBeamSrcStartEndNMain2ndBottom2nd.Compare(a.N_2nd_main_bottom_2nd, b.N_2nd_main_bottom_2nd, key1, records);
                            StbSecBarBeamSrcStartEndNMain2ndBottom3rd.Compare(a.N_2nd_main_bottom_3rd, b.N_2nd_main_bottom_3rd, key1, records);
                            StbSecBarBeamSrcStartEndNStirrup.Compare(a.N_stirrup, b.N_stirrup, key1, records);
                            StbSecBarBeamSrcStartEndPitchStirrup.Compare(a.pitch_stirrup, b.pitch_stirrup, key1, records);
                            StbSecBarBeamSrcStartEndNWeb.Compare(a.N_web, b.N_web, key1, records);
                            StbSecBarBeamSrcStartEndNBarSpacing.Compare(a.N_bar_spacing, b.N_bar_spacing, key1, records);
                            StbSecBarBeamSrcStartEndPitchBarSpacing.Compare(a.pitch_bar_spacingSpecified, a.pitch_bar_spacing,
                                b.pitch_bar_spacingSpecified, b.pitch_bar_spacing, key1, records);
                            set.Add(b);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBarBeam_SRC_StartEnd>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBarBeamSrcStartEnd.Compare(null, nameof(StbSecBarBeam_SRC_StartEnd), keyB, records);
                        }
                    }
                }
            }

            if (secA.Items.Any(n => n is StbSecBarBeamXReinforced))
            {
                if (secB.Items.Any(n => n is StbSecBarBeamXReinforced))
                {
                    var a = secA.Items.OfType<StbSecBarBeamXReinforced>().First();
                    var b = secA.Items.OfType<StbSecBarBeamXReinforced>().First();
                    StbSecBarBeamXReinforcedNMainTop.Compare(a.N_main_top, b.N_main_top, key, records);
                    StbSecBarBeamXReinforcedNMainBottom.Compare(a.N_main_bottom, b.N_main_bottom, key, records);
                }
                else
                {
                    CheckObjects.StbSecBarBeamXReinforced.Compare(nameof(StbSecBarBeamXReinforced), null, key, records);
                }
            }
            else
            {
                if (secB.Items.Any(n => n is StbSecBarBeamXReinforced))
                {
                    CheckObjects.StbSecBarBeamXReinforced.Compare(null, nameof(StbSecBarBeamXReinforced), key, records);
                }
            }
        }

        private static void CompareStbSecFigureBeamSrc(StbSecFigureBeam_SRC secA, StbSecFigureBeam_SRC secB,
            IReadOnlyList<string> key, List<Record> records)
        {
            if (secA.Items.Any(n => n is StbSecBeam_SRC_Straight))
            {
                if (secB.Items.Any(n => n is StbSecBeam_SRC_Straight))
                {
                    var straightA = secA.Items.OfType<StbSecBeam_SRC_Straight>().First();
                    var straightB = secB.Items.OfType<StbSecBeam_SRC_Straight>().First();
                    StbSecBeamSrcStraightWidth.Compare(straightA.width, straightB.width, key, records);
                    StbSecBeamSrcStraightDepth.Compare(straightA.depth, straightB.depth, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_SRC_Taper))
                {
                    StbSecBeamSrcStraight.Compare(nameof(StbSecBeam_SRC_Straight), nameof(StbSecBeam_SRC_Taper), key,
                        records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_SRC_Haunch))
                {
                    StbSecBeamSrcStraight.Compare(nameof(StbSecBeam_SRC_Straight), nameof(StbSecBeam_SRC_Haunch), key,
                        records);

                }
            }
            else if (secA.Items.Any(n => n is StbSecBeam_SRC_Taper))
            {
                if (secB.Items.Any(n => n is StbSecBeam_SRC_Straight))
                {
                    StbSecBeamSrcTaper.Compare(nameof(StbSecBeam_SRC_Taper), nameof(StbSecBeam_SRC_Straight), key,
                        records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_SRC_Taper))
                {
                    var set = new HashSet<StbSecBeam_SRC_Taper>();
                    foreach (var a in secA.Items.OfType<StbSecBeam_SRC_Taper>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBeamSrcTaperPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBeamSrcTaperWidth.Compare(a.width, b.width, key1, records);
                            StbSecBeamSrcTaperDepth.Compare(a.depth, b.depth, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBeamSrcTaper.Compare(nameof(StbSecBeam_SRC_Taper), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBeam_SRC_Taper>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBeamSrcTaper.Compare(null, nameof(StbSecBeam_SRC_Taper), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecBeam_SRC_Haunch))
                {
                    StbSecBeamSrcTaper.Compare(nameof(StbSecBeam_SRC_Taper), nameof(StbSecBeam_SRC_Haunch), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecBeam_SRC_Haunch))
            {
                if (secB.Items.Any(n => n is StbSecBeam_SRC_Straight))
                {
                    StbSecBeamSrcHaunch.Compare(nameof(StbSecBeam_SRC_Haunch), nameof(StbSecBeam_SRC_Straight), key,
                        records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_SRC_Taper))
                {
                    StbSecBeamSrcHaunch.Compare(nameof(StbSecBeam_SRC_Haunch), nameof(StbSecBeam_SRC_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecBeam_SRC_Haunch))
                {

                    var set = new HashSet<StbSecBeam_SRC_Haunch>();
                    foreach (var a in secA.Items.OfType<StbSecBeam_SRC_Haunch>())
                    {
                        var key1 = new List<string>(key) { "pos=" + a.pos };
                        var b = secB.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBeamSrcHaunchPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBeamSrcHaunchWidth.Compare(a.width, b.width, key1, records);
                            StbSecBeamSrcHaunchDepth.Compare(a.depth, b.depth, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBeamSrcHaunch.Compare(nameof(StbSecBeam_SRC_Haunch), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecBeam_SRC_Haunch>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string>(key) { "pos=" + b.pos };
                            StbSecBeamSrcHaunch.Compare(null, nameof(StbSecBeam_SRC_Haunch), keyB, records);
                        }
                    }
                }
            }
        }
    }
}

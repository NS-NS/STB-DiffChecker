using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    internal static class SecBeamS
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stBridgeA?.StbModel?.StbSections?.StbSecBeam_S;
            var secB = stBridgeB?.StbModel?.StbSections?.StbSecBeam_S;
            var setB = secB != null ? new HashSet<StbSecBeam_S>(secB) : new HashSet<StbSecBeam_S>();

            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() {$"Name={secColumnA.name}", $"floor={secColumnA.floor}"};
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name && n.floor == secColumnA.floor);
                    if (secColumnB != null)
                    {
                        CompareSecBeamS(stBridgeA, stBridgeB, secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        StbSecBeamS.Compare(nameof(StbSecBeam_S), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var keyB = new List<string> {$"Name={b.name}", $"floor={b.floor}"};
                StbSecBeamS.Compare(null, nameof(StbSecBeam_S), keyB, records);

            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecBeamS(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecBeam_S secA, StbSecBeam_S secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBeamSId.Compare(secA.id, secB.id, key, records);
            StbSecBeamSGuid.Compare(secA.guid, secB.guid, key, records);
            StbSecBeamSName.Compare(secA.name, secB.name, key, records);
            StbSecBeamSFloor.Compare(secA.floor, secB.floor, key, records);
            StbSecBeamSKindBeam.Compare(secA.kind_beam.ToString(), secB.kind_beam.ToString(), key, records);
            StbSecBeamSIsCanti.Compare(secA.isCanti, secB.isCanti, key, records);
            StbSecBeamSIsOutin.Compare(secA.isOutin, secB.isOutin, key, records);

            CompareSecSteelFigureBeamS(stBridgeA, stBridgeB, secA.StbSecSteelFigureBeam_S,
                secB.StbSecSteelFigureBeam_S, key, records);
        }

        private static void CompareSecSteelFigureBeamS(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecSteelFigureBeam_S secA, StbSecSteelFigureBeam_S secB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecSteelFigureBeamSJointIdStart.Compare(secA.joint_id_start, stBridgeA, secB.joint_id_start, stBridgeB, key, records);
            StbSecSteelFigureBeamSJointIdEnd.Compare(secA.joint_id_end, stBridgeA, secB.joint_id_end, stBridgeB, key, records);

            if (secA.Items.Any(n => n is StbSecSteelBeam_S_Straight))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_S_Straight))
                {
                    var a = secA.Items.OfType<StbSecSteelBeam_S_Straight>().First();
                    var b = secB.Items.OfType<StbSecSteelBeam_S_Straight>().First();
                    StbSecSteelBeamSStraightShape.Compare(a.shape, stBridgeA, b.shape, stBridgeB, key, records);
                    StbSecSteelBeamSStraightStrengthMain.Compare(a.strength_main, b.strength_main, key, records);
                    StbSecSteelBeamSStraightStrengthWeb.Compare(a.strength_web, b.strength_web, key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Taper))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_S_Straight), nameof(StbSecSteelBeam_S_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Joint))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_S_Straight), nameof(StbSecSteelBeam_S_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Haunch))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_S_Straight), nameof(StbSecSteelBeam_S_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_FiveTypes))
                {
                    StbSecSteelBeamSStraight.Compare(nameof(StbSecSteelBeam_S_Straight), nameof(StbSecSteelBeam_S_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_S_Taper))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_S_Straight))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_S_Taper), nameof(StbSecSteelBeam_S_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Taper))
                {
                    var set = new HashSet<StbSecSteelBeam_S_Taper>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_S_Taper>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_S_Taper>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { $"pos={a.pos}" };
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
                            StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_S_Taper), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_S_Taper>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> {$"pos={b.pos}"};
                            StbSecSteelBeamSTaper.Compare(null, nameof(StbSecSteelBeam_S_Taper), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Joint))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_S_Taper), nameof(StbSecSteelBeam_S_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Haunch))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_S_Taper), nameof(StbSecSteelBeam_S_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_FiveTypes))
                {
                    StbSecSteelBeamSTaper.Compare(nameof(StbSecSteelBeam_S_Taper), nameof(StbSecSteelBeam_S_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_S_Joint))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_S_Straight))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_S_Joint), nameof(StbSecSteelBeam_S_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Taper))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_S_Joint), nameof(StbSecSteelBeam_S_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Joint))
                {
                    var set = new HashSet<StbSecSteelBeam_S_Joint>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_S_Joint>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_S_Joint>().FirstOrDefault(n => n.pos == a.pos);
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
                            StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_S_Joint), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_S_Joint>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { $"pos={b.pos}"};
                            StbSecSteelBeamSJoint.Compare(null, nameof(StbSecSteelBeam_S_Joint), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Haunch))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_S_Joint), nameof(StbSecSteelBeam_S_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_FiveTypes))
                {
                    StbSecSteelBeamSJoint.Compare(nameof(StbSecSteelBeam_S_Joint), nameof(StbSecSteelBeam_S_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_S_Haunch))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_S_Straight))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_S_Haunch), nameof(StbSecSteelBeam_S_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Taper))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_S_Haunch), nameof(StbSecSteelBeam_S_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Joint))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_S_Haunch), nameof(StbSecSteelBeam_S_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Haunch))
                {
                    var set = new HashSet<StbSecSteelBeam_S_Haunch>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_S_Haunch>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_S_Haunch>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { $"pos={a.pos}"};
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
                            StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_S_Haunch), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_S_Haunch>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { $"pos={b.pos}"};
                            StbSecSteelBeamSHaunch.Compare(null, nameof(StbSecSteelBeam_S_Haunch), keyB, records);
                        }
                    }
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_FiveTypes))
                {
                    StbSecSteelBeamSHaunch.Compare(nameof(StbSecSteelBeam_S_Haunch), nameof(StbSecSteelBeam_S_FiveTypes), key, records);
                }
            }
            else if (secA.Items.Any(n => n is StbSecSteelBeam_S_FiveTypes))
            {
                if (secB.Items.Any(n => n is StbSecSteelBeam_S_Straight))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_S_FiveTypes), nameof(StbSecSteelBeam_S_Straight), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Taper))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_S_FiveTypes), nameof(StbSecSteelBeam_S_Taper), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Joint))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_S_FiveTypes), nameof(StbSecSteelBeam_S_Joint), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_Haunch))
                {
                    StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_S_FiveTypes), nameof(StbSecSteelBeam_S_Haunch), key, records);
                }
                else if (secB.Items.Any(n => n is StbSecSteelBeam_S_FiveTypes))
                {
                    var set = new HashSet<StbSecSteelBeam_S_FiveTypes>();
                    foreach (var a in secA.Items.OfType<StbSecSteelBeam_S_FiveTypes>())
                    {
                        var b = secB.Items.OfType<StbSecSteelBeam_S_FiveTypes>().FirstOrDefault(n => n.pos == a.pos);
                        var key1 = new List<string>(key) { $"pos={a.pos}"};
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
                            StbSecSteelBeamSFiveTypes.Compare(nameof(StbSecSteelBeam_S_FiveTypes), null, key1, records);
                        }
                    }

                    foreach (var b in secB.Items.OfType<StbSecSteelBeam_S_FiveTypes>())
                    {
                        if (!set.Contains(b))
                        {
                            var keyB = new List<string> { $"pos={b.pos}" };
                            StbSecSteelBeamSFiveTypes.Compare(null, nameof(StbSecSteelBeam_S_FiveTypes), keyB, records);
                        }
                    }
                }
            }
        }
    }
}

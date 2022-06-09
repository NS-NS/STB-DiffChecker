using STBridge201;
using System.Collections.Generic;
using System.Linq;
using static STBDiffChecker.v201.CheckObjects;

namespace STBDiffChecker.v201.Records
{
    static class SecWallRc
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();

            var secA = stBridgeA?.StbModel?.StbSections?.StbSecWall_RC;
            var tempB = stBridgeB?.StbModel?.StbSections?.StbSecWall_RC;
            var secB = tempB != null ? new HashSet<StbSecWall_RC>(tempB) : new HashSet<StbSecWall_RC>();

            if (secA != null)
            {
                foreach (var secWallA in secA)
                {
                    var key = new List<string>() {"Name=" + secWallA.name};
                    var secWallB = tempB?.FirstOrDefault(n => n.name == secWallA.name);
                    if (secWallB != null)
                    {
                        CompareSecWallRc(secWallA, secWallB, key, records);
                        secB.Remove(secWallB);
                    }
                    else
                    {
                        StbSecWallRc.Compare(nameof(StbSecWall_RC), null, key, records);
                    }
                }
            }

            foreach (var b in secB)
            {
                var key = new List<string> {"Name=" + b.name};
                StbSecWallRc.Compare(null, nameof(StbSecWall_RC), key, records);
            }

            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecWallRc(StbSecWall_RC secWallA, StbSecWall_RC secWallB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecWallRcId.Compare(secWallA.id, secWallB.id, key, records);
            StbSecWallRcGuid.Compare(secWallA.guid, secWallB.guid, key, records);
            StbSecWallRcName.Compare(secWallA.name, secWallB.name, key, records);
            StbSecWallRcStrengthConcrete.Compare(secWallA.strength_concrete, secWallB.strength_concrete, key, records);

            StbSecWallRcStraightT.Compare(secWallA.StbSecFigureWall_RC.StbSecWall_RC_Straight.t, secWallB.StbSecFigureWall_RC.StbSecWall_RC_Straight.t, key, records);

            if (secWallA.StbSecBarArrangementWall_RC != null || secWallB.StbSecBarArrangementWall_RC != null)
            {
                if (secWallB.StbSecBarArrangementWall_RC == null)
                {
                    StbSecBarArrangementWallRc.Compare(nameof(StbSecBarArrangementWall_RC), null, key, records);
                }
                else if (secWallA.StbSecBarArrangementWall_RC == null)
                {
                    StbSecBarArrangementWallRc.Compare(null, nameof(StbSecBarArrangementWall_RC), key, records);
                }
                else
                {
                    CompareSecBarArrangementWallRc(secWallA.StbSecBarArrangementWall_RC,
                        secWallB.StbSecBarArrangementWall_RC, key, records);
                }
            }
        }

        private static void CompareSecBarArrangementWallRc(StbSecBarArrangementWall_RC wallA, StbSecBarArrangementWall_RC wallB, IReadOnlyList<string> key, List<Record> records)
        {
            StbSecBarArrangementWallRcDepthCoverOutside.Compare(wallA.depth_cover_outsideSpecified,
                wallA.depth_cover_outside, wallB.depth_cover_outsideSpecified, wallB.depth_cover_outside, key, records);
            StbSecBarArrangementWallRcDepthCoverInside.Compare(wallA.depth_cover_insideSpecified,
                wallA.depth_cover_inside, wallB.depth_cover_insideSpecified, wallB.depth_cover_inside, key, records);

            if (wallA.Items.Any(n => n is StbSecBarWall_RC_Single))
            {
                if (wallB.Items.Any(n => n is StbSecBarWall_RC_Single))
                {
                    var set = new HashSet<StbSecBarWall_RC_Single>();
                    foreach (var a in wallA.Items.OfType<StbSecBarWall_RC_Single>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = wallB.Items.OfType<StbSecBarWall_RC_Single>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarWallRcSinglePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarWallRcSingleStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarWallRcSingleD.Compare(a.D, b.D, key1, records);
                            StbSecBarWallRcSinglePitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarWallRcSingle.Compare(nameof(StbSecBarWall_RC_Single), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarWall_RC_Single>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarWallRcSingle.Compare(null, nameof(StbSecBarWall_RC_Single), key1, records);
                        }
                    }
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_Zigzag))
                {
                    StbSecBarWallRcSingle.Compare(nameof(StbSecBarWall_RC_Single), nameof(StbSecBarWall_RC_Zigzag), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_DoubleNet))
                {
                    StbSecBarWallRcSingle.Compare(nameof(StbSecBarWall_RC_Single), nameof(StbSecBarWall_RC_DoubleNet), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_InsideAndOutside))
                {
                    StbSecBarWallRcSingle.Compare(nameof(StbSecBarWall_RC_Single), nameof(StbSecBarWall_RC_InsideAndOutside), key, records);
                }
            }
            else if (wallA.Items.Any(n => n is StbSecBarWall_RC_Zigzag))
            {
                if (wallB.Items.Any(n => n is StbSecBarWall_RC_Single))
                {
                    StbSecBarWallRcZigzag.Compare(nameof(StbSecBarWall_RC_Zigzag), nameof(StbSecBarWall_RC_Single), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_Zigzag))
                {
                    var set = new HashSet<StbSecBarWall_RC_Zigzag>();
                    foreach (var a in wallA.Items.OfType<StbSecBarWall_RC_Zigzag>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = wallB.Items.OfType<StbSecBarWall_RC_Zigzag>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarWallRcZigzagPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarWallRcZigzagStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarWallRcZigzagD.Compare(a.D, b.D, key1, records);
                            StbSecBarWallRcZigzagPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarWallRcZigzag.Compare(nameof(StbSecBarWall_RC_Zigzag), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarWall_RC_Zigzag>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarWallRcZigzag.Compare(null, nameof(StbSecBarWall_RC_Zigzag), key1, records);
                        }
                    }
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_DoubleNet))
                {
                    StbSecBarWallRcZigzag.Compare(nameof(StbSecBarWall_RC_Zigzag), nameof(StbSecBarWall_RC_DoubleNet), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_InsideAndOutside))
                {
                    StbSecBarWallRcZigzag.Compare(nameof(StbSecBarWall_RC_Zigzag), nameof(StbSecBarWall_RC_InsideAndOutside), key, records);
                }
            }
            else if (wallA.Items.Any(n => n is StbSecBarWall_RC_DoubleNet))
            {
                if (wallB.Items.Any(n => n is StbSecBarWall_RC_Single))
                {
                    StbSecBarWallRcDoubleNet.Compare(nameof(StbSecBarWall_RC_DoubleNet), nameof(StbSecBarWall_RC_Single), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_Zigzag))
                {
                    StbSecBarWallRcDoubleNet.Compare(nameof(StbSecBarWall_RC_DoubleNet), nameof(StbSecBarWall_RC_Zigzag), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_DoubleNet))
                {
                    var set = new HashSet<StbSecBarWall_RC_DoubleNet>();
                    foreach (var a in wallA.Items.OfType<StbSecBarWall_RC_DoubleNet>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = wallB.Items.OfType<StbSecBarWall_RC_DoubleNet>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarWallRcDoubleNetPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarWallRcDoubleNetStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarWallRcDoubleNetD.Compare(a.D, b.D, key1, records);
                            StbSecBarWallRcDoubleNetPitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarWallRcDoubleNet.Compare(nameof(StbSecBarWall_RC_DoubleNet), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarWall_RC_DoubleNet>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarWallRcDoubleNet.Compare(null, nameof(StbSecBarWall_RC_DoubleNet), key1, records);
                        }
                    }
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_InsideAndOutside))
                {
                    StbSecBarWallRcDoubleNet.Compare(nameof(StbSecBarWall_RC_DoubleNet), nameof(StbSecBarWall_RC_InsideAndOutside), key, records);
                }
            }
            else if (wallA.Items.Any(n => n is StbSecBarWall_RC_InsideAndOutside))
            {
                if (wallB.Items.Any(n => n is StbSecBarWall_RC_Single))
                {
                    StbSecBarWallRcInsideAndOutside.Compare(nameof(StbSecBarWall_RC_InsideAndOutside), nameof(StbSecBarWall_RC_Single), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_Zigzag))
                {
                    StbSecBarWallRcInsideAndOutside.Compare(nameof(StbSecBarWall_RC_InsideAndOutside), nameof(StbSecBarWall_RC_Zigzag), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_DoubleNet))
                {
                    StbSecBarWallRcInsideAndOutside.Compare(nameof(StbSecBarWall_RC_InsideAndOutside), nameof(StbSecBarWall_RC_DoubleNet), key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_InsideAndOutside))
                {
                    var set = new HashSet<StbSecBarWall_RC_InsideAndOutside>();
                    foreach (var a in wallA.Items.OfType<StbSecBarWall_RC_InsideAndOutside>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos, "pos2=" + a.pos2};
                        var b = wallB.Items.OfType<StbSecBarWall_RC_InsideAndOutside>().FirstOrDefault(n => n.pos == a.pos && n.pos2 == a.pos2);
                        if (b != null)
                        {
                            StbSecBarWallRcInsideAndOutsidePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarWallRcInsideAndOutsidePos2.Compare(a.pos2.ToString(), b.pos2.ToString(), key1, records);
                            StbSecBarWallRcInsideAndOutsideStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarWallRcInsideAndOutsideD.Compare(a.D, b.D, key1, records);
                            StbSecBarWallRcInsideAndOutsidePitch.Compare(a.pitch, b.pitch, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarWallRcInsideAndOutside.Compare(nameof(StbSecBarWall_RC_InsideAndOutside), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarWall_RC_InsideAndOutside>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarWallRcInsideAndOutside.Compare(null, nameof(StbSecBarWall_RC_InsideAndOutside), key1, records);
                        }
                    }
                }
            }

            if (wallA.Items.Any(n => n is StbSecBarWall_RC_Edge))
            {
                if (!wallB.Items.Any(n => n is StbSecBarWall_RC_Edge))
                {
                    StbSecBarWallRcEdge.Compare(nameof(StbSecBarWall_RC_Edge), null, key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_Edge))
                {
                    var set = new HashSet<StbSecBarWall_RC_Edge>();
                    foreach (var a in wallA.Items.OfType<StbSecBarWall_RC_Edge>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = wallB.Items.OfType<StbSecBarWall_RC_Edge>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarWallRcEdgePos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarWallRcEdgeStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarWallRcEdgeD.Compare(a.D, b.D, key1, records);
                            StbSecBarWallRcEdgeN.Compare(a.N, b.N, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarWallRcEdge.Compare(nameof(StbSecBarWall_RC_Edge), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarWall_RC_Edge>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarWallRcEdge.Compare(null, nameof(StbSecBarWall_RC_Edge), key1, records);
                        }
                    }
                }
            }
            else
            {
                if (wallB.Items.Any(n => n is StbSecBarWall_RC_Edge))
                {
                    StbSecBarWallRcEdge.Compare(null, nameof(StbSecBarWall_RC_Edge), key, records);
                }
            }

            if (wallA.Items.Any(n => n is StbSecBarWall_RC_Open))
            {
                if (!wallB.Items.Any(n => n is StbSecBarWall_RC_Open))
                {
                    StbSecBarWallRcOpen.Compare(nameof(StbSecBarWall_RC_Open), null, key, records);
                }
                else if (wallB.Items.Any(n => n is StbSecBarWall_RC_Open))
                {
                    var set = new HashSet<StbSecBarWall_RC_Open>();
                    foreach (var a in wallA.Items.OfType<StbSecBarWall_RC_Open>())
                    {
                        var key1 = new List<string>(key) {"pos=" + a.pos};
                        var b = wallB.Items.OfType<StbSecBarWall_RC_Open>().FirstOrDefault(n => n.pos == a.pos);
                        if (b != null)
                        {
                            StbSecBarWallRcOpenPos.Compare(a.pos.ToString(), b.pos.ToString(), key1, records);
                            StbSecBarWallRcOpenStrength.Compare(a.strength, b.strength, key1, records);
                            StbSecBarWallRcOpenD.Compare(a.D, b.D, key1, records);
                            StbSecBarWallRcOpenN.Compare(a.N, b.N, key1, records);
                            StbSecBarWallRcOpenLength.Compare(a.lengthSpecified, a.length, b.lengthSpecified, b.length, key1, records);
                            set.Add(b);
                        }
                        else
                        {
                            StbSecBarWallRcOpen.Compare(nameof(StbSecBarWall_RC_Open), null, key1, records);
                        }
                    }

                    foreach (var b in wallB.Items.OfType<StbSecBarWall_RC_Open>())
                    {
                        if (!set.Contains(b))
                        {
                            var key1 = new List<string>(key) {"pos=" + b.pos};
                            StbSecBarWallRcOpen.Compare(null, nameof(StbSecBarWall_RC_Open), key1, records);
                        }
                    }
                }
            }
            else
            {
                if (wallB.Items.Any(n => n is StbSecBarWall_RC_Open))
                {
                    StbSecBarWallRcOpen.Compare(null, nameof(StbSecBarWall_RC_Open), key, records);
                }
            }
        }
    }
}

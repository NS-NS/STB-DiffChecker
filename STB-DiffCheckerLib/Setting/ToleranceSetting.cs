using STBDiffChecker.v201.Records;
using System;
using System.Collections.Generic;
using System.Data;

namespace STBDiffChecker
{
    /// <summary>
    /// 許容差用クラス
    /// </summary>
    internal class ToleranceSetting
    {
        internal List<UserTolerance> tolerances = new List<UserTolerance>()
        {
            new UserTolerance("柱(StbColumn)"),
            new UserTolerance("間柱(StbPost)"),
            new UserTolerance("大梁(StbGirder)"),
            new UserTolerance("小梁(StbBeam)"),
            new UserTolerance("ブレース(StbBrace)"),
            new UserTolerance("スラブ(Stbslab)"),
            new UserTolerance("壁(StbWall)"),
            new UserTolerance("開口(StbOpen)")
        };

        internal DataTable CreateTable()
        {
            DataTable toleranceTable = new DataTable();
            var name = toleranceTable.Columns.Add("Name", typeof(string));
            name.ReadOnly = true;
            toleranceTable.Columns.Add("Node", typeof(double));
            toleranceTable.Columns.Add("Offset", typeof(double));
            foreach (var tolerance in this.tolerances)
            {
                toleranceTable.Rows.Add(tolerance.Name, tolerance.Node, tolerance.Offset);
            }

            return toleranceTable;
        }

        internal void ImportCsv(List<string> csv)
        {
            foreach (var line in csv)
            {
                var split = line.Split(',');
                try
                {
                    //var tolerance = this.tolerances.First(n => n.Name == split[0]);
                    //tolerance.Node = double.Parse(split[1]);
                    //tolerance.Offset = double.Parse(split[2]);
                    foreach ( var tolerance in tolerances)
                    {
                        if( tolerance.Name==split[0])
                        {
                            tolerance.Node = double.Parse(split[1]);
                            tolerance.Offset = double.Parse(split[2]);
                            break;
                        }
                    }
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }
            #region デバッグ
            //foreach(var i in tolerances)
            //{
            //    Console.WriteLine($"Name={i.Name},Node={i.Node},Offset={i.Offset}");
            //}
            #endregion
        }

        internal void Export()
        {
            foreach (var tolerance in tolerances)
            {
                
            }
        }

        internal void Apply()
        {
            if (tolerances[0].Node > Utility.Tolerance)
                Columns.analysisMargin = tolerances[0].Node;
            if (tolerances[0].Offset > Utility.Tolerance)
                Columns.offsetMargin = tolerances[0].Offset;
            if (tolerances[1].Node > Utility.Tolerance)
                Posts.analysisMargin = tolerances[1].Node;
            if (tolerances[1].Offset > Utility.Tolerance)
                Posts.offsetMargin = tolerances[1].Offset;
            if (tolerances[2].Node > Utility.Tolerance)
                Girders.analysisMargin = tolerances[2].Node;
            if (tolerances[2].Offset > Utility.Tolerance)
                Girders.offsetMargin = tolerances[2].Offset;
            if (tolerances[3].Node > Utility.Tolerance)
                Beams.analysisMargin = tolerances[3].Node;
            if (tolerances[3].Offset > Utility.Tolerance)
                Beams.offsetMargin = tolerances[3].Offset;
            if (tolerances[4].Node > Utility.Tolerance)
                Braces.analysisMargin = tolerances[4].Node;
            if (tolerances[4].Offset > Utility.Tolerance)
                Braces.offsetMargin = tolerances[4].Offset;
            if (tolerances[5].Node > Utility.Tolerance)
                Slabs.analysisMargin = tolerances[5].Node;
            if (tolerances[5].Offset > Utility.Tolerance)
                Slabs.offsetMargin = tolerances[5].Offset;
            if (tolerances[6].Node > Utility.Tolerance)
                Walls.analysisMargin = tolerances[6].Node;
            if (tolerances[6].Offset > Utility.Tolerance)
                Walls.offsetMargin = tolerances[6].Offset;
        }
    }
}
using STBridge201;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace STBDiffChecker.v201.Records
{
    public class TotalRecord
    {
        internal List<KeyValuePair<string, string>> item = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string,string>(),
            new KeyValuePair<string,string>(),
            new KeyValuePair<string,string>(),
            new KeyValuePair<string,string>(),
            new KeyValuePair<string,string>(),
            new KeyValuePair<string,string>()
        };

        public List<RecordTab> recordTabs = new List<RecordTab>();

        internal ResultFormSetting resultFormSetting;
        internal ST_BRIDGE stbridgeA;
        internal ST_BRIDGE stbridgeB;
        public Summary Summary = new Summary();
        internal RecordTab CommonRecord = new RecordTab("DataGidCommon", "共通");
        internal RecordTab NodesRecord = new RecordTab("DataGidNodes", "節点");
        internal RecordTab ParallelAxesRecord = new RecordTab("DataGidParallelAxes", "平行軸");
        internal RecordTab ArcAxesRecord = new RecordTab("DataGidArcAxes", "円弧軸");
        internal RecordTab RadialAxesRecord = new RecordTab("DataGidRadialAxes", "放射軸");
        internal RecordTab DrawingLineAxis = new RecordTab("DataGidDrawingLineAxis", "作図用直線軸");
        internal RecordTab DrawingArcAxis = new RecordTab("DataGidDrawingArcAxis", "作図用円弧軸");
        internal RecordTab StoriesRecord = new RecordTab("DataGidStories", "階");
        internal RecordTab ColumnsRecord = new RecordTab("DataGidColumns", "柱");
        internal RecordTab PostsRecord = new RecordTab("DataGidPosts", "間柱");
        internal RecordTab GirdersRecord = new RecordTab("DataGidGirders", "大梁");
        internal RecordTab BeamsRecord = new RecordTab("DataGidBeams", "小梁");
        internal RecordTab BracesRecord = new RecordTab("DataGidBraces", "ブレース");
        internal RecordTab SlabsRecord = new RecordTab("DataGidSlabs", "スラブ");
        internal RecordTab WallsRecord = new RecordTab("DataGidWalls", "壁");
        internal RecordTab FootingsRecord = new RecordTab("DataGidFootings", "フーチング");
        internal RecordTab StripFootingsRecord = new RecordTab("DataGidStripFootings", "布基礎");
        internal RecordTab PilesRecord = new RecordTab("DataGidPiles", "杭");
        internal RecordTab FoundationColumnsRecord = new RecordTab("DataGidFoundationColumns", "基礎柱");
        internal RecordTab ParapetsRecord = new RecordTab("DataGidParapets", "パラペット");
        internal RecordTab SecColumnRcRecord = new RecordTab("DataGidSecColumnRc", "柱断面(RC)");
        internal RecordTab SecColumnSRecord = new RecordTab("DataGidSecColumnS", "柱断面(S)");
        internal RecordTab SecColumnSrcRecord = new RecordTab("DataGidSecColumnSrc", "柱断面(SRC)");
        internal RecordTab SecColumnCftRecord = new RecordTab("DataGidSecColumnCft", "柱断面(CFT)");
        internal RecordTab SecBeamRcRecord = new RecordTab("DataGidSecBeamRcRecord", "梁断面(RC)");
        internal RecordTab SecBeamSRecord = new RecordTab("DataGidSecBeamSRecord", "梁断面(S)");
        internal RecordTab SecBeamSrcRecord = new RecordTab("DataGidSecBeamSrcRecord", "梁断面(SRC)");
        internal RecordTab SecBraceSRecord = new RecordTab("DataGidSecBraceSRecord", "ブレース断面(S)");
        internal RecordTab SecSlabRcRecord = new RecordTab("DataGidSecSlabRcRecord", "スラブ断面(RC)");
        internal RecordTab SecSlabDeckRecord = new RecordTab("DataGidSecSlabDeckRecord", "スラブ断面(デッキ)");
        internal RecordTab SecSlabPrecastRecord = new RecordTab("DataGidSecSlabPrecastRecord", "スラブ断面(プレキャスト)");
        internal RecordTab SecWallRcRecord = new RecordTab("DataGidSecWallRcRecord", "壁断面(RC)");
        internal RecordTab SecFoundationRcRecord = new RecordTab("DataGidSecFoundationRcRecord", "基礎断面(RC)");
        internal RecordTab SecPileRcRecord = new RecordTab("DataGidSecPileRcRecord", "杭断面(RC)");
        internal RecordTab SecPileSRecord = new RecordTab("DataGidSecPileSRecord", "杭断面(S)");
        internal RecordTab SecPileProductRecord = new RecordTab("DataGidSecPileProductRecord", "杭断面(既成)");
        internal RecordTab SecParapetRcRecord = new RecordTab("DataGidSecParapetRcRecord", "パラペット断面(RC)");

        public TotalRecord(ResultFormSetting resultFormSetting)
        {
            Summary.dateTime = DateTime.Now;
            this.resultFormSetting = resultFormSetting;
            stbridgeA = Deserialize(resultFormSetting.PathA);
            stbridgeB = Deserialize(resultFormSetting.PathB);
            recordTabs = new List<RecordTab>()
            {
                CommonRecord,
                NodesRecord,
                ParallelAxesRecord,
                ArcAxesRecord,
                RadialAxesRecord,
                DrawingLineAxis,
                DrawingArcAxis,
                StoriesRecord,
                ColumnsRecord,
                PostsRecord,
                GirdersRecord,
                BeamsRecord,
                BracesRecord,
                SlabsRecord,
                WallsRecord,
                FootingsRecord,
                StripFootingsRecord,
                PilesRecord,
                FoundationColumnsRecord,
                ParapetsRecord,
                SecColumnRcRecord,
                SecColumnSRecord,
                SecColumnSrcRecord,
                SecColumnCftRecord,
                SecBeamRcRecord,
                SecBeamSRecord,
                SecBeamSrcRecord,
                SecBraceSRecord,
                SecSlabRcRecord,
                SecSlabDeckRecord,
                SecSlabPrecastRecord,
                SecWallRcRecord,
                SecFoundationRcRecord,
                SecPileRcRecord,
                SecPileSRecord,
                SecPileProductRecord,
                SecParapetRcRecord
            };
        }

        static ST_BRIDGE Deserialize(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(ST_BRIDGE));
                return (ST_BRIDGE)serializer.Deserialize(sr);
            }
        }

        public void Run()
        {
            ApplySetting();

            CommonRecord.records = Common.Check(stbridgeA, stbridgeB);
            NodesRecord.records = Nodes.Check(stbridgeA, stbridgeB);
            ParallelAxesRecord.records = ParallelAxes.Check(stbridgeA, stbridgeB);
            ArcAxesRecord.records = ArcAxes.Check(stbridgeA, stbridgeB);
            RadialAxesRecord.records = RadialAxes.Check(stbridgeA, stbridgeB);
            DrawingLineAxis.records = Records.DrawingLineAxis.Check(stbridgeA, stbridgeB);
            DrawingArcAxis.records = Records.DrawingArcAxis.Check(stbridgeA, stbridgeB);
            StoriesRecord.records = Stories.Check(stbridgeA, stbridgeB);
            ColumnsRecord.records = Columns.Check(stbridgeA, stbridgeB);
            PostsRecord.records = Posts.Check(stbridgeA, stbridgeB);
            GirdersRecord.records = Girders.Check(stbridgeA, stbridgeB);
            BeamsRecord.records = Beams.Check(stbridgeA, stbridgeB);
            BracesRecord.records = Braces.Check(stbridgeA, stbridgeB);
            SlabsRecord.records = Slabs.Check(stbridgeA, stbridgeB);
            WallsRecord.records = Walls.Check(stbridgeA, stbridgeB);
            FootingsRecord.records = Footings.Check(stbridgeA, stbridgeB);
            StripFootingsRecord.records = StripFootings.Check(stbridgeA, stbridgeB);
            PilesRecord.records = Piles.Check(stbridgeA, stbridgeB);
            FoundationColumnsRecord.records = FoundationColumns.Check(stbridgeA, stbridgeB);
            ParapetsRecord.records = Parapets.Check(stbridgeA, stbridgeB);

            SecColumnRcRecord.records = SecColumnRc.Check(stbridgeA, stbridgeB);
            SecColumnSRecord.records = SecColumnS.Check(stbridgeA, stbridgeB);
            SecColumnSrcRecord.records = SecColumnSrc.Check(stbridgeA, stbridgeB);
            SecColumnCftRecord.records = SecColumnCft.Check(stbridgeA, stbridgeB);
            SecBeamRcRecord.records = SecBeamRc.Check(stbridgeA, stbridgeB);
            SecBeamSRecord.records = SecBeamS.Check(stbridgeA, stbridgeB);
            SecBeamSrcRecord.records = SecBeamSrc.Check(stbridgeA, stbridgeB);
            SecBraceSRecord.records = SecBraceS.Check(stbridgeA, stbridgeB);
            SecSlabRcRecord.records = SecSlabRc.Check(stbridgeA, stbridgeB);
            SecSlabDeckRecord.records = SecSlabDeck.Check(stbridgeA, stbridgeB);
            SecSlabPrecastRecord.records = SecSlabPrecast.Check(stbridgeA, stbridgeB);
            SecWallRcRecord.records = SecWallRc.Check(stbridgeA, stbridgeB);
            SecFoundationRcRecord.records = SecFoundationRc.Check(stbridgeA, stbridgeB);
            SecPileRcRecord.records = SecPileRc.Check(stbridgeA, stbridgeB);
            SecPileSRecord.records = SecPileS.Check(stbridgeA, stbridgeB);
            SecPileProductRecord.records = SecPileProduct.Check(stbridgeA, stbridgeB);
            SecParapetRcRecord.records = SecParapetRc.Check(stbridgeA, stbridgeB);

            Summary = CreateSummary();
        }

        private void ApplySetting()
        {
            this.resultFormSetting.toleranceSetting.Apply();
        }


        internal Summary CreateSummary()
        {
            if (this.NodesRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.NodesRecord.HeaderName, this.stbridgeA?.StbModel?.StbNodes?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbNodes?.Length ?? 0, this.NodesRecord.records));
            }

            if (this.ParallelAxesRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.ParallelAxesRecord.HeaderName, this.stbridgeA?.StbModel?.StbAxes?.StbParallelAxes?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbAxes?.StbParallelAxes?.Length ?? 0, this.ParallelAxesRecord.records));
            }

            if (this.ArcAxesRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.ArcAxesRecord.HeaderName, this.stbridgeA?.StbModel?.StbAxes?.StbArcAxes?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbAxes?.StbArcAxes?.Length ?? 0, this.ArcAxesRecord.records));
            }

            if (this.RadialAxesRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.RadialAxesRecord.HeaderName, this.stbridgeA?.StbModel?.StbAxes?.StbRadialAxes?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbAxes?.StbRadialAxes?.Length ?? 0, this.RadialAxesRecord.records));
            }

            if (this.DrawingLineAxis.records != null)
            {
                Summary.Rows.Add(Summary.CollectResult(this.DrawingLineAxis.HeaderName, this.stbridgeA?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis?.Length ?? 0,
                    this.stbridgeB?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingLineAxis?.Length ?? 0, this.DrawingLineAxis.records));
            }

            if (this.DrawingArcAxis.records != null)
            {
                Summary.Rows.Add(Summary.CollectResult(this.DrawingArcAxis.HeaderName, this.stbridgeA?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis?.Length ?? 0,
                    this.stbridgeB?.StbModel?.StbAxes?.StbDrawingAxes?.StbDrawingArcAxis?.Length ?? 0, this.DrawingArcAxis.records));
            }

            if (this.StoriesRecord.records != null)
            {
                Summary.Rows.Add(Summary.CollectResult(this.StoriesRecord.HeaderName, this.stbridgeA?.StbModel?.StbStories?.Length ?? 0,
                    this.stbridgeB?.StbModel?.StbStories?.Length ?? 0, this.StoriesRecord.records));
            }

            if (this.ColumnsRecord.records != null)
            {
                Summary.Rows.Add(Summary.CollectResult(this.ColumnsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbColumns?.Length ?? 0,
                    this.stbridgeB?.StbModel?.StbMembers?.StbColumns?.Length ?? 0, this.ColumnsRecord.records));
            }

            if (this.PostsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.PostsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbPosts?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbPosts?.Length ?? 0, this.PostsRecord.records));
            }

            if (this.GirdersRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.GirdersRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbGirders?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbGirders?.Length ?? 0, this.GirdersRecord.records));
            }

            if (this.BeamsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.BeamsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbBeams?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbBeams?.Length ?? 0, this.BeamsRecord.records));
            }

            if (this.BracesRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.BracesRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbBraces?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbBraces?.Length ?? 0, this.BracesRecord.records));
            }

            if (this.SlabsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SlabsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbSlabs?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbSlabs?.Length ?? 0, this.SlabsRecord.records));
            }

            if (this.WallsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.WallsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbWalls?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbWalls?.Length ?? 0, this.WallsRecord.records));
            }

            if (this.FootingsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.FootingsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbFootings?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbFootings?.Length ?? 0, this.FootingsRecord.records));
            }

            if (this.StripFootingsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.StripFootingsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbStripFootings?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbStripFootings?.Length ?? 0, this.StripFootingsRecord.records));
            }

            if (this.PilesRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.PilesRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbPiles?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbPiles?.Length ?? 0, this.PilesRecord.records));
            }

            if (this.FoundationColumnsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.FoundationColumnsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbFoundationColumns?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbFoundationColumns?.Length ?? 0, this.FoundationColumnsRecord.records));
            }

            if (this.ParapetsRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.ParapetsRecord.HeaderName, this.stbridgeA?.StbModel?.StbMembers?.StbParapets?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbMembers?.StbParapets?.Length ?? 0, this.ParapetsRecord.records));
            }

            if (this.SecColumnRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecColumnRcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecColumn_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecColumn_RC?.Length ?? 0, this.SecColumnRcRecord.records));
            }

            if (this.SecColumnSRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecColumnSRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecColumn_S?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecColumn_S?.Length ?? 0, this.SecColumnSRecord.records));
            }

            if (this.SecColumnSrcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecColumnSrcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecColumn_SRC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecColumn_SRC?.Length ?? 0, this.SecColumnSrcRecord.records));
            }

            if (this.SecColumnCftRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecColumnCftRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecColumn_CFT?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecColumn_CFT?.Length ?? 0, this.SecColumnCftRecord.records));
            }

            if (this.SecBeamRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecBeamSRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecBeam_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecBeam_RC?.Length ?? 0, this.SecBeamRcRecord.records));
            }

            if (this.SecBeamSRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecBeamSRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecBeam_S?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecBeam_S?.Length ?? 0, this.SecBeamSRecord.records));
            }

            if (this.SecBeamSrcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecBeamSrcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecBeam_SRC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecBeam_SRC?.Length ?? 0, this.SecBeamSrcRecord.records));
            }

            if (this.SecBraceSRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecBraceSRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecBrace_S?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecBrace_S?.Length ?? 0, this.SecBraceSRecord.records));
            }

            if (this.SecSlabRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecSlabRcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecSlab_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecSlab_RC?.Length ?? 0, this.SecSlabRcRecord.records));
            }

            if (this.SecSlabDeckRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecSlabDeckRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecSlabDeck?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecSlabDeck?.Length ?? 0, this.SecSlabDeckRecord.records));
            }

            if (this.SecSlabPrecastRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecSlabPrecastRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecSlabPrecast?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecSlabPrecast?.Length ?? 0, this.SecSlabPrecastRecord.records));
            }

            if (this.SecWallRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecWallRcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecWall_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecWall_RC?.Length ?? 0, this.SecWallRcRecord.records));
            }

            if (this.SecFoundationRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecFoundationRcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecFoundation_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecFoundation_RC?.Length ?? 0, this.SecFoundationRcRecord.records));
            }

            if (this.SecPileRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecPileRcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecPile_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecPile_RC?.Length ?? 0, this.SecPileRcRecord.records));
            }

            if (this.SecPileSRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecPileSRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0, this.SecPileSRecord.records));
            }

            if (this.SecPileProductRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecPileProductRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecPile_S?.Length ?? 0, this.SecPileProductRecord.records));
            }

            if (this.SecParapetRcRecord.records != null)
            {
                Summary.Rows.Add(
                    Summary.CollectResult(this.SecParapetRcRecord.HeaderName, this.stbridgeA?.StbModel?.StbSections?.StbSecParapet_RC?.Length ?? 0,
                        this.stbridgeB?.StbModel?.StbSections?.StbSecParapet_RC?.Length ?? 0, this.SecParapetRcRecord.records));
            }

            return Summary;
        }




    }
}

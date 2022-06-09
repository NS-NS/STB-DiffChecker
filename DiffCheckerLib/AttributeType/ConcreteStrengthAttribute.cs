using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.AttributeType
{
    internal class ConcreteStrengthAttribute : AbstractAttribute
    {
        private static SortedList<double, StbStory> sortedStories;

        public ConcreteStrengthAttribute(string stbName) : base(stbName)
        {
        }

        private void Compare(string a, string b, IReadOnlyList<string> key, List<Record> records)
        {
            if (a == null && b == null)
                return;
            else if (a == b)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(),
                    Consistency.Consistent, this.Importance));
            else if (a == null || b == null)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(),
                    Consistency.Incomparable, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(),
                    Consistency.Inconsistent, this.Importance));
        }

        internal void CompareColumn(StbColumn a, ST_BRIDGE stBridgeA, StbColumn b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(ColumnConcrete(a, stBridgeA), ColumnConcrete(b, stBridgeB), key, records);
        }

        internal void ComparePost(StbPost a, ST_BRIDGE stBridgeA, StbPost b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(PostConcrete(a, stBridgeA), PostConcrete(b, stBridgeB), key, records);
        }

        internal void CompareGirder(StbGirder a, ST_BRIDGE stBridgeA, StbGirder b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(GirderConcrete(a, stBridgeA), GirderConcrete(b, stBridgeB), key, records);
        }

        internal void CompareBeam(StbBeam a, ST_BRIDGE stBridgeA, StbBeam b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(BeamConcrete(a, stBridgeA), BeamConcrete(b, stBridgeB), key, records);
        }

        internal void CompareBrace(StbBrace a, ST_BRIDGE stBridgeA, StbBrace b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        { }

        internal void CompareSlab(StbSlab a, ST_BRIDGE stBridgeA, StbSlab b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(SlabConcrete(a, stBridgeA), SlabConcrete(b, stBridgeB), key, records);
        }

        internal void CompareWall(StbWall a, ST_BRIDGE stBridgeA, StbWall b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(WallConcrete(a, stBridgeA), WallConcrete(b, stBridgeB), key, records);
        }

        internal void CompareFooting(StbFooting a, ST_BRIDGE stBridgeA, StbFooting b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(FootingConcrete(a, stBridgeA), FootingConcrete(b, stBridgeB), key, records);
        }

        internal void CompareStripFooting(StbStripFooting a, ST_BRIDGE stBridgeA, StbStripFooting b,
            ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            Compare(StripFootingConcrete(a, stBridgeA), StripFootingConcrete(b, stBridgeB), key, records);

        }

        internal void ComparePile(StbPile a, ST_BRIDGE stBridgeA, StbPile b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            // Productの場合、一つのインスタンスに複数のコンクリート強度
        }

        internal void CompareFoundationColumn(StbFoundationColumn a, ST_BRIDGE stBridgeA, StbFoundationColumn b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records) { }

        internal void CompareParapet(StbParapet a, ST_BRIDGE stBridgeA, StbParapet b, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records) { }




        private string ColumnConcrete(StbColumn column, ST_BRIDGE stBridge)
        {
            if (column.kind_structure == StbColumnKind_structure.S)
                return null;
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            string storyConcrete = StoryConcrete(new List<string>() { column.id_node_bottom, column.id_node_top}, stBridge);
            string sectionConcrete = null;
            if (column.kind_structure == StbColumnKind_structure.CFT)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecColumn_CFT
                    ?.FirstOrDefault(n => n.id == column.id_section)?.strength_concrete;
            }
            else if (column.kind_structure == StbColumnKind_structure.RC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecColumn_RC
                    ?.FirstOrDefault(n => n.id == column.id_section)?.strength_concrete;
            }
            else if (column.kind_structure == StbColumnKind_structure.SRC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecColumn_SRC
                    ?.FirstOrDefault(n => n.id == column.id_section)?.strength_concrete;
            }

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string PostConcrete(StbPost p, ST_BRIDGE stBridge)
        {
            if (p.kind_structure == StbColumnKind_structure.S)
                return null;
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            string storyConcrete = StoryConcrete(new List<string>() { p.id_node_bottom, p.id_node_top }, stBridge);
            string sectionConcrete = null;
            if (p.kind_structure == StbColumnKind_structure.CFT)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecColumn_CFT
                    ?.FirstOrDefault(n => n.id == p.id_section)?.strength_concrete;
            }
            else if (p.kind_structure == StbColumnKind_structure.RC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecColumn_RC
                    ?.FirstOrDefault(n => n.id == p.id_section)?.strength_concrete;
            }
            else if (p.kind_structure == StbColumnKind_structure.SRC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecColumn_SRC
                    ?.FirstOrDefault(n => n.id == p.id_section)?.strength_concrete;
            }

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string GirderConcrete(StbGirder g, ST_BRIDGE stBridge)
        {
            if (g.kind_structure == StbGirderKind_structure.S)
                return null;
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            string storyConcrete = StoryConcrete(new List<string>() { g.id_node_start, g.id_node_end }, stBridge);
            string sectionConcrete = null;
            if (g.kind_structure == StbGirderKind_structure.RC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecBeam_RC
                    ?.FirstOrDefault(n => n.id == g.id_section)?.strength_concrete;
            }
            else if (g.kind_structure == StbGirderKind_structure.SRC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecBeam_SRC
                    ?.FirstOrDefault(n => n.id == g.id_section)?.strength_concrete;
            }

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string BeamConcrete(StbBeam b, ST_BRIDGE stBridge)
        {
            if (b.kind_structure == StbGirderKind_structure.S)
                return null;
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            string storyConcrete = StoryConcrete(new List<string>() { b.id_node_start, b.id_node_end }, stBridge);
            string sectionConcrete = null;
            if (b.kind_structure == StbGirderKind_structure.RC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecBeam_RC
                    ?.FirstOrDefault(n => n.id == b.id_section)?.strength_concrete;
            }
            else if (b.kind_structure == StbGirderKind_structure.SRC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecBeam_SRC
                    ?.FirstOrDefault(n => n.id == b.id_section)?.strength_concrete;
            }

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string SlabConcrete(StbSlab s, ST_BRIDGE stBridge)
        {
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            var nodes = s.StbNodeIdOrder.Split(' ');
            string storyConcrete = StoryConcrete(new List<string>(nodes), stBridge, true);
            string sectionConcrete = null;
            if (s.kind_structure == StbSlabKind_structure.RC)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecSlab_RC
                    ?.FirstOrDefault(n => n.id == s.id_section)?.strength_concrete;
            }
            else if (s.kind_structure == StbSlabKind_structure.DECK)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecSlabDeck
                    ?.FirstOrDefault(n => n.id == s.id_section)?.strength_concrete;
            }
            else if (s.kind_structure == StbSlabKind_structure.PRECAST)
            {
                sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecSlabPrecast
                    ?.FirstOrDefault(n => n.id == s.id_section)?.strength_concrete;
            }

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string WallConcrete(StbWall w, ST_BRIDGE stBridge)
        {
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            var nodes = w.StbNodeIdOrder.Split(' ');
            string storyConcrete = StoryConcrete(new List<string>(nodes), stBridge, true);
            string sectionConcrete = null;

            sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecWall_RC
                    ?.FirstOrDefault(n => n.id == w.id_section)?.strength_concrete;
            
            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string FootingConcrete(StbFooting f, ST_BRIDGE stBridge)
        {
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            string storyConcrete = StoryConcrete(new List<string>(){f.id_node}, stBridge, true);
            string sectionConcrete = null;

            sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecFoundation_RC
                ?.FirstOrDefault(n => n.id == f.id_section)?.strength_concrete;

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }

        private string StripFootingConcrete(StbStripFooting f, ST_BRIDGE stBridge)
        {
            string defaultConcrete = stBridge?.StbCommon?.strength_concrete;
            string storyConcrete = StoryConcrete(new List<string>() { f.id_node_start, f.id_node_end }, stBridge, true);
            string sectionConcrete = null;

            sectionConcrete = stBridge?.StbModel?.StbSections?.StbSecFoundation_RC
                ?.FirstOrDefault(n => n.id == f.id_section)?.strength_concrete;

            if (sectionConcrete != null)
                return sectionConcrete;
            if (storyConcrete != null)
                return storyConcrete;
            return defaultConcrete;
        }


        private string StoryConcrete(IReadOnlyList<string> idList, ST_BRIDGE stBridge, bool isSlab = false)
        {
            var stbNodes = stBridge?.StbModel?.StbNodes;
            var stbStories = stBridge?.StbModel?.StbStories;
            if (stbNodes == null || stbStories == null)
                return null;

            // Height順にSort
            if (sortedStories == null)
            {
                sortedStories = new SortedList<double, StbStory>();
                foreach (var stbStory in stbStories)
                {
                    sortedStories.Add(stbStory.height, stbStory);
                }
            }

            int maxFloorIndex = Int32.MinValue;
            int minFloorIndex = Int32.MaxValue;
            foreach (var id in idList)
            {
                var stbNode = stbNodes.FirstOrDefault(n => n.id == id);
                if (stbNode == null)
                    return null;
                StbStory stbStory = null;
                foreach (var story in stbStories)
                {
                    if (story.StbNodeIdList != null)
                    {
                        foreach (var node in story.StbNodeIdList)
                        {
                            if (node.id == id)
                            {
                                stbStory = story;
                                break;
                            }
                        }
                    }

                    if (stbStory != null)
                        break;
                }

                if (stbStory == null)
                    return null;

                int index = sortedStories.IndexOfValue(stbStory);
                if (maxFloorIndex < index)
                    maxFloorIndex = index;
                if (minFloorIndex > index)
                    minFloorIndex = index;
            }

            if (maxFloorIndex - minFloorIndex > 1)
                return null;
            else if (isSlab && maxFloorIndex - minFloorIndex == 1)
            {
                // スラブの場合は同じ階を前提とする。
                return null;
            }
            else
                return sortedStories.Values[minFloorIndex].strength_concrete;

        }

    }
}

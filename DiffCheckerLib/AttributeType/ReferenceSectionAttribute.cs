using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.AttributeType
{
    class ReferenceSectionAttribute : AbstractAttribute
    {
        public ReferenceSectionAttribute(string stbName) : base(stbName)
        {
        }

        internal void CompareColumnSection(string keyA, string kindA, ST_BRIDGE stbridgeA, string keyB, string kindB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindColumnName(keyA, kindA, stbridgeA);
            var b = FindColumnName(keyB, kindB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void CompareBeamSection(string keyA, string kindA, ST_BRIDGE stBridgeA, string keyB, string kindB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindBeamName(keyA, kindA, stBridgeA);
            var b = FindBeamName(keyB, kindB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void CompareBraceSection(string keyA, string kindA, ST_BRIDGE stBridgeA, string keyB, string kindB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindBraceName(keyA, kindA, stBridgeA);
            var b = FindBraceName(keyB, kindB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void CompareSlabSection(string keyA, string kindA, ST_BRIDGE stBridgeA, string keyB, string kindB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindSlabName(keyA, kindA, stBridgeA);
            var b = FindSlabName(keyB, kindB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void CompareWallSection(string keyA, string kindA, ST_BRIDGE stBridgeA, string keyB, string kindB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindWallName(keyA, kindA, stBridgeA);
            var b = FindWallName(keyB, kindB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void CompareFoundationSection(string keyA, ST_BRIDGE stBridgeA, string keyB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindFoundationName(keyA, stBridgeA);
            var b = FindFoundationName(keyB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void ComparePileSection(string keyA, string kindA, ST_BRIDGE stBridgeA, string keyB, string kindB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindPileName(keyA, kindA, stBridgeA);
            var b = FindPileName(keyB, kindB, stBridgeB);
            Compare(a, b, key, records);
        }


        public void CompareParapetSection(string keyA, ST_BRIDGE stBridgeA, string keyB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            var a = FindParapetName(keyA, stBridgeA);
            var b = FindParapetName(keyB, stBridgeB);
            Compare(a, b, key, records);
        }

        public void CompareFoundationColumnSection(string keyA, ST_BRIDGE stBridgeA, string keyB, ST_BRIDGE stBridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            CompareColumnSection(keyA, "RC", stBridgeA, keyB, "RC", stBridgeB, key, records);
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

        private static string FindColumnName(string key, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecColumn_RC.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else if (kind == "S")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecColumn_S.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else if (kind == "SRC")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecColumn_SRC.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else if (kind == "CFT")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecColumn_CFT.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else
                throw new Exception();
        }

        private static string FindBeamName(string key, string kind, ST_BRIDGE stbridge)
        {
            if (kind == "RC")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecBeam_RC.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else if (kind == "S")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecBeam_S.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else if (kind == "SRC")
            {
                var section = stbridge?.StbModel?.StbSections?.StbSecBeam_SRC.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else
                throw new Exception();
        }

        private string FindBraceName(string key, string kind, ST_BRIDGE stBridge)
        {
            if (kind == "S")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecBrace_S.FirstOrDefault(n => n.id == key);
                return section.floor + "/" + section.name;
            }
            else
                throw new Exception();
        }

        private string FindSlabName(string key, string kind, ST_BRIDGE stBridge)
        {
            if (kind == "RC")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecSlab_RC.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else if (kind == "DECK")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecSlabDeck.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else if (kind == "PRECAST")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecSlabPrecast.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else
                throw new Exception();
        }

        private string FindWallName(string key, string kind, ST_BRIDGE stBridge)
        {
            if (kind == "RC")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecWall_RC.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else
                throw new Exception();
        }

        private string FindFoundationName(string key, ST_BRIDGE stBridge)
        {
            var section = stBridge?.StbModel?.StbSections?.StbSecFoundation_RC.FirstOrDefault(n => n.id == key);
            return section.name;
        }

        private string FindPileName(string key, string kind, ST_BRIDGE stBridge)
        {
            if (kind == "RC")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecPile_RC.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else if (kind == "S")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecPile_S.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else if (kind == "PC")
            {
                var section = stBridge?.StbModel?.StbSections?.StbSecPileProduct.FirstOrDefault(n => n.id == key);
                return section.name;
            }
            else
                throw new Exception();
        }

        private string FindParapetName(string key, ST_BRIDGE stBridge)
        {
            var section = stBridge?.StbModel?.StbSections?.StbSecParapet_RC.FirstOrDefault(n => n.id == key);
            return section.name;
        }
    }
}

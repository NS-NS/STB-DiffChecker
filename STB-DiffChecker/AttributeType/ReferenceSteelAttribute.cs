using STBridge201;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.AttributeType
{
    public class ReferenceSteelAttribute : AbstractAttribute
    {
        public ReferenceSteelAttribute(string stbName) : base(stbName) { }

        internal void Compare(string keyA, ST_BRIDGE stbridgeA, string keyB, ST_BRIDGE stbridgeB, IReadOnlyList<string> key, List<Record> records)
        {
            if (keyA == null && keyB == null)
                return;
            else if (keyA == null && keyB != null)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), "無", "有", Consistency.Incomparable, Importance));
                return;
            }
            else if (keyA != null && keyB == null)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), "有", "無", Consistency.Incomparable, Importance));
                return;
            }

            string shapeA = SteelShape(keyA, stbridgeA);
            string shapeB = SteelShape(keyB, stbridgeB);

            Compare(shapeA, shapeB, key, records);
        }

        internal string SteelShape(string key, ST_BRIDGE stBridge)
        {
            var rollH = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecRollH?.FirstOrDefault(n => n.name == key);
            if (rollH != null)
            {
                return rollH.type + "-" + rollH.A + "x" + rollH.B + "x" + rollH.t1 + "x" + rollH.t2 + "x" + rollH.r;
            }

            var buildH = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecBuildH?.FirstOrDefault(n => n.name == key);
            if (buildH != null)
            {
                return "BH-" + buildH.A + "x" + buildH.B + "x" + buildH.t1 + "x" + buildH.t2;
            }

            var rollBox = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecRollBOX?.FirstOrDefault(n => n.name == key);
            if (rollBox != null)
            {
                return "□-" + rollBox.A + "x" + rollBox.B + "x" + rollBox.t + "x" + rollBox.r + "(" + rollBox.type + ")";
            }

            var buildBox = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecBuildBOX?.FirstOrDefault(n => n.name == key);
            if (buildBox != null)
            {
                return "BX-" + buildBox.A + "x" + buildBox.B + "x" + buildBox.t1 + "x" + buildBox.t2;
            }

            var pipe = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecPipe?.FirstOrDefault(n => n.name == key);
            if (pipe != null)
            {
                return "P-" + pipe.D + "x" + pipe.t;
            }

            var rollT = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecRollT?.FirstOrDefault(n => n.name == key);
            if (rollT != null)
            {
                return rollT.type + "-" + rollT.A + "x" + rollT.B + "x" + rollT.t1 + "x" + rollT.t2 + "x" + rollT.r;
            }

            var rollC = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecRollC?.FirstOrDefault(n => n.name == key);
            if (rollC != null)
            {
                if (rollC.type == StbSecRollCType.SINGLE)
                    return "[-" + rollC.A + "x" + rollC.B + "x" + rollC.t1 + "x" + rollC.t2 + "x" + rollC.r1 + "x" +
                           rollC.r2;
                else
                    return "2[-" + rollC.A + "x" + rollC.B + "x" + rollC.t1 + "x" + rollC.t2 + "x" + rollC.r1 + "x" +
                           rollC.r2 + "(" + rollC.type + ")";
            }

            var rollL = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecRollL?.FirstOrDefault(n => n.name == key);
            if (rollL != null)
            {
                if (rollL.type == StbSecRollLType.SINGLE)
                    return "L-" + rollL.A + "x" + rollL.B + "x" + rollL.t1 + "x" + rollL.t2 + "x" + rollL.r1 + "x" +
                           rollL.r2;
                else
                    return "2L-" + rollL.A + "x" + rollL.B + "x" + rollL.t1 + "x" + rollL.t2 + "x" + rollL.r1 + "x" +
                           rollL.r2 + "(" + rollL.type + ")";
            }

            var lipC = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecLipC?.FirstOrDefault(n => n.name == key);
            if (lipC != null)
            {
                if (lipC.type == StbSecLipCType.SINGLE)
                    return "C-" + lipC.H + "x" + lipC.A + "x" + lipC.C + "x" + lipC.t;
                else
                    return "2C-" + lipC.H + "x" + lipC.A + "x" + lipC.C + "x" + lipC.t  + "(" + lipC.type + ")";
            }

            var flatBar = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecFlatBar?.FirstOrDefault(n => n.name == key);
            if (flatBar != null)
            {
                return "FB-" + flatBar.B + "x" + flatBar.t;
            }

            var roundBar = stBridge?.StbModel?.StbSections?.StbSecSteel?.StbSecRoundBar?.FirstOrDefault(n => n.name == key);
            if (roundBar != null)
            {
                return "●-" + roundBar.R;
            }

            return null;
        }



        private void Compare(string a, string b, IReadOnlyList<string> key, List<Record> records)
        {
            if (a == null && b == null)
                return;
            else if (a == b)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b,
                    Consistency.Consistent, this.Importance));
            else if (a == null || b == null)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b,
                    Consistency.Incomparable, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b,
                    Consistency.Inconsistent, this.Importance));
        }
    }
}

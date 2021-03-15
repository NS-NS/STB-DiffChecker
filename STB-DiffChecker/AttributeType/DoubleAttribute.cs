using System;
using System.Collections.Generic;
using STBDiffChecker;

namespace STBDiffChecker.AttributeType
{
    public class DoubleAttribute : AbstractAttribute
    {
        public DoubleAttribute(string stbName) : base(stbName)
        {
        }

        internal void Compare(double a, double b, IReadOnlyList<string> key, List<Record> records, double margin = Utility.Tolerance)
        {
            if (Math.Abs(a - b) < Utility.Tolerance)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(),
                    Consistency.Consistent, this.Importance));
            else if (Math.Abs(a - b) < margin)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(),
                    Consistency.AlmostMatch, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(),
                    Consistency.Inconsistent, this.Importance));
        }

        internal void Compare(bool specifiedA, double a, bool specifiedB, double b, IReadOnlyList<string> key,
            List<Record> records, double margin = Utility.Tolerance)
        {
            if (specifiedA == false && specifiedB == false)
                return;
            else if (specifiedA == true && specifiedB == false)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), null, Consistency.Incomparable, this.Importance));
            }
            else if (specifiedA == false && specifiedB == true)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), null, b.ToString(), Consistency.Incomparable, this.Importance));
            }
            else
            {
                Compare(a, b, key, records, margin);
            }
        }
    }
}

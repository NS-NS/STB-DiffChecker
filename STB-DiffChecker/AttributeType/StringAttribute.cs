using System.Collections.Generic;

namespace STBDiffChecker.AttributeType
{
    public class StringAttribute : AbstractAttribute
    {
        public StringAttribute(string stbName) : base(stbName)
        {

        }

        internal void Compare(string a, string b, IReadOnlyList<string> key, List<Record> records)
        {
            if (a == null && b == null)
                return;
            else if (a == b)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b,
                    Consistency.Consistent, this.Importance));
            else if (a == null || b == null)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a ?? "null", b ?? "null", Consistency.Incomparable, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b,
                    Consistency.Inconsistent, this.Importance));
        }

        internal void Compare(bool specifiedA, string a, bool specifiedB, string b, IReadOnlyList<string> key,
            List<Record> records)
        {
            if (specifiedA == false && specifiedB == false)
                return;
            else if (specifiedA == true && specifiedB == false)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, null, Consistency.Incomparable, this.Importance));
            }
            else if (specifiedA == false && specifiedB == true)
            {
                records.Add(new Record(this.ParentElement(), key, this.Item(), null, b, Consistency.Incomparable, this.Importance));
            }
            else
            {
                Compare(a, b, key, records);
            }
        }
    }
}

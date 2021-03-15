using System.Collections.Generic;

namespace STBDiffChecker.AttributeType
{
    public class IntegerAttribute : AbstractAttribute
    {
        public IntegerAttribute(string stbName) : base(stbName)
        {
        }

        internal void Compare(string a, string b, IReadOnlyList<string> key, List<Record> records)
        {
            if (a == null && b == null)
                return;
            else if (a == b)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b, Consistency.Consistent, this.Importance));
            else if (a == null || b == null)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a ?? "null", b ?? "null",
                    Consistency.Incomparable, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a, b, Consistency.Inconsistent, this.Importance));
        }



    }
}

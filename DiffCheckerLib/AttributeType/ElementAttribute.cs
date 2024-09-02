using System.Collections.Generic;

namespace STBDiffChecker.AttributeType
{
    public class ElementAttribute : AbstractAttribute
    {
        public ElementAttribute(string stbName): base(stbName) { }


        internal void Compare(object a, object b, IReadOnlyList<string> key, List<Record> records)
        {
            if (a == null && b == null)
                return;
            else if ((a == null && b != null))
                records.Add(new Record(this.ParentElement(), key, this.Item(), "無", "有",
                    Consistency.ElementIncomparable, this.Importance));
            else if ((a != null && b == null))
                records.Add(new Record(this.ParentElement(), key, this.Item(), "有", "無",
                    Consistency.ElementIncomparable, this.Importance));
            else
            {
                Consistency result = a == b ? Consistency.Consistent : Consistency.Inconsistent;
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(), result, this.Importance));

            }
        }


        internal void AppendConcistentRecord(object a, IReadOnlyList<string> key, List<Record> records, bool isAlomost = false)
        {
            if (isAlomost)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), a.ToString(), Consistency.AlmostMatch, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), a.ToString(), Consistency.Consistent, this.Importance));

        }

    }
}

using System.Collections.Generic;

namespace STBDiffChecker.AttributeType
{
    public class BooleanAttribute : AbstractAttribute
    {
        public BooleanAttribute(string stbName) : base(stbName)
        {
        }


        internal void Compare(bool a, bool b, IReadOnlyList<string> key, List<Record> records)
        {
            if (a == b)
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(), Consistency.Consistent, this.Importance));
            else
                records.Add(new Record(this.ParentElement(), key, this.Item(), a.ToString(), b.ToString(), Consistency.Inconsistent, this.Importance));
        }
    }
}

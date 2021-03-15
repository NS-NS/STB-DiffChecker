using STBridge201;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.AttributeType
{
    class ReferenceStoryAttribute : AbstractAttribute
    {
        public ReferenceStoryAttribute(string stbName) : base(stbName)
        {
        }

        internal void Compare(string keyA, ST_BRIDGE stBridgeA, string keyB, ST_BRIDGE stBridgeB,
            IReadOnlyList<string> key, List<Record> records)
        {
            var storyA = stBridgeA.StbModel.StbStories.FirstOrDefault(n => n.id == keyA);
            var storyB = stBridgeB.StbModel.StbStories.FirstOrDefault(n => n.id == keyB);
            if (storyA?.name != null && storyB?.name != null)
            {
                if (storyB?.name == null)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), storyA.name, null, Consistency.Incomparable, this.Importance));
                }
                else if (storyA?.name == null)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), null, storyB.name, Consistency.Incomparable, this.Importance));
                }
                else if (storyA.name != storyB.name)
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), storyA.name, storyB.name, Consistency.Inconsistent, this.Importance));
                }
                else
                {
                    records.Add(new Record(this.ParentElement(), key, this.Item(), storyA.name, storyB.name, Consistency.Consistent, this.Importance));

                }
            }
        }
    }
}

using STBridge201;
using System.Collections.Generic;
using System.Linq;
using StbStory = STBridge201.StbStory;

namespace STBDiffChecker.v201.Records
{
    internal static class Stories
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var storyA = stBridgeA?.StbModel?.StbStories;
            var storyB = stBridgeB?.StbModel?.StbStories;
            var setB = storyB != null ? new HashSet<StbStory>(storyB) : new HashSet<StbStory>();

            if (storyA != null)
            {
                foreach (var a in storyA)
                {
                    var key = new List<string> { $"name={a.name}"};
                    bool hasItem = false;
                    if (storyB != null)
                    {
                        foreach (var b in storyB.Where(n => n.name == a.name))
                        {
                            CheckObjects.StbStoryId.Compare(a.id, b.id, key, records);
                            CheckObjects.StbStoryGuid.Compare(a.guid, b.guid, key, records);
                            CheckObjects.StbStoryName.Compare(a.name, b.name, key, records);
                            CheckObjects.StbStoryHeight.Compare(a.height, b.height, key, records);
                            CheckObjects.StbStoryKind.Compare(a.kind.ToString(), b.kind.ToString(), key, records);
                            CheckObjects.StbStoryIdDependence.Compare(a.id_dependence, stBridgeA, b.id_dependence, stBridgeB, key, records);
                            CheckObjects.StbStoryStrengthConcrete.Compare(a.strength_concrete, b.strength_concrete, key, records);
                            setB.Remove(b);
                            hasItem = true;
                        }
                    }

                    if (!hasItem)
                    {
                        CheckObjects.StbStory.Compare(nameof(StbStory), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var key = new List<string> { $"name={b.name}"};
                CheckObjects.StbStory.Compare(null, nameof(StbStory), key, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }
    }
}

using System.Runtime.CompilerServices;

namespace ST_BRIDGE202
{
    /// <summary>
    /// 節点をidから高速に検索するための拡張メソッド
    /// StbNodes全走査(O(n))を辞書lookup(O(1))に置き換える。辞書はST_BRIDGEインスタンスごとにキャッシュされる
    /// </summary>
    public static class StbNodeFinder
    {
        private static readonly ConditionalWeakTable<ST_BRIDGE, Dictionary<string, StbNode>> cache = new();

        public static StbNode? FindNode(this ST_BRIDGE? stb, string? id)
        {
            if (stb?.StbModel?.StbNodes == null || id == null)
            {
                return null;
            }

            Dictionary<string, StbNode> dict = cache.GetValue(stb, s =>
            {
                Dictionary<string, StbNode> d = new(s.StbModel.StbNodes.Length);
                foreach (StbNode node in s.StbModel.StbNodes)
                {
                    // id重複時は最初の節点を採用(FirstOrDefaultと同じ挙動)
                    _ = d.TryAdd(node.id, node);
                }
                return d;
            });

            return dict.TryGetValue(id, out StbNode? node) ? node : null;
        }
    }
}
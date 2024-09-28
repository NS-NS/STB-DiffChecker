using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using System.Reflection;
using System.Text;
using System.Windows;

namespace STB_DiffChecker_202
{
    /// <summary>
    /// 重要度の読み込み用クラス
    /// </summary>
    public class ImportanceSetting : IImportanceSetting
    {
        /// <summary>
        /// リソースに保存したスキーマファイルの文字列を取得
        /// </summary>
        public string GetSchemaContent()
        {
            return XmlValidate.GetEmbeddedXsd(Assembly.GetExecutingAssembly(), @"STB_DiffChecker_202.STBridge.STBridge_v202.xsd", Encoding.UTF8);
        }

        public IReadOnlyList<(string, string)> GetTabs()
        {
            return [
                ("StbCommon", "共通"),
                ("StbNodes", "節点"),
                ("StbParallelAxes", "平行軸"),
                ("StbArcAxes", "円弧軸"),
                ("StbRadialAxes", "放射軸"),
                ("StbDrawingLineAxis", "描画軸(直線)"),
                ("StbDrawingArcAxis", "描画軸(円弧)"),
                ("StbStories", "階"),
                ("StbColumns", "柱"),
                ("StbPosts", "間柱"),
                ("StbGirders", "大梁"),
                ("StbBeams", "小梁"),
                ("StbBraces", "ブレース"),
                ("StbSlabs", "スラブ"),
                ("StbWalls", "壁"),
                ("StbFootings", "基礎"),
                ("StbStripFootings", "布基礎"),
                ("StbPiles", "杭"),
                ("StbFoundationColumns", "基礎柱"),
                ("StbParapets", "パラペット"),
                ("StbSecColumn_RC", "柱断面RC"),
                ("StbSecColumn_S", "柱断面S"),
                ("StbSecColumn_SRC", "柱断面SRC"),
                ("StbSecColumn_CFT", "柱断面CFT"),
                ("StbSecBeam_RC", "梁断面RC"),
                ("StbSecBeam_S", "梁断面S"),
                ("StbSecBeam_SRC", "梁断面SRC"),
                ("StbSecBrace_S", "ブレース断面S"),
                ("StbSecSlab_RC", "スラブ断面RC"),
                ("StbSecSlabDeck", "スラブ断面デッキ"),
                ("StbSecSlabPrecast", "スラブ断面プレキャスト"),
                ("StbSecWall_RC", "壁断面RC"),
                ("StbSecFoundation_RC", "基礎断面RC"),
                ("StbSecPile_RC", "杭断面RC"),
                ("StbSecPile_S", "杭断面S"),
                ("StbSecPileProduct", "杭断面製品"),
                ("StbSecParapet_RC", "パラペット断面RC"),
                ("StbJoints", "継手")
             ];
        }

        private static IReadOnlyList<string>? orderedImportance;

        private static Dictionary<string, Importance>? userImportance;

        public IReadOnlyList<string> OrderedImportance()
        {
            if (orderedImportance != null)
            {
                return orderedImportance;
            }

            IReadOnlyList<string> ReferenceElements = ["StbOpen", "StbSecOpen_RC", "StbSecSteel"];
            List<string> elements = XmlTree.GetElementPaths(GetSchemaContent(), ["ST_BRIDGE", .. ReferenceElements], out List<string> errors);
            if (errors.Count > 0)
            {
                _ = MessageBox.Show(
                    "XMLスキーマの読み込みに失敗しました。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return [];
            }

            List<string> ordered = [];
            List<string> selectedElements = new(GetTabs().Select(n => n.Item1));
            foreach (string element in elements)
            {
                if (ordered.Contains(element))
                {
                    continue;
                }

                if (selectedElements.Any(element.Contains))
                {
                    ordered.Add(element);
                }

                if (!element.Contains("ST_BRIDGE") && ReferenceElements.Any(element.Contains))
                {
                    ordered.Add(element);
                }
            }

            orderedImportance = ordered;
            return orderedImportance;
        }

        public Dictionary<string, Importance> UserImportance()
        {
            if (userImportance != null)
            {
                return userImportance;
            }

            userImportance = [];
            foreach (string element in OrderedImportance())
            {
                userImportance.Add(element, Importance.Required);
            }

            return userImportance;
        }
    }
}
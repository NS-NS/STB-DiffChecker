using DiffCheckerLib.Setting;
using System.Collections.Generic;
using System.Linq;

namespace DiffCheckerLib.Interface
{
    /// <summary>
    /// 全てのレコードを管理するクラス用のインターフェース
    /// </summary>
    public interface ITotalRecords
    {
        /// <summary>
        /// 概要タブ
        /// </summary>
        Summary Summary { get; set; }

        /// <summary>
        /// 概要以外の結果タブ
        /// </summary>
        List<RecordTab> recordTabs { get; set; }

        ResultFormSetting resultFormSetting { get; set; }
        IST_BRIDGE istbridgeA { get; set; }
        IST_BRIDGE istbridgeB { get; set; }

        /// <summary>
        /// 概要作成関数
        /// </summary>
        Summary CreateSummary();

        /// <summary>
        /// 実行関数
        /// </summary>
        void Run()
        {
            List<string> targetNames = resultFormSetting.importanceSetting.GetTabs().Select(n => n.Item1).ToList();
            Dictionary<string, List<Record>> records = ObjectComparer.CompareSTBridgeFiles(istbridgeA, istbridgeB, targetNames, resultFormSetting.importanceSetting.UserImportance(), resultFormSetting.toleranceSetting);

            foreach ((string, string) target in resultFormSetting.importanceSetting.GetTabs())
            {
                if (records.ContainsKey(target.Item1))
                {
                    recordTabs.First(n => n.Header == target.Item2).SetRecord(records[target.Item1]);
                }
            }

            Summary = CreateSummary();
        }

    }
}

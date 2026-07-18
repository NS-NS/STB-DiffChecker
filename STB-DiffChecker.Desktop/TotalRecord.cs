using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;

namespace STB_DiffChecker
{
    /// <summary>
    /// 比較の実行と概要集計(全バージョン共通)
    /// 要素数はDiffCheckerCoreのElementCounterでバージョン非依存に取得する
    /// </summary>
    public class TotalRecord : ITotalRecords
    {
        public List<RecordTab> recordTabs { get; set; } = [];
        public ResultFormSetting resultFormSetting { get; set; }
        public Summary Summary { get; set; } = new Summary();
        public IST_BRIDGE istbridgeA { get; set; }
        public IST_BRIDGE istbridgeB { get; set; }

        public TotalRecord(ResultFormSetting resultFormSetting, IST_BRIDGE stbA, IST_BRIDGE stbB)
        {
            Summary.dateTime = DateTime.Now;
            this.resultFormSetting = resultFormSetting;
            istbridgeA = stbA;
            istbridgeB = stbB;

            recordTabs = [];
            foreach ((string, string) tab in resultFormSetting.importanceSetting.GetTabs())
            {
                recordTabs.Add(new RecordTab(tab.Item2));
            }
        }

        public Summary CreateSummary()
        {
            IReadOnlyList<(string, string)> tabs = resultFormSetting.importanceSetting.GetTabs();
            for (int i = 0; i < tabs.Count(); i++)
            {
                if (recordTabs.ElementAt(i).records != null)
                {
                    int numberA = ElementCounter.Count(istbridgeA, tabs.ElementAt(i).Item1);
                    int numberB = ElementCounter.Count(istbridgeB, tabs.ElementAt(i).Item1);
                    Summary.Rows.Add(
                        Summary.CollectResult(tabs.ElementAt(i).Item2, numberA, numberB, recordTabs.ElementAt(i).records));
                }
            }

            Summary.SetRow();
            return Summary;
        }
    }
}

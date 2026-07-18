using DiffCheckerLib;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using ST_BRIDGE210;

namespace STB_DiffChecker_210
{
    public class TotalRecord : ITotalRecords
    {
        public List<RecordTab> recordTabs { get; set; } = [];
        public ResultFormSetting resultFormSetting { get; set; }
        public Summary Summary { get; set; } = new Summary();
        public IST_BRIDGE istbridgeA { get; set; }
        public IST_BRIDGE istbridgeB { get; set; }

        public TotalRecord(ResultFormSetting resultFormSetting, ST_BRIDGE stbA, ST_BRIDGE stbB)
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
            ST_BRIDGE? stbridgeA = istbridgeA as ST_BRIDGE;
            ST_BRIDGE? stbridgeB = istbridgeB as ST_BRIDGE;
            for (int i = 0; i < tabs.Count(); i++)
            {
                if (recordTabs.ElementAt(i).records != null)
                {
                    int numberA = StbElementCounter.CountElements(stbridgeA, tabs.ElementAt(i).Item1);
                    int numberB = StbElementCounter.CountElements(stbridgeB, tabs.ElementAt(i).Item1);
                    Summary.Rows.Add(
                        Summary.CollectResult(tabs.ElementAt(i).Item2, numberA, numberB, recordTabs.ElementAt(i).records));
                }
            }

            Summary.SetRow();
            return Summary;
        }
    }
}

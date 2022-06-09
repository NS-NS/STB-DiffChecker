using System;

namespace STBDiffChecker
{
    /// <summary>
    /// 申請側に渡す設定用クラス
    /// </summary>
    public class ResultFormSetting
    {
        public string PathA;
        public string PathB;
        public DateTime dateTime;

        public ToleranceSetting toleranceSetting = new ToleranceSetting();
        public ImportanceSetting importanceSetting = new ImportanceSetting();
        // TODO:コメント受け渡しも必要

        internal void Export(string path)
        {
            toleranceSetting.Export();
            importanceSetting.Export();
        }
    }
}

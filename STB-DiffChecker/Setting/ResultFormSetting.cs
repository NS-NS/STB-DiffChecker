using System;

namespace STBDiffChecker
{
    /// <summary>
    /// 申請側に渡す設定用クラス
    /// </summary>
    class ResultFormSetting
    {
        internal string PathA;
        internal string PathB;
        internal DateTime dateTime;

        internal ToleranceSetting toleranceSetting = new ToleranceSetting();
        internal ImportanceSetting importanceSetting = new ImportanceSetting();
        // TODO:コメント受け渡しも必要

        internal void Export(string path)
        {
            toleranceSetting.Export();
            importanceSetting.Export();
        }
    }
}

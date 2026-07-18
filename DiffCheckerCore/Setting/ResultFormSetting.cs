using DiffCheckerLib.Interface;

namespace DiffCheckerLib.Setting
{
    /// <summary>
    /// 設定をまとめたクラス
    /// </summary>
    public class ResultFormSetting
    {
        public IToleranceSetting toleranceSetting;
        public IImportanceSetting importanceSetting;

        /// <summary>
        /// バージョンごとのToleranceSettingとImportanceSettingを受け取る
        /// </summary>
        public ResultFormSetting(IToleranceSetting toleranceSetting, IImportanceSetting importanceSetting)
        {
            this.toleranceSetting = toleranceSetting;
            this.importanceSetting = importanceSetting;
        }
    }
}

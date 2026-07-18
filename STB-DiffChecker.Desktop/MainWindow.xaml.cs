using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using DiffCheckerLib.WPF;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace STB_DiffChecker
{
    /// <summary>
    /// 統合版MainWindow
    /// ファイルのversion属性からST-Bridgeのバージョン(2.0.1/2.0.2/2.1.0)を自動判別する
    /// </summary>
    public partial class MainWindow : AbstractMainWindow
    {
        private DesktopVersionEngine engine = DesktopVersionEngine.Create("2.1.0")!;

        public override ResultFormSetting resultFormSetting { get; set; }

        //最初のウィンドウ処理用の引数
        private bool isFirst = true;

        private string baseTitle = "";

        /// <summary>
        /// 読込み中のファイルスロット('A'または'B')。PrepareForVersionでの相手側バージョン確認用
        /// </summary>
        private char loadingSlot;

        private string? versionA;
        private string? versionB;

        public MainWindow()
        {
            resultFormSetting = new(engine.Tolerance, engine.Importance);
            InitializeComponent();
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            Version ver = assembly.Version;

            this.Title += " Ver" + ver.ToString();
            baseTitle = this.Title;
            Activated += (s, e) =>
            {
                if (isFirst)
                {
                    isFirst = false;
                    RefreshSettingTables();
                    dgrdTolerance.CanUserAddRows = false;
                    dgrdTolerance.CanUserSortColumns = false;
                    dgrdImportance.CanUserAddRows = false;
                    dgrdImportance.CanUserSortColumns = false;

                    // 実行前は結果タブを非表示
                    ((TabItem)TabControl.Items[1]).IsEnabled = false;
                }
            };
        }

        /// <summary>
        /// バージョンの受け入れ確認。対応バージョンならエンジンを切り替える
        /// </summary>
        protected override bool PrepareForVersion(string version)
        {
            if (!DesktopVersionEngine.SupportedVersions.Contains(version))
            {
                _ = MessageBox.Show(
                    $"ST-Bridgeのバージョン ({version}) は未対応です。\n対応バージョン: {string.Join(" / ", DesktopVersionEngine.SupportedVersions)}",
                    "ST-Bridgeのバージョンエラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            // 既に読み込んだもう片方のファイルとバージョンが違う場合は拒否
            string? otherVersion = loadingSlot == 'A' ? versionB : versionA;
            if (otherVersion != null && otherVersion != version)
            {
                _ = MessageBox.Show(
                    $"ファイルAとファイルBのバージョンが一致していません。\n読込済: {otherVersion} / 選択したファイル: {version}",
                    "ST-Bridgeのバージョンエラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (engine.Version != version)
            {
                engine = DesktopVersionEngine.Create(version)!;
                resultFormSetting = new(engine.Tolerance, engine.Importance);
                RefreshSettingTables();
            }

            this.Title = baseTitle + "  [ST-Bridge " + version + "]";
            return true;
        }

        protected override IST_BRIDGE GetST_Bridge(string path, Encoding encoding, string schemaContent, out List<string> errors)
        {
            (IST_BRIDGE stb, List<string> loadErrors) = engine.LoadFile(path, encoding);
            errors = loadErrors;
            return stb;
        }

        /// <summary>
        /// 許容差・重要度のDataGridを現在のエンジンの内容で作り直す
        /// </summary>
        private void RefreshSettingTables()
        {
            toleranceTable = resultFormSetting.toleranceSetting.CreateTable();
            dgrdTolerance.DataContext = toleranceTable;

            importanceTable = resultFormSetting.importanceSetting.CreateTable();
            dgrdImportance.DataContext = importanceTable;
        }

        private void BtnStbA_Click(object sender, RoutedEventArgs e)
        {
            string path = GetPathWithDialog("stbファイル(*.stb)|*.stb");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            loadingSlot = 'A';
            IST_BRIDGE? istBridge = readFile(path);
            if (istBridge != null)
            {
                istBridgeA = istBridge;
                versionA = engine.Version;
                DirStbA.Text = path;
            }
        }

        private void BtnStbB_Click(object sender, RoutedEventArgs e)
        {
            string path = GetPathWithDialog("stbファイル(*.stb)|*.stb");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            loadingSlot = 'B';
            IST_BRIDGE? istBridge = readFile(path);
            if (istBridge != null)
            {
                istBridgeB = istBridge;
                versionB = engine.Version;
                DirStbB.Text = path;
            }
        }

        /// <summary>
        /// 設定の読込みボタン実行
        /// </summary>
        private void BtnReadSet_Click(object sender, RoutedEventArgs e)
        {
            string path = GetPathWithDialog("csvファイル(*.csv)|*.csv");
            if (path == string.Empty)
            {
                return;
            }

            if (File.Exists(path))
            {
                ReadCsv(path);
                RefreshSettingTables();
            }
            else
            {
                _ = MessageBox.Show("設定ファイルが存在しません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 実行
        /// </summary>
        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForm(DirStbA.Text, DirStbB.Text))
            {
                _ = MessageBox.Show("STBファイルが選択されていません", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            resultFormSetting = SetSetting();
            RunCompare();
            TabControl.SelectedIndex = 1; //タブ表示を変更
        }

        /// <summary>
        /// 設定情報をDataGridから読込み
        /// </summary>
        private ResultFormSetting SetSetting()
        {
            IToleranceSetting tolerance = engine.ToleranceFactory();
            for (int i = 0; i < toleranceTable.Rows.Count; i++)
            {
                tolerance.Tolerances()[i].Node = (double)toleranceTable.Rows[i][1];
                tolerance.Tolerances()[i].Offset = (double)toleranceTable.Rows[i][2];
            }

            IImportanceSetting importance = engine.ImportanceFactory();
            for (int i = 0; i < importanceTable.Rows.Count; i++)
            {
                importance.UserImportance()[importanceTable.Rows[i][0].ToString()] = EnumExtension.TranslateJapanese(importanceTable.Rows[i][1].ToString());
            }
            return new(tolerance, importance);
        }

        /// <summary>
        /// STBファイル比較の実行
        /// </summary>
        private void RunCompare()
        {
            //繰返し操作用にタブをフォーマット
            TabItemResult.TabCntrlResult.Items.Clear();

            //比較(全バージョン共通のTotalRecord)
            ITotalRecords totalRecord = new TotalRecord(resultFormSetting, istBridgeA, istBridgeB);
            totalRecord.Run();

            // Summaryの出力
            _ = TabItemResult.TabCntrlResult.Items.Add(totalRecord.Summary);

            //結果のタブ出力
            foreach (RecordTab recordTab in totalRecord.recordTabs)
            {
                if (recordTab.records?.Count > 0)
                {
                    _ = TabItemResult.TabCntrlResult.Items.Add(recordTab);
                }
            }

            ((TabItem)TabControl.Items[1]).IsEnabled = true;
            ((TabItem)TabControl.Items[1]).Header = "結果(変換時刻:" + totalRecord.Summary.dateTime.ToString("yyyy/MM/dd HH:mm:ss") + ")";
            //最初のタブを選択
            TabItemResult.TabCntrlResult.SelectedIndex = 0;
        }
    }
}

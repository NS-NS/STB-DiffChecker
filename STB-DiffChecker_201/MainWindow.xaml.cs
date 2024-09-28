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

namespace STB_DiffChecker_201
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : AbstractMainWindow
    {
        public override ResultFormSetting resultFormSetting { get; set; } = new(new ToleranceSetting(), new ImportanceSetting());

        //最初のウィンドウ処理用の引数
        private bool isFirst = true;

        /// <summary>
        /// メインウィンドウのコンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            Version ver = assembly.Version;

            this.Title += " Ver" + ver.ToString();
            Activated += (s, e) =>
            {
                if (isFirst)
                {
                    isFirst = false;
                    toleranceTable = resultFormSetting.toleranceSetting.CreateTable();
                    dgrdTolerance.DataContext = toleranceTable;
                    dgrdTolerance.CanUserAddRows = false;
                    dgrdTolerance.CanUserSortColumns = false;

                    importanceTable = resultFormSetting.importanceSetting.CreateTable();
                    dgrdImportance.DataContext = importanceTable;
                    dgrdImportance.CanUserAddRows = false;
                    dgrdImportance.CanUserSortColumns = false;

                    // 実行前は結果タブを非表示
                    ((TabItem)TabControl.Items[1]).IsEnabled = false;
                }

            };
        }

        protected override string GetVersion()
        {
            return "2.0.1";
        }
        protected override IST_BRIDGE GetST_Bridge(string path, Encoding encoding, string schemaContent, out List<string> errors)
        {
            return XmlValidate.LoadSTBridgeFile(path, encoding, schemaContent, new ST_BRIDGE201.ST_BRIDGE(), out errors);
        }

        private void BtnStbA_Click(object sender, RoutedEventArgs e)
        {
            string path = GetPathWithDialog("stbファイル(*.stb)|*.stb");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            IST_BRIDGE? istBridge = readFile(path);
            if (istBridge != null)
            {
                istBridgeA = istBridge;
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

            IST_BRIDGE? istBridge = readFile(path);
            if (istBridge != null)
            {
                istBridgeB = istBridge;
                DirStbB.Text = path;
            }
        }

        /// <summary>
        /// 設定の読込みボタン実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                //許容差の設定
                toleranceTable = resultFormSetting.toleranceSetting.CreateTable();
                dgrdTolerance.DataContext = toleranceTable;

                //重要度の設定
                importanceTable = resultFormSetting.importanceSetting.CreateTable();
                dgrdImportance.DataContext = importanceTable;
            }
            else
            {
                _ = MessageBox.Show("設定ファイルが存在しません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForm(DirStbA.Text, DirStbB.Text))
            {
                _ = MessageBox.Show("STBファイルが選択されていません", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            resultFormSetting = SetSetting(DirStbA.Text, DirStbB.Text);
            RunCompare();
            TabControl.SelectedIndex = 1; //タブ表示を変更
        }

        /// <summary>
        /// 設定情報をDataGridから読込み
        /// </summary>
        private ResultFormSetting SetSetting(string pathA, string pathB)
        {
            ToleranceSetting tolerance = new();
            for (int i = 0; i < toleranceTable.Rows.Count; i++)
            {
                tolerance.Tolerances()[i].Node = (double)toleranceTable.Rows[i][1];
                tolerance.Tolerances()[i].Offset = (double)toleranceTable.Rows[i][2];
            }

            ImportanceSetting importance = new();
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

            //比較
            ST_BRIDGE201.ST_BRIDGE stbA = istBridgeA as ST_BRIDGE201.ST_BRIDGE;
            ST_BRIDGE201.ST_BRIDGE stbB = istBridgeB as ST_BRIDGE201.ST_BRIDGE;
            ITotalRecords totalRecord = new TotalRecord(resultFormSetting, stbA, stbB);
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

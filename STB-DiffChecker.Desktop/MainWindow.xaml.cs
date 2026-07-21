using DiffCheckerLib;
using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using DiffCheckerLib.WPF;
using System.Data;
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

        /// <summary>
        /// 重要度の設計段階プロファイル
        /// </summary>
        private enum ImportanceProfile
        {
            S2,
            S4,
            Custom,
        }

        /// <summary>
        /// 現在選択中のプロファイル(既定はS2)
        /// </summary>
        private ImportanceProfile currentProfile = ImportanceProfile.S2;

        /// <summary>
        /// プロファイル適用後にユーザーがセルを編集したか
        /// </summary>
        private bool importanceModified;

        /// <summary>
        /// STB読込によりバージョンが確定したか(確定まで重要度タブは編集不可)
        /// </summary>
        private bool versionConfirmed;

        /// <summary>
        /// ラジオボタンをコードから設定する際のCheckedイベント抑制フラグ
        /// </summary>
        private bool suppressProfileEvents;

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

                    SetProfileRadio(currentProfile);
                    UpdateImportanceGate();

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

                if (versionConfirmed)
                {
                    // 別バージョン読込時はS2/S4を再適用(編集破棄)。カスタムは持ち越せないためS2へ戻す
                    ApplyProfileForCurrentVersion();
                }
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
            // プロファイル編集用に重要度列を編集可能化し、着色基準を取り直す(DataContext代入前に行う)
            importanceTable.Columns["Importance"]!.ReadOnly = false;
            RebuildImportanceBaseline();
            importanceModified = false;
            importanceTable.ColumnChanged += OnImportanceTableChanged;
            dgrdImportance.DataContext = importanceTable;
            UpdateProfileUI();
        }

        /// <summary>
        /// 重要度セルの編集を検知して編集済み表示(S2*等)を更新する
        /// </summary>
        private void OnImportanceTableChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column?.ColumnName != "Importance")
            {
                return;
            }

            importanceModified = HasImportanceEdits();
            UpdateProfileUI();
        }

        /// <summary>
        /// プロファイル適用時の基準値から変更されたセルがあるか
        /// </summary>
        private bool HasImportanceEdits()
        {
            foreach (DataRow row in importanceTable.Rows)
            {
                if (importanceBaseline.TryGetValue(row[0].ToString()!, out string? baseValue)
                    && baseValue != row[1].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// バージョン確定前は重要度タブを編集不可にし、案内を表示する
        /// </summary>
        private void UpdateImportanceGate()
        {
            pnlImportanceProfile.IsEnabled = versionConfirmed;
            dgrdImportance.IsEnabled = versionConfirmed;
            txtImportanceGuide.Visibility = versionConfirmed ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 最初のSTB読込成功でバージョンを確定し、既定プロファイル(S2)を適用する
        /// </summary>
        private void ConfirmVersionIfFirst()
        {
            if (versionConfirmed)
            {
                return;
            }

            versionConfirmed = true;
            ApplyProfileForCurrentVersion();
            UpdateImportanceGate();
        }

        /// <summary>
        /// 現在のプロファイルを現在バージョンへ適用する(カスタムはS2へ戻す)
        /// </summary>
        private void ApplyProfileForCurrentVersion()
        {
            if (currentProfile == ImportanceProfile.Custom)
            {
                currentProfile = ImportanceProfile.S2;
            }
            SetProfileRadio(currentProfile);
            _ = ApplyPresetProfile(currentProfile);
        }

        /// <summary>
        /// プリセット(S2/S4)を読み込んで現在の設定へ適用する
        /// </summary>
        /// <returns>適用できたか(プリセット未同梱ならfalse)</returns>
        private bool ApplyPresetProfile(ImportanceProfile profile)
        {
            string? csv = resultFormSetting.importanceSetting.GetPresetCsv(profile.ToString());
            if (csv == null)
            {
                _ = MessageBox.Show(
                    $"このバージョン(ST-Bridge {engine.Version})には{profile}プリセットがありません。",
                    "情報",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                SetProfileRadio(currentProfile);
                return false;
            }

            ApplyCsvLines(csv.Replace("\r\n", "\n").Split('\n'));
            currentProfile = profile;
            RefreshSettingTables();
            return true;
        }

        /// <summary>
        /// S2/S4ラジオボタンの選択
        /// </summary>
        private void RdoProfilePreset_Checked(object sender, RoutedEventArgs e)
        {
            if (suppressProfileEvents || !versionConfirmed)
            {
                return;
            }

            ImportanceProfile target = ReferenceEquals(sender, rdoProfileS4) ? ImportanceProfile.S4 : ImportanceProfile.S2;
            _ = ApplyPresetProfile(target);
        }

        /// <summary>
        /// カスタム(ファイル読込)ラジオボタンの選択。キャンセル時は元のプロファイルへ戻す
        /// </summary>
        private void RdoProfileCustom_Checked(object sender, RoutedEventArgs e)
        {
            if (suppressProfileEvents || !versionConfirmed)
            {
                return;
            }

            string path = GetPathWithDialog("csvファイル(*.csv)|*.csv");
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                SetProfileRadio(currentProfile);
                return;
            }

            ReadCsv(path);
            currentProfile = ImportanceProfile.Custom;
            RefreshSettingTables();
        }

        /// <summary>
        /// Checkedイベントを発火させずにラジオボタンの選択状態を設定する
        /// </summary>
        private void SetProfileRadio(ImportanceProfile profile)
        {
            suppressProfileEvents = true;
            rdoProfileS2.IsChecked = profile == ImportanceProfile.S2;
            rdoProfileS4.IsChecked = profile == ImportanceProfile.S4;
            rdoProfileCustom.IsChecked = profile == ImportanceProfile.Custom;
            suppressProfileEvents = false;
        }

        /// <summary>
        /// プロファイルの選択表示(編集済みなら「S2*」等)を更新する
        /// </summary>
        private void UpdateProfileUI()
        {
            string mark = importanceModified ? "*" : "";
            rdoProfileS2.Content = "S2" + (currentProfile == ImportanceProfile.S2 ? mark : "");
            rdoProfileS4.Content = "S4" + (currentProfile == ImportanceProfile.S4 ? mark : "");
            rdoProfileCustom.Content = "カスタム(ファイル読込)" + (currentProfile == ImportanceProfile.Custom ? mark : "");
            txtProfileState.Text = importanceModified ? "編集あり: 適用時から変更されたセルを着色表示" : "";
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
                ConfirmVersionIfFirst();
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
                ConfirmVersionIfFirst();
            }
        }

        /// <summary>
        /// 設定の読込みボタン実行
        /// </summary>
        private void BtnReadSet_Click(object sender, RoutedEventArgs e)
        {
            // 重要度を含むためカスタム読込はバージョン確定後のみ許可
            if (!versionConfirmed)
            {
                _ = MessageBox.Show(
                    "先にST-Bridgeファイルを指定してバージョンを認識させてください。",
                    "情報",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            string path = GetPathWithDialog("csvファイル(*.csv)|*.csv");
            if (path == string.Empty)
            {
                return;
            }

            if (File.Exists(path))
            {
                ReadCsv(path);
                currentProfile = ImportanceProfile.Custom;
                SetProfileRadio(currentProfile);
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
            // 編集中のセルが未確定のまま実行された場合に備えて確定させる
            _ = dgrdTolerance.CommitEdit(DataGridEditingUnit.Row, true);
            _ = dgrdImportance.CommitEdit(DataGridEditingUnit.Row, true);

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

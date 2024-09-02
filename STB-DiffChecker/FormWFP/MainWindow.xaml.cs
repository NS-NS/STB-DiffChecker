using Microsoft.Win32;
//バージョン2.4.1とする(2020.7.28時点で2.5.1はバグあり)
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using STBDiffChecker.v201.Records;
using DataGrid = System.Windows.Controls.DataGrid;

//バージョン2.4.1とする(2020.7.28時点で2.5.1はバグあり)


namespace STBDiffChecker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //バインド用のデータ
        internal ResultFormSetting resultFormSetting = new ResultFormSetting();
        internal ToleranceSetting toleranceSetting = new ToleranceSetting();
        internal ImportanceSetting importanceSetting = new ImportanceSetting();

        private DataTable toleranceTable = new DataTable("UserTolerance");
        private DataTable importanceTable = new DataTable("Importance");
        private const string RegistoryKey = @"Software\StbCompare";
        private const string Key = "Path";
        private static readonly string CsvToleranceKeyWord = "<Tolerance>";
        private static readonly string CsvImportanceKeyWord = "<Importance>";

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

            this.TitleName.Title += " Ver"+ver.ToString();
            #region 設定情報のデフォルト設定
            Activated += (s, e) =>
            {
                if (isFirst)
                {
                    isFirst = false;
                    toleranceTable = toleranceSetting.CreateTable();
                    dgrdTolerance.DataContext = toleranceTable;
                    dgrdTolerance.CanUserAddRows = false;
                    dgrdTolerance.CanUserSortColumns = false;

                    importanceTable = importanceSetting.CreateTable();
                    dgrdImportance.DataContext = importanceTable;
                    dgrdImportance.CanUserAddRows = false;
                    dgrdImportance.CanUserSortColumns = false;

                    // 実行前は結果タブを非表示
                    ((TabItem) TabControl.Items[1]).IsEnabled = false;
                }

            };
            #endregion

        }

        /// <summary>
        /// STBファイルAの読込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStbA_Click(object sender, RoutedEventArgs e)
        {
            string path = GetPathWithDialog("stbファイル(*.stb)|*.stb");
            if (File.Exists(path))
            {
                DirStbA.Text = path;
                Validate(path);
            }
        }

        private static void Validate(string path)
        {
            try
            {
                var result = XmlValidate.Validate(path);
                if (result.Count > 0)
                {
                    List<string> header = new List<string>();
                    header.Add("ST-Bridgeのフォーマットが正しくありません。");
                    header.Add("実行しても処理が落ちる可能性があります。");
                    header.Add("ST-Bridgeファイルを出力したソフトウェアの開発元にお問い合わせください。");
                    header.Add("");
                    header.Add("■■■■■■エラーメッセージ■■■■■■");
                    header.AddRange(result);
                    string errorFile = path.Replace(".stb", "_error.txt");
                    try
                    {
                        File.WriteAllLines(errorFile, header, Encoding.UTF8);
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "notepad.exe";
                        p.StartInfo.Arguments = errorFile;
                        if (!p.Start())
                        {
                            System.Windows.MessageBox.Show(
                                $"ST-Bridgeのフォーマットが正しくありません。\n詳細は{errorFile}を確認してください。",
                                "ST-Bridgeのフォーマットエラー",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show(
                            $"ST-Bridgeのフォーマットが正しくありません。\n詳細を{errorFile}に書き込もうとしましたが失敗しました。",
                            "ST-Bridgeのフォーマットエラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                System.Windows.MessageBox.Show(
                    $"ST-Bridgeのバージョンが2.0.1と異なるか、ファイルが正しくありません。\n実行しても処理が落ちる可能性があります。",
                    "ST-Bridgeのフォーマットエラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// STBファイルBの読込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStbB_Click(object sender, RoutedEventArgs e)
        {
            string path = GetPathWithDialog("stbファイル(*.stb)|*.stb");
            if (File.Exists(path))
            {
                DirStbB.Text = path;
                Validate(path);
            }
        }

        #region 設定関連
        /// <summary>
        /// 許容差DataGridのヘッダ生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_AutoGeneratingColumn_Tolerance(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // プロパティ名をもとに自動生成する列をカスタマイズします
            switch (e.PropertyName)
            {
                case "Name":
                    e.Column.Header = "種類";
                    e.Column.DisplayIndex = 0;
                    e.Column.Width = 200;
                    break;
                case "Node":
                    e.Column.Header = "基準点(mm)";
                    e.Column.DisplayIndex = 1;
                    e.Column.Width = 200;
                    break;
                case "Offset":
                    e.Column.Header = "基準点からのオフセット(mm)";
                    e.Column.DisplayIndex = 2;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("許容差に意図しない情報が入っており、UIに表示が出来ません。");
                    throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// 重要度DataGridのヘッダ生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_AutoGeneratingColumn_Importance(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // プロパティ名をもとに自動生成する列をカスタマイズします
            switch (e.PropertyName)
            {
                case "StbName":
                    e.Column.Header = "種類";
                    e.Column.DisplayIndex = 0;
                    break;
                case "Importance":
                    e.Column.Header = "重要度";
                    e.Column.DisplayIndex = 1;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("重要度に意図しない情報が入っており、UIに表示が出来ません。");
                    throw new InvalidOperationException();
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
                return;

            if (File.Exists(path))
            {
                ReadCsv(path);

                //許容差の設定
                toleranceTable = toleranceSetting.CreateTable();
                dgrdTolerance.DataContext = toleranceTable;

                //重要度の設定
                importanceTable = importanceSetting.CreateTable();
                dgrdImportance.DataContext = importanceTable;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("設定ファイルが存在しません。");
            }
        }

        /// <summary>
        /// 設定ファイルの読込み処理
        /// </summary>
        /// <param name="path"></param>
        private void ReadCsv(string path)
        {
            string[] readData = null;
            try
            {
                readData = File.ReadAllLines(path, Encoding.GetEncoding("Shift_JIS"));
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("読込み時にエラー。変換コードがおかしい");
                return;
            }

            List<string> csvTolerance = new List<string>();
            List<string> csvImportance = new List<string>();
            int flag = 0;   //Tleranceは1,Importanceは2

            #region 読込み処理
            foreach (string line in readData)
            {
                if (flag == 0)
                {
                    string head = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    if (head == CsvToleranceKeyWord)
                    {
                        flag = 1;
                        continue;
                    }
                    if (head == CsvImportanceKeyWord)
                    {
                        flag = 2;
                        continue;
                    }

                }
                else if (flag == 1)
                {
                    string head = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    if (head == CsvImportanceKeyWord)
                    {
                        flag = 2;
                        continue;
                    }
                    csvTolerance.Add(line);
                    continue;
                }
                else if (flag == 2)
                {
                    // 一応逆パターン用に設定

                    string head = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    if (head == CsvToleranceKeyWord)
                    {
                        flag = 1;
                        continue;
                    }
                    csvImportance.Add(line);
                    continue;
                }
                else
                {
                    continue;
                }
            }
            #endregion

            toleranceSetting.ImportCsv(csvTolerance);
            importanceSetting.ImportCsv(csvImportance);
        }

        /// <summary>
        /// 設定出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExportSet_Click(object sender, RoutedEventArgs e)
        {
            string path = GetSavePathWithDialog("csvファイル(*.csv) | *.csv");
            if (path == string.Empty)
                return;
            WriteCsv(path);
        }
        /// <summary>
        /// CSV出力処理
        /// </summary>
        /// <param name="path"></param>
        private void WriteCsv(string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path, false, Encoding.GetEncoding("Shift_JIS")))
                {
                    file.WriteLine(CsvToleranceKeyWord);
                    foreach (string tolerance in DataTable2String(toleranceTable))
                    {
                        file.WriteLine(tolerance);
                    }

                    file.WriteLine(CsvImportanceKeyWord);
                    foreach (string importance in DataTable2String(importanceTable))
                    {
                        file.WriteLine(importance);
                    }
                }
                System.Windows.Forms.MessageBox.Show("設定出力完了");
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("設定の出力に失敗しました");
            }
        }

        /// <summary>
        /// DataTableの情報をcsv出力用に","繋ぎで文字列にする
        /// </summary>
        private List<string> DataTable2String(DataTable dataTable)
        {
            List<string> strings = new List<string>();
            foreach (DataRow dr in dataTable.Rows)
            {
                string text = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        text = dr[j].ToString();
                    }
                    else
                    {
                        text += "," + dr[j];
                    }
                }
                strings.Add(text);
            }

            return strings;
        }


        #endregion


        #region 実行関連
        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForm())
            {
                // TODO ファイルがないエラー通知
                System.Windows.Forms.MessageBox.Show("STBファイルが選択されていません");
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
            ResultFormSetting setting = new ResultFormSetting();
            setting.PathA = DirStbA.Text;
            setting.PathB = DirStbB.Text;
            ToleranceSetting tolerance = new ToleranceSetting();
            for (int i = 0; i < toleranceTable.Rows.Count; i++)
            {
                tolerance.tolerances[i].Node = (double)toleranceTable.Rows[i][1];
                tolerance.tolerances[i].Offset = (double)toleranceTable.Rows[i][2];
            }
            setting.toleranceSetting = tolerance;

            ImportanceSetting importance = new ImportanceSetting();
            for (int i = 0; i < importanceTable.Rows.Count; i++)
            {
                importance.importances[i].SetImportance(importanceTable.Rows[i][1].ToString());
            }
            setting.importanceSetting = importance;
            return setting;
        }

        /// <summary>
        /// STBファイル比較の実行
        /// </summary>
        private void RunCompare()
        {
            //比較
            TotalRecord totalRecord = new TotalRecord(resultFormSetting);
            totalRecord.Run();

            //実行日時とファイルディレクトリ出力
            SetDatePath(totalRecord);

            //繰返し操作用にタブをフォーマット
            TabItemResult.TabCntrlResult.Items.Clear();

            // Summaryの出力
            AddSummary(totalRecord.Summary);

            //結果のタブ出力
            foreach (var recordTab in totalRecord.recordTabs)
            {
                if (recordTab.records?.Count > 0)
                {
                    AddTab(recordTab);
                }

            }

            ((TabItem) TabControl.Items[1]).IsEnabled = true;
            ((TabItem)TabControl.Items[1]).Header = "結果(変換時刻:" + totalRecord.Summary.dateTime.ToString("yyyy/MM/dd HH:mm:ss") + ")";
            //最初のタブを選択
            TabItemResult.TabCntrlResult.SelectedIndex = 0;
        }


        /// <summary>
        /// 指定名称タブを追加する
        /// </summary>
        private void AddTab(RecordTab recordTab)
        {
            //タブの定義
            TabItem tabItem = new TabItem();
            tabItem.Header = recordTab.HeaderName;

            //DataGridの定義
            DataGrid dataGrid = new DataGrid();
            dataGrid.Name = recordTab.DataGridName;

            DataTable table = Record.CreateTable(recordTab.records);
            BindingSource sorce = new BindingSource();
            sorce.DataSource = table;
            dataGrid.DataContext = sorce;
            dataGrid.ItemsSource = table.DefaultView;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserSortColumns = false;

            dataGrid.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;

            tabItem.Content = dataGrid;

            //タブコントロールに追加
            TabItemResult.TabCntrlResult.Items.Add(tabItem);
        }

        private void AddSummary(Summary summary)
        {
            //タブの定義
            TabItem tabItem = new TabItem();
            tabItem.Header = Summary.HeaderName;

            //DataGridの定義
            DataGrid dataGrid = new DataGrid();
            dataGrid.Name = Summary.DataGridName;

            DataTable table = Record.CreateSummaryTable(summary);
            BindingSource sorce = new BindingSource();
            sorce.DataSource = table;
            dataGrid.DataContext = sorce;
            dataGrid.ItemsSource = table.DefaultView;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserSortColumns = false;

            tabItem.Content = dataGrid;

            //タブコントロールに追加
            TabItemResult.TabCntrlResult.Items.Add(tabItem);
        }

        private void dataGrid_AutoGeneratingColumn(object s, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Record.JapaneseParentElement:
                    e.Column.Header = Record.JapaneseParentElement;
                    break;
                case Record.JapaneseKey:
                    e.Column.Header = Record.JapaneseKey;
                    break;
                case Record.JapaneseItem:
                    e.Column.Header = Record.JapaneseItem;
                    break;
                case Record.JapaneseA:
                    e.Column.Header = Record.JapaneseA;
                    break;
                case Record.JapaneseB:
                    e.Column.Header = Record.JapaneseB;
                    break;
                case Record.JapaneseConsistency:
                    e.Column.Header = Record.JapaneseConsistency;
                    break;
                case Record.JapaneseImportance:
                    e.Column.Header = Record.JapaneseImportance;
                    break;
                case Record.JapaneseComment:
                    e.Column.Header = Record.JapaneseComment;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
            }

        }


        /// <summary>
        /// 実行日時とSTBファイルのディレクトリを出力
        /// </summary>
        /// <param name="totalRecord"></param>
        private void SetDatePath(TotalRecord totalRecord)
        {
            resultFormSetting.dateTime = totalRecord.Summary.dateTime;
        }

        #endregion


        #region 共通処理
        /// <summary>
        /// ファイルのパス(ファイルを開く)を取得する
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private string GetPathWithDialog(string kind)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.InitialDirectory = ReadPass(RegistoryKey);
            dialog.Filter = kind;  //[ファイルの種類]
            dialog.FilterIndex = 1;    //[ファイルの種類]でFilterセットのデータ形式を選択
            dialog.Title = "開くファイルを選択してください";  //タイトル設定
            dialog.RestoreDirectory = true;    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            dialog.CheckFileExists = true;     //存在しないファイルの名前が指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            dialog.CheckPathExists = true;     //存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                SetPass(Path.GetDirectoryName(dialog.FileName));
                return dialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// ファイルのパス(ファイルを保存)を取得する
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private string GetSavePathWithDialog(string kind)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.InitialDirectory = ReadPass(RegistoryKey);
            dialog.Filter = kind;  //[ファイルの種類]
            dialog.FilterIndex = 1;    //[ファイルの種類]でFilterセットのデータ形式を選択
            dialog.Title = "出力先とファイル名を設定して下さい";  //タイトル設定
            dialog.RestoreDirectory = true;    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            dialog.CheckFileExists = false;     //存在しないファイルの名前が指定されたとき警告を表示する
            dialog.CheckPathExists = true;     //存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                SetPass(Path.GetDirectoryName(dialog.FileName));
                return dialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// レジストリキーからパスを取得
        /// </summary>
        /// <param name="RgstKeyPass"></param>
        /// <returns></returns>
        public string ReadPass(string RgstKeyPass)
        {
            //初期パスは最後に開いたフォルダとする
            try
            {
                RegistryKey rk = Registry.CurrentUser.CreateSubKey(RgstKeyPass);  //using Microsoft.Win32;が必要
                if (rk == null)
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);    //デスクトップ
                }
                else
                {
                    return (string)rk.GetValue(Key);
                }
            }
            catch (Exception)
            {
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        /// <summary>
        /// レジストリキーにパスを設定
        /// </summary>
        /// <param name="path"></param>
        public void SetPass(string path)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(RegistoryKey, true);
            rk?.SetValue(Key, path);
        }

        /// <summary>
        /// STBファイルの有無を確認
        /// </summary>
        /// <returns></returns>
        private bool CheckForm()
        {
            if (!File.Exists(DirStbA.Text) ||
                !File.Exists(DirStbB.Text))
                return false;
            return true;
        }
        #endregion

    }

}

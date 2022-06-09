using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using STBDiffChecker.AttributeType;
using STBDiffChecker.Enum;
using DataGrid = System.Windows.Controls.DataGrid;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

namespace STBDiffChecker
{

    /// <summary>
    /// TabItemResult.xaml の相互作用ロジック
    /// </summary>
    public partial class TabItemResult : UserControl
    {
        private const string RegistoryKey = @"Software\StbCompare";
        private const string Key = "Path";
        public TabItemResult()
        {
            InitializeComponent();
            SetFilterComboBox();

            // チェックボックス文字変更
            ChkCmpNothing.Content = Consistency.ElementIncomparable.ToJapanese();
            ChkCmpInconsistent.Content = Consistency.Inconsistent.ToJapanese();
            ChkCmpAlmostMatch.Content = Consistency.AlmostMatch.ToJapanese();
            ChkCmpConsistent.Content = Consistency.Consistent.ToJapanese();

            ChkImpNotApplicapable.Content = Importance.NotApplicable.ToJapanese();
            ChkImpUnnecessary.Content = Importance.Unnecessary.ToJapanese();
            ChkImpOptional.Content = Importance.Optional.ToJapanese();
            ChkImpRequired.Content = Importance.Required.ToJapanese();
        }

        private void UpdateFilter(object sender, EventArgs e)
        {
            if (TabCntrlResult?.SelectedItem is TabItem item)
            {
                if (item.Content is DataGrid dataGrid)
                {
                    if (dataGrid.Name == Summary.DataGridName)
                        return;
                    BindingSource sorce = dataGrid.DataContext as BindingSource;
                    sorce.Filter = MakeFilterText();
                }

            }
        }

        private string MakeFilterText()
        {
            string filter = string.Empty;
            List<string> ResultFilter = new List<string>();
            if (ChkCmpNothing.IsChecked != null && (bool)ChkCmpNothing.IsChecked)
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.ElementIncomparable.ToJapanese() + "'");

            if (ChkCmpInconsistent.IsChecked != null && (bool)ChkCmpInconsistent.IsChecked)
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.Inconsistent.ToJapanese() + "'");

            if (ChkCmpAlmostMatch.IsChecked != null && (bool)ChkCmpAlmostMatch.IsChecked)
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.Incomparable.ToJapanese() + "'");

            //Consistent
            if (ChkCmpConsistent.IsChecked != null && (bool)ChkCmpConsistent.IsChecked)
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.Consistent.ToJapanese() + "'");

            if (ResultFilter.Count != 0)
            {
                filter = "(" + string.Join(" OR ", ResultFilter) + ")";
            }


            List<string> ImportanceFilter = new List<string>();
            if (ChkImpNotApplicapable.IsChecked != null && (bool)ChkImpNotApplicapable.IsChecked)
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.NotApplicable.ToJapanese() + "'");

            if (ChkImpUnnecessary.IsChecked != null && (bool)ChkImpUnnecessary.IsChecked)
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.Unnecessary.ToJapanese() + "'");

            if (ChkImpOptional.IsChecked != null && (bool)ChkImpOptional.IsChecked)
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.Optional.ToJapanese() + "'");

            if (ChkImpRequired.IsChecked != null && (bool)ChkImpRequired.IsChecked)
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.Required.ToJapanese() + "'");

            if (ImportanceFilter.Count != 0)
            {
                if (filter != string.Empty)
                {
                    filter += " AND ";
                }

                filter += "(" + string.Join(" OR ", ImportanceFilter) + ")";
            }


            if (!string.IsNullOrEmpty(CmbKey1.SelectedItem?.ToString()) && TxtKey1.Text != string.Empty)
            {
                if (filter != string.Empty)
                {
                    filter += " AND ";
                }

                filter += CmbKey1.SelectedItem.ToString() + " LIKE '%" + TxtKey1.Text + "%'";
            }

            if (!string.IsNullOrEmpty(CmbKey2.SelectedItem?.ToString()) && TxtKey2.Text != string.Empty)
            {
                if (filter != string.Empty)
                {
                    filter += " AND ";
                }

                filter += CmbKey2.SelectedItem.ToString() + " LIKE '%" + TxtKey2.Text + "%'";
            }

            return filter;
        }

        /// <summary>
        /// 「Excel出力」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            string path = GetSavePathWithDialog("xlsx ファイル(.xlsx)|*.xlsx|All Files (*.*)|*.*");
            if (String.IsNullOrWhiteSpace(path) == false)
            {
                //以下Excel処理
                XSSFWorkbook wb;
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    wb = new XSSFWorkbook();
                    wb.CreateSheet("dummy");

                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("ファイル作成に失敗しました。");
                    return;
                }

                ReadDataGrid(wb);

                //1番目のシートは削除
                wb.RemoveSheetAt(0);
                wb.SetForceFormulaRecalculation(true);//計算の再実行(これを入れないと計算結果の値が変わらない)

                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        wb.Write(fs);
                    }
                    System.Windows.Forms.MessageBox.Show("Excelファイルの作成完了。");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Excelファイル保存時にエラー発生。");
                }
            }
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
        /// 結果の読込み
        /// </summary>
        private void ReadDataGrid(XSSFWorkbook wb)
        {
            foreach (var item in TabCntrlResult.Items)
            {
                TabItem tabItem = item as TabItem;
                DataGrid dataGrid = tabItem.Content as DataGrid;
                BindingSource source = dataGrid.DataContext as BindingSource;
                source.Filter = "";
                DataTable table = ((DataView)source.List).ToTable();
                WriteExcelSheet(wb, tabItem.Header.ToString(), table);
            }

        }

        /// <summary>
        /// Excelシートの作成
        /// </summary>
        private void WriteExcelSheet(XSSFWorkbook wb, string SheetName, DataTable table)
        {
            //シートの作成
            ISheet ws = wb.CreateSheet(SheetName);

            //ヘッダの生成
            for (int i = 0; i < table.Columns.Count; i++)
            {
                writeCellValue(ws, i, 0, table.Columns[i].ColumnName);
            }

            for (int i = 0; i < table.Columns.Count; i++)
            {
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    writeCellValue(ws, i, j + 1, table.Rows[j][i]);
                }
            }
        }

        // セルへの書込み処理　参照元:https://www.sejuku.net/blog/100771
        /// <summary>
        /// セル書き込み(書き込む値が文字列の場合)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="idxColumn"></param>
        /// <param name="idxRow"></param>
        /// <param name="value"></param>
        static void writeCellValue(ISheet sheet, int idxColumn, int idxRow, object obj)
        {
            var row = sheet.GetRow(idxRow) ?? sheet.CreateRow(idxRow); //指定した行を取得できない時はエラーとならないよう新規作成している
            var cell = row.GetCell(idxColumn) ?? row.CreateCell(idxColumn); //一行上の処理の列版
            if (obj == null)
                cell.SetCellValue(string.Empty);

            string value = obj.ToString();
            /*
            double d;
            if (double.TryParse(value, out d))
            {
                cell.SetCellValue(d);
                cell.SetCellType(CellType.Numeric); 
            }
            */

            cell.SetCellValue(value);
        }

        /// <summary>
        /// フィルタ用のコンボボックス設定
        /// </summary>
        private void SetFilterComboBox()
        {
            var combo = new List<string>()
            {
                Record.JapaneseParentElement,
                Record.JapaneseKey,
                Record.JapaneseItem,
                Record.JapaneseA,
                Record.JapaneseB,
                Record.JapaneseComment
            };
            foreach (var item in combo)
            {
                CmbKey1.Items.Add(item);
                CmbKey2.Items.Add(item);
            }

            CmbKey1.SelectedIndex = 0;
            CmbKey2.SelectedIndex = 1;
        }

    }
}

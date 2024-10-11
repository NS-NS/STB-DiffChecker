using DiffCheckerLib.Enum;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DataGrid = System.Windows.Controls.DataGrid;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

namespace DiffCheckerLib.WPF
{
    /// <summary>
    /// 結果表示用のユーザーコントロール
    /// </summary>
    public partial class TabItemResult : UserControl
    {
        public TabItemResult()
        {
            InitializeComponent();
            SetFilterComboBox();
        }

        /// <summary>
        /// 結果フィルターの更新
        /// </summary>
        private void UpdateFilter(object sender, EventArgs e)
        {
            if (TabCntrlResult?.SelectedItem is TabItem item)
            {
                if (item.Content is DataGrid dataGrid)
                {
                    if (dataGrid.Name == Summary.Header)
                    {
                        return;
                    }

                    if (dataGrid.ItemsSource is DataView dataView)
                    {
                        dataView.RowFilter = MakeFilterText();
                    }
                }

            }
        }


        /// <summary>
        /// フィルターのテキストを作成
        /// 何も選択していない場合は全選択
        /// </summary>
        private string MakeFilterText()
        {
            string filter = string.Empty;
            List<string> ResultFilter = [];
            if (ChkCmpAttributeNothing.IsChecked != null && (bool)ChkCmpAttributeNothing.IsChecked)
            {
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.Incomparable.ToJapanese() + "'");
            }

            if (ChkCmpElementNothing.IsChecked != null && (bool)ChkCmpElementNothing.IsChecked)
            {
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.ElementIncomparable.ToJapanese() + "'");
            }

            if (ChkCmpInconsistent.IsChecked != null && (bool)ChkCmpInconsistent.IsChecked)
            {
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.Inconsistent.ToJapanese() + "'");
            }

            if (ChkCmpAlmostMatch.IsChecked != null && (bool)ChkCmpAlmostMatch.IsChecked)
            {
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.AlmostMatch.ToJapanese() + "'");
            }

            //Consistent
            if (ChkCmpConsistent.IsChecked != null && (bool)ChkCmpConsistent.IsChecked)
            {
                ResultFilter.Add(Record.JapaneseConsistency + @" = '" + Consistency.Consistent.ToJapanese() + "'");
            }

            if (ResultFilter.Count != 0)
            {
                filter = "(" + string.Join(" OR ", ResultFilter) + ")";
            }


            List<string> ImportanceFilter = [];
            if (ChkImpNotApplicapable.IsChecked != null && (bool)ChkImpNotApplicapable.IsChecked)
            {
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.NotApplicable.ToJapanese() + "'");
            }

            if (ChkImpUnnecessary.IsChecked != null && (bool)ChkImpUnnecessary.IsChecked)
            {
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.Unnecessary.ToJapanese() + "'");
            }

            if (ChkImpOptional.IsChecked != null && (bool)ChkImpOptional.IsChecked)
            {
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.Optional.ToJapanese() + "'");
            }

            if (ChkImpRequired.IsChecked != null && (bool)ChkImpRequired.IsChecked)
            {
                ImportanceFilter.Add(Record.JapaneseImportance + @" = '" + Importance.Required.ToJapanese() + "'");
            }

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
            if (string.IsNullOrWhiteSpace(path) == false)
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
                    _ = wb.CreateSheet("dummy");

                }
                catch
                {
                    _ = MessageBox.Show("ファイル作成に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ReadDataGrid(wb);

                //1番目のシートは削除
                wb.RemoveSheetAt(0);
                wb.SetForceFormulaRecalculation(true);//計算の再実行(これを入れないと計算結果の値が変わらない)

                try
                {
                    using (FileStream fs = new(path, FileMode.Create))
                    {
                        wb.Write(fs);
                    }
                    _ = MessageBox.Show("Excelファイルの作成完了。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    _ = MessageBox.Show("Excelファイル保存時にエラー発生。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            Microsoft.Win32.SaveFileDialog dialog = new()
            {
                InitialDirectory = ReadPass(AbstractMainWindow.RegistoryKey),
                Filter = kind,  //[ファイルの種類]
                FilterIndex = 1,    //[ファイルの種類]でFilterセットのデータ形式を選択
                Title = "出力先とファイル名を設定して下さい",  //タイトル設定
                RestoreDirectory = true,    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                CheckFileExists = false,     //存在しないファイルの名前が指定されたとき警告を表示する
                CheckPathExists = true     //存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            };

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
                    return (string)rk.GetValue(AbstractMainWindow.Key);
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
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(AbstractMainWindow.RegistoryKey, true);
            rk?.SetValue(AbstractMainWindow.Key, path);
        }

        /// <summary>
        /// 結果の読込み
        /// </summary>
        private void ReadDataGrid(XSSFWorkbook wb)
        {
            foreach (object item in TabCntrlResult.Items)
            {
                RecordTab tabItem = item as RecordTab;
                WriteExcelSheet(wb, tabItem.Header.ToString(), tabItem.dataTable);
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
            IRow row = sheet.GetRow(idxRow) ?? sheet.CreateRow(idxRow); //指定した行を取得できない時はエラーとならないよう新規作成している
            ICell cell = row.GetCell(idxColumn) ?? row.CreateCell(idxColumn); //一行上の処理の列版
            if (obj == null)
            {
                cell.SetCellValue(string.Empty);
            }

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
            List<string> combo =
            [
                Record.JapaneseParentElement,
                Record.JapaneseKey,
                Record.JapaneseItem,
                Record.JapaneseA,
                Record.JapaneseB,
                Record.JapaneseComment
            ];
            foreach (string item in combo)
            {
                _ = CmbKey1.Items.Add(item);
                _ = CmbKey2.Items.Add(item);
            }

            CmbKey1.SelectedIndex = 0;
            CmbKey2.SelectedIndex = 1;
        }

    }
}

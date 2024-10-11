using DiffCheckerLib.Interface;
using DiffCheckerLib.Setting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DiffCheckerLib.WPF
{
    /// <summary>
    /// WPFのGUIは継承できなさそうなので、関数の共通部分を抜き出したクラス
    /// </summary>
    public abstract class AbstractMainWindow : Window
    {
        public abstract ResultFormSetting resultFormSetting { get; set; }

        /// <summary>
        /// ユーザー設定の許容差テーブル
        /// </summary>
        protected DataTable toleranceTable = new("UserTolerance");

        /// <summary>
        /// 重要度設定テーブル
        /// </summary>
        protected DataTable importanceTable = new("Importance");

        /// <summary>
        /// 前回設定保存用のレジストリキー
        /// </summary>
        internal const string RegistoryKey = @"NS-NS\STB-DiffChecker";
        internal const string Key = "Path";

        /// <summary>
        /// csv用のマーカー
        /// </summary>
        protected static readonly string CsvToleranceKeyWord = "<Tolerance>";
        protected static readonly string CsvImportanceKeyWord = "<Importance>";

        protected IST_BRIDGE istBridgeA;
        protected IST_BRIDGE istBridgeB;

        /// <summary>
        /// バージョンを指定(例:2.0.1)
        /// </summary>
        protected abstract string GetVersion();

        /// <summary>
        /// 各バージョンのST_BRIDGEを取得
        /// </summary>
        protected abstract IST_BRIDGE GetST_Bridge(string path, Encoding encoding, string schemaContent, out List<string> errors);

        /// <summary>
        /// エンコーディングとバージョンを確認してファイルからST_BRIDGEを読み込む
        /// </summary>
        protected IST_BRIDGE? readFile(string path)
        {
            // Encodingとversionチェック
            (string, string) encodingAndVersion = XmlValidate.CheckVersionAndEncoding(path);

            if (encodingAndVersion.Item1 != GetVersion())
            {
                _ = MessageBox.Show(
                                     $"ST-Bridgeのバージョンが" + GetVersion() + "と異なります。",
                                     "ST-Bridgeのバージョンエラー",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                return null;
            }

            Encoding encoding;
            if (encodingAndVersion.Item2 != "utf-8")
            {
                // .NETの標準以外
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                encoding = Encoding.GetEncoding(encodingAndVersion.Item2);
            }
            else
            {
                encoding = Encoding.UTF8;
            }

            List<string> errors;
            IST_BRIDGE istBridge = GetST_Bridge(path, encoding, resultFormSetting.importanceSetting.GetSchemaContent(), out errors);
            if (errors.Count > 0)
            {
                List<string> header =
                [
                    "ST-Bridgeのフォーマットが正しくありません。",
                        "実行しても処理が落ちる可能性があります。",
                        "ST-Bridgeファイルを出力したソフトウェアの開発元にお問い合わせください。",
                        "",
                        "■■■■■■エラーメッセージ■■■■■■",
                        .. errors,
                    ];
                string errorFile = path.Replace(".stb", "_error.txt");
                try
                {
                    File.WriteAllLines(errorFile, header, Encoding.UTF8);
                    System.Diagnostics.Process p = new();
                    p.StartInfo.FileName = "notepad.exe";
                    p.StartInfo.Arguments = errorFile;
                    if (!p.Start())
                    {
                        _ = MessageBox.Show(
                            $"ST-Bridgeのフォーマットが正しくありません。\n詳細は{errorFile}を確認してください。",
                            "ST-Bridgeのフォーマットエラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch
                {
                    _ = MessageBox.Show(
                        $"ST-Bridgeのフォーマットが正しくありません。\n詳細を{errorFile}に書き込もうとしましたが失敗しました。",
                        "ST-Bridgeのフォーマットエラー",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }

            return istBridge;
        }

        /// <summary>
        /// 許容差DataGridのヘッダ生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dataGrid_AutoGeneratingColumn_Tolerance(object sender, DataGridAutoGeneratingColumnEventArgs e)
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
                    _ = MessageBox.Show("許容差に意図しない情報が入っており、UIに表示が出来ません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// 重要度DataGridのヘッダ生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dataGrid_AutoGeneratingColumn_Importance(object sender, DataGridAutoGeneratingColumnEventArgs e)
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
                    _ = MessageBox.Show("重要度に意図しない情報が入っており、UIに表示が出来ません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// ファイルのパス(ファイルを開く)を取得する
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        protected string GetPathWithDialog(string kind)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = ReadPass(RegistoryKey),
                Filter = kind,  //[ファイルの種類]
                FilterIndex = 1,    //[ファイルの種類]でFilterセットのデータ形式を選択
                Title = "開くファイルを選択してください",  //タイトル設定
                RestoreDirectory = true,    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                CheckFileExists = true,     //存在しないファイルの名前が指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
                CheckPathExists = true     //存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                SetPass(System.IO.Path.GetDirectoryName(dialog.FileName));
                return dialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// レジストリキーからパスを取得
        /// </summary>
        /// <param name="RgstKeyPass"></param>
        /// <returns></returns>
        private string ReadPass(string RgstKeyPass)
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
        private void SetPass(string path)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(RegistoryKey, true);
            rk?.SetValue(Key, path);
        }

        /// <summary>
        /// DataTableの情報をcsv出力用に","繋ぎで文字列にする
        /// </summary>
        private List<string> DataTable2String(DataTable dataTable)
        {
            List<string> strings = [];
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


        /// <summary>
        /// CSV出力処理
        /// </summary>
        /// <param name="path"></param>
        private void WriteCsv(string path)
        {
            try
            {
                using (StreamWriter file = new(path, false))
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
                _ = MessageBox.Show("設定出力完了", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"設定出力失敗。{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// ファイルのパス(ファイルを保存)を取得する
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private string GetSavePathWithDialog(string kind)
        {
            SaveFileDialog dialog = new()
            {
                InitialDirectory = ReadPass(RegistoryKey),
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
                SetPass(System.IO.Path.GetDirectoryName(dialog.FileName));
                return dialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 設定ファイルの読込み処理
        /// </summary>
        /// <param name="path"></param>
        protected void ReadCsv(string path)
        {
            string[] readData;
            try
            {
                readData = File.ReadAllLines(path);
            }
            catch
            {
                _ = MessageBox.Show("読込み時にエラー。変換コードがおかしい。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string> csvTolerance = [];
            List<string> csvImportance = [];
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

            resultFormSetting.toleranceSetting.ImportCsv(csvTolerance);
            resultFormSetting.importanceSetting.ImportCsv(csvImportance);
        }

        /// <summary>
        /// 設定出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnExportSet_Click(object sender, RoutedEventArgs e)
        {
            string path = GetSavePathWithDialog("csvファイル(*.csv) | *.csv");
            if (path == string.Empty)
            {
                return;
            }
            WriteCsv(path);
        }


        /// <summary>
        /// STBファイルの有無を確認
        /// </summary>
        /// <returns></returns>
        protected bool CheckForm(string pathA, string pathB)
        {
            return File.Exists(pathA) && File.Exists(pathB);
        }
    }
}

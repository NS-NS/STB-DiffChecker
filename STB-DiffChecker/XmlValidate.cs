using System.Collections.Generic;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using STBDiffChecker.Properties;

namespace STBDiffChecker
{
    static class XmlValidate
    {
        private static readonly List<ValidationEventArgs> Errors = new List<ValidationEventArgs>();

        internal static void Validate(string path)
        {
            Errors.Clear();
            List<string> result = new List<string>();
            switch (CheckVersion(path))
            {
                case "2.0.1":
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Resources.STBridge_v201)))
                    {
                        ValidationEventHandler eventHandler = new ValidationEventHandler(SettingValidationEventHandler);
                        XmlSchema schema = XmlSchema.Read(stream, eventHandler);
                        XmlSchemaSet schemaSet = new XmlSchemaSet();
                        schemaSet.Add(schema);
                        System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(path, System.Xml.Linq.LoadOptions.SetLineInfo);
                        xdoc.Validate(schemaSet, eventHandler);
                    }

                    foreach (var e in Errors)
                    {
                        if (e.Severity == System.Xml.Schema.XmlSeverityType.Error)
                        {
                            result.Add($"Error:({e.Exception.LineNumber}) {e.Message}");
                        }
                        else if (e.Severity == System.Xml.Schema.XmlSeverityType.Warning)
                        {
                            result.Add($"Warning:({e.Exception.LineNumber}) {e.Message}");
                        }
                    }

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

                    break;

                default:
                    System.Windows.MessageBox.Show(
                            $"ST-Bridgeのバージョンが2.0.1と異なるか、ファイルが正しくありません。\n実行しても処理が落ちる可能性があります。",
                            "ST-Bridgeのフォーマットエラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    break;
            }
        }

        private static void SettingValidationEventHandler(object sender, ValidationEventArgs e)
        {
            Errors.Add(e);
        }

        private static string CheckVersion(string stbPath)
        {
            string version;
            using (StreamReader reader = new StreamReader(stbPath, Encoding.UTF8))
            {
                string text = reader.ReadToEnd();
                Match match2 = Regex.Match(text, "version=\"[0-9]+[.][\\d]+[.][\\d]+");
                version = match2.Value.Replace("version=\"", string.Empty);
            }

            return version;
        }
    }
}

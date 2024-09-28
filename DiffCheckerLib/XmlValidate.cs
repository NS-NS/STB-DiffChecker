using DiffCheckerLib.Interface;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DiffCheckerLib
{
    public static class XmlValidate
    {
        // エラーメッセージを保持するリスト
        private static List<string> validationErrors = [];

        // リソースファイルからスキーマを読み込む
        public static string GetEmbeddedXsd(Assembly assembly, string resourcePath, Encoding encoding)
        {
            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                throw new FileNotFoundException("リソースが見つかりません: " + resourcePath);
            }

            using StreamReader reader = new(stream, encoding);
            return reader.ReadToEnd(); // XSDファイルの内容を文字列として返す
        }

        internal static (string, string) CheckVersionAndEncoding(string filePath)
        {
            // デフォルトのエンコーディング
            string encoding = string.Empty;
            string version = string.Empty;
            // まずは、ファイルをバイナリで読み込み、BOM (Byte Order Mark) を確認する
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new(fileStream, detectEncodingFromByteOrderMarks: true);
            // ファイルの最初の部分を読んで、エンコーディングを検出
            char[] buffer = new char[1024];
            int readChars = reader.Read(buffer, 0, buffer.Length);

            // 読み込んだ文字列を検査
            string xmlSnippet = new(buffer, 0, readChars);

            // XMLの宣言部分をパース
            using (StringReader stringReader = new(xmlSnippet))
            {
                using XmlReader xmlReader = XmlReader.Create(stringReader);
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        version = xmlReader.GetAttribute("version");
                        encoding = xmlReader.GetAttribute("encoding");
                        continue;
                    }

                    // 開始要素をチェック（最初の "ST_BRIDGE" などの要素）
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.HasAttributes)
                    {
                        // 属性をループして "version" 属性をチェック
                        while (xmlReader.MoveToNextAttribute())
                        {
                            if (xmlReader.Name == "version")
                            {
                                version = xmlReader.Value; // versionの値を取得
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(version))
                        {
                            break;
                        }

                        // 属性を元の位置に戻す
                        _ = xmlReader.MoveToElement();
                    }

                }
            }

            // BOMや宣言がない場合は、StreamReaderの検出したエンコーディングを使用
            if (string.IsNullOrEmpty(encoding))
            {
                encoding = reader.CurrentEncoding.WebName;
            }

            return (version, encoding);
        }

        public static T LoadSTBridgeFile<T>(string filePath, Encoding encoding, string schemaContent, T istBridge, out List<string> errors)
            where T : IST_BRIDGE
        {
            validationErrors.Clear();
            // XMLリーダー設定（妥当性検証用）
            XmlReaderSettings settings = new()
            {
                ValidationType = ValidationType.Schema
            };
            _ = settings.Schemas.Add(null, XmlReader.Create(new StringReader(schemaContent)));
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

            // 妥当性検証をしながらファイルを読み込む
            using XmlReader reader = XmlReader.Create(filePath, settings);
            XmlSerializer serializer = new(typeof(T)); // STBridgeクラスに合わせる

            T stbData = (T)serializer.Deserialize(reader);

            errors = validationErrors;
            return stbData;
        }

        // // 妥当性検証のコールバック
        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            // エラーメッセージに行番号と列番号を追加する
            string locationInfo = "";

            if (e.Exception is XmlSchemaException schemaException)
            {
                locationInfo = $" (Line: {schemaException.LineNumber}, Position: {schemaException.LinePosition})";
            }

            // 警告かエラーかでメッセージを分岐
            if (e.Severity == XmlSeverityType.Warning)
            {
                validationErrors.Add($"Warning: {e.Message}{locationInfo}");
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                validationErrors.Add($"Error: {e.Message}{locationInfo}");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using STB_DiffCheckerLib.Properties;

namespace STBDiffChecker
{
    public static class XmlValidate
    {
        private static readonly List<ValidationEventArgs> Errors = new List<ValidationEventArgs>();

        public static List<string> Validate(string path)
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

                    return result;

                default:
                    throw new NotImplementedException();
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

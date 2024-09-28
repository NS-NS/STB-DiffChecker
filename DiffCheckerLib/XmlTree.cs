using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace DiffCheckerLib
{
    /// <summary>
    /// XMLスキーマから要素・属性一覧を取得するクラス
    /// SettingAll.csvを作成するベースとしても使用する
    /// </summary>
    public static class XmlTree
    {
        private static List<string> validationErrors = [];

        /// <summary>
        /// XMLスキーマから要素のパス一覧を取得するメイン関数
        /// </summary>
        public static List<string> GetElementPaths(string xsdContent, IReadOnlyList<string> elementNames, out List<string> errors)
        {
            validationErrors.Clear();
            List<string> paths = [];  // 要素のパスを格納するリスト

            XmlSchema schema;
            using (StringReader stringReader = new(xsdContent))
            {
                using XmlReader reader = XmlReader.Create(stringReader);
                schema = XmlSchema.Read(reader, ValidationCallback);
            }

            XmlSchemaSet schemaSet = new();
            _ = schemaSet.Add(schema);
            schemaSet.Compile();

            // 指定した要素をルートとして走査を開始
            foreach (string name in elementNames)
            {
                foreach (XmlSchemaElement element in schema.Elements.Values)
                {
                    if (element.Name == name)
                    {
                        TraverseSchema(element, "", paths, schema);
                    }
                }
            }

            errors = validationErrors;
            return paths;
        }

        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            validationErrors.Add($"Validation error: {e.Message}");
        }

        // 要素名を取得するヘルパー関数
        private static string GetElementName(XmlSchemaElement element, XmlSchema schema)
        {
            // ref で参照されている場合、その要素名を取得
            if (!string.IsNullOrWhiteSpace(element.RefName.Name))
            {
                XmlQualifiedName refName = element.RefName;

                // スキーマから参照されている要素を解決
                if (schema.Elements[new XmlQualifiedName(refName.Name, refName.Namespace)] is XmlSchemaElement referencedElement)
                {
                    return referencedElement.Name;  // 参照先の要素名を返す
                }
            }

            // 通常の要素の場合
            return !string.IsNullOrWhiteSpace(element.Name) ? element.Name : "UnnamedElement";
        }

        // 再帰的にスキーマを走査してXPathを生成
        private static void TraverseSchema(XmlSchemaElement element, string parentPath, List<string> paths, XmlSchema schema)
        {
            // 要素名が空でないか確認し、空なら名前をデフォルトに設定
            string elementName = GetElementName(element, schema);

            string currentPath = $"{parentPath}/{elementName}";
            paths.Add(currentPath);  // 要素のパスを追加

            if (element.ElementSchemaType is XmlSchemaComplexType complexType)
            {
                // 属性の処理
                foreach (XmlSchemaObject attribute in complexType.Attributes)
                {
                    if (attribute is XmlSchemaAttribute schemaAttribute)
                    {
                        paths.Add($"{currentPath}/@{schemaAttribute.Name}");
                    }
                    else if (attribute is XmlSchemaAttributeGroupRef groupRef)
                    {
                        XmlSchemaAttributeGroup? group = FindAttributeGroup(groupRef.RefName, schema);
                        if (group != null)
                        {
                            foreach (XmlSchemaAttribute subAttribute in group.Attributes.OfType<XmlSchemaAttribute>())
                            {
                                paths.Add($"{currentPath}/@{subAttribute.Name}");
                            }
                        }
                    }
                }

                // 子要素の処理 (Sequence, Choice, Allに対応)
                if (complexType.ContentTypeParticle is XmlSchemaSequence sequence)
                {
                    foreach (XmlSchemaObject item in sequence.Items)
                    {
                        if (item is XmlSchemaElement childElement)
                        {
                            TraverseSchema(childElement, currentPath, paths, schema);
                        }
                        else if (item is XmlSchemaChoice sequenceChoice)
                        {
                            foreach (XmlSchemaObject choice in sequenceChoice.Items)
                            {
                                if (choice is XmlSchemaElement choiceElement)
                                {
                                    TraverseSchema(choiceElement, currentPath, paths, schema);
                                }
                            }
                        }
                    }
                }
                else if (complexType.ContentTypeParticle is XmlSchemaChoice choice)
                {
                    foreach (XmlSchemaObject item in choice.Items)
                    {
                        if (item is XmlSchemaSequence choiceSequence)
                        {
                            foreach (XmlSchemaObject choiceItem in choiceSequence.Items)
                            {
                                if (choiceItem is XmlSchemaElement choiceElement)
                                {
                                    TraverseSchema(choiceElement, currentPath, paths, schema);
                                }
                            }
                        }
                        else if (item is XmlSchemaElement choiceElement)
                        {
                            TraverseSchema(choiceElement, currentPath, paths, schema);
                        }
                    }
                }
                else if (complexType.ContentTypeParticle is XmlSchemaAll all)
                {
                    foreach (XmlSchemaObject item in all.Items)
                    {
                        if (item is XmlSchemaElement childElement)
                        {
                            TraverseSchema(childElement, currentPath, paths, schema);
                        }
                    }
                }
            }
            else if (element.ElementSchemaType is XmlSchemaSimpleType)
            {
                paths.Add($"{currentPath}");
            }
        }

        // 属性グループを検索するための関数
        private static XmlSchemaAttributeGroup? FindAttributeGroup(XmlQualifiedName groupRefName, XmlSchema schema)
        {
            foreach (XmlSchemaObject item in schema.Items)
            {
                if (item is XmlSchemaAttributeGroup group && group.Name == groupRefName.Name)
                {
                    return group;
                }
            }
            return null;
        }

    }
}

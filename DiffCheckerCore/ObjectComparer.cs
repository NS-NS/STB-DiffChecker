using DiffCheckerLib.Enum;
using DiffCheckerLib.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DiffCheckerLib
{
    /// <summary>
    /// スキーマから自動生成されたST_BRIDGEクラスを比較するクラス
    /// 再帰的に属性・要素を取得して比較する
    /// </summary>
    public static class ObjectComparer
    {
        /// <summary>
        /// StbApplyConditionsListのset_default当て込みで実効値を補完したことを示すコメント
        /// </summary>
        private const string ApplyConditionDefaultComment = "StbApplyConditionsListのset_defaultにより当て込み";

        /// <summary>
        /// 型ごとのリフレクション情報キャッシュ
        /// GetProperties/GetCustomAttributesは高コストなため型単位で1回だけ解決する
        /// </summary>
        private sealed class TypeShape
        {
            public required PropertyInfo[] AllProperties { get; init; }
            public required PropertyInfo[] AttributeProperties { get; init; }
            public required PropertyInfo[] ElementProperties { get; init; }
            public required Dictionary<string, PropertyInfo> SpecifiedProperties { get; init; }
        }

        private static readonly ConcurrentDictionary<Type, TypeShape> typeShapeCache = new();

        private static TypeShape GetShape(Type type)
        {
            return typeShapeCache.GetOrAdd(type, t =>
            {
                PropertyInfo[] all = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Dictionary<string, PropertyInfo> specified = [];
                foreach (PropertyInfo p in all)
                {
                    if (p.Name.EndsWith("Specified"))
                    {
                        specified[p.Name[..^"Specified".Length]] = p;
                    }
                }

                return new TypeShape
                {
                    AllProperties = all,
                    AttributeProperties = all.Where(p => p.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any()).ToArray(),
                    ElementProperties = all.Where(p => !p.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any()).ToArray(),
                    SpecifiedProperties = specified,
                };
            });
        }

        /// <summary>
        /// UserImportanceで設定した重要度を取得する
        /// XMLパスが一致するものを取得するが、参照先を比較しているものについては末尾が一致しているかを確認する
        /// </summary>
        public static Importance CheckImportance(string key, Dictionary<string, Importance> importanceDict)
        {
            if (importanceDict.ContainsKey(key))
            {
                return importanceDict[key];
            }

            // StbSecSteelなどは参照先をみているので末尾が一致しているか
            foreach (string importanceKey in importanceDict.Keys)
            {
                // StbSecRoll-BOXの-はクラス名では省略されているので
                Regex regex = new($"{importanceKey.Replace("-", string.Empty)}$");
                if (regex.IsMatch(key))
                {
                    return importanceDict[importanceKey];
                }
            }

            throw new Exception("Importance not found");
        }

        /// <summary>
        /// ST_BRIDGEの2つのファイルを比較するメイン関数
        /// ルートとなる要素をまとめたListを引数に取って、その名前をキーに結果をまとめたDictionaryを返す
        /// </summary>
        public static Dictionary<string, List<DiffCheckerLib.Record>> CompareSTBridgeFiles(IST_BRIDGE stbA, IST_BRIDGE stbB, IReadOnlyList<string> targetClassNames, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            Dictionary<string, List<DiffCheckerLib.Record>> result = [];

            // ターゲットクラスごとに比較を実施
            foreach (string className in targetClassNames)
            {
                // ターゲットクラスを比較し、結果を取得
                List<DiffCheckerLib.Record> records = CompareClass(stbA, stbB, className, importanceDict, toleranceSetting);

                // 結果が存在する場合、結果をDictionaryに追加
                if (records.Count > 0)
                {
                    result[className] = records;
                }
            }

            return result;
        }

        /// <summary>
        /// classNameをルートにそれ以下のツリーすべての比較を行う関数
        /// </summary>
        private static List<DiffCheckerLib.Record> CompareClass(IST_BRIDGE stbA, IST_BRIDGE stbB, string className, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            List<DiffCheckerLib.Record> records = [];

            // それぞれのクラスがST_BRIDGEの中に存在するか確認して比較する
            (object, string) classA = GetClassInstance(stbA, className, "/ST_BRIDGE");
            (object, string) classB = GetClassInstance(stbB, className, "/ST_BRIDGE");

            if (classA.Item1 == null && classB.Item1 == null)
            {
                return records;
            }

            if (classA.Item1 != null || classB.Item1 != null)
            {
                // Xmlパスをルートにして比較開始
                string xmlPath = classA.Item1 != null ? classA.Item2 : classB.Item2;
                CompareAndGroup(classA.Item1, stbA, classB.Item1, stbB, xmlPath, [], records, importanceDict, toleranceSetting);
            }

            return records;
        }

        /// <summary>
        /// 要素の比較関数
        /// </summary>
        private static void CompareAndGroup(object objA, IST_BRIDGE stbA, object objB, IST_BRIDGE stbB, string xmlPath, List<string> key, List<DiffCheckerLib.Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            // 両方がnullの場合は比較不要
            if (objA == null && objB == null)
            {
                return;
            }

            Type typeA = objA?.GetType();
            Type typeB = objB?.GetType();

            if (typeA != typeB)
            {
                string elementOnlyKey = typeA != null ? $"{typeA.Name}" : $"{typeB.Name}";
                records.Add(new Record(xmlPath, key, elementOnlyKey, typeA != null ? "有" : "無", typeB != null ? "有" : "無", Consistency.ElementIncomparable, CheckImportance(xmlPath, importanceDict)));
                //return;
            }

            // 配列の場合の比較処理
            if ((typeA != null && typeA.IsArray) || (typeB != null && typeB.IsArray))
            {
                Array arrayA = (Array)objA;
                Array arrayB = (Array)objB;
                CompareArrays(arrayA, stbA, arrayB, stbB, xmlPath, records, importanceDict, key, toleranceSetting);
                return;
            }
            else
            {
                // プロパティやフィールドを比較する
                CompareProperties(objA, stbA, objB, stbB, xmlPath, key, records, importanceDict, toleranceSetting);
            }
        }

        /// <summary>
        /// プロパティとフィールドの比較を行う関数
        /// </summary>
        public static void CompareProperties(object objA, IST_BRIDGE stbA, object objB, IST_BRIDGE stbB, string xmlPath, List<string> key, List<Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting)
        {
            if (objA == null || objB == null)
            {
                string elementOnlyKey = objA != null ? objA.GetType().Name : objB.GetType().Name;
                records.Add(new Record(xmlPath, key, elementOnlyKey, objA != null ? "有" : "無", objB != null ? "有" : "無", Consistency.Inconsistent, CheckImportance(xmlPath, importanceDict)));
                return;
            }

            Type typeA = objA.GetType();
            Type typeB = objB.GetType();

            // 両方の型が一致していることを確認
            if (typeA != typeB)
            {
                string elementOnlyKey = typeA != null ? typeA.Name : typeB.Name;
                records.Add(new Record(xmlPath, key, elementOnlyKey, typeA != null ? "有" : "無", typeB != null ? "有" : "無", Consistency.Inconsistent, CheckImportance(xmlPath, importanceDict)));
                return;
            }

            // プロパティの比較（型ごとのキャッシュを利用）
            TypeShape shape = GetShape(objA.GetType());

            // 属性→要素の順に比較
            foreach (PropertyInfo property in shape.AttributeProperties)
            {
                object valueA = property.GetValue(objA);
                object valueB = property.GetValue(objB);
                string appliedDefaultComment = null;

                // Specifiedフラグを持たない属性(文字列属性等)がnullの場合、
                // StbApplyConditionsListのset_defaultによる当て込みを試みる
                if (valueA == null && objA is IApplyConditionDefault applyDefaultA && applyDefaultA.TryGetApplyConditionDefault(property, stbA, out object defaultValueA))
                {
                    valueA = defaultValueA;
                    appliedDefaultComment = ApplyConditionDefaultComment;
                }
                if (valueB == null && objB is IApplyConditionDefault applyDefaultB && applyDefaultB.TryGetApplyConditionDefault(property, stbB, out object defaultValueB))
                {
                    valueB = defaultValueB;
                    appliedDefaultComment = ApplyConditionDefaultComment;
                }

                if (valueA == null && valueB == null)
                {
                    continue;
                }

                string propertyName = "@" + property.Name;
                string propertyPath = $"{xmlPath}/{propertyName}";

                // 片方がnullの場合は、不一致として記録
                if (valueA == null || valueB == null)
                {
                    records.Add(new Record(propertyPath, key, propertyName, valueA != null ? "有" : "無", valueB != null ? "有" : "無", Consistency.Inconsistent, CheckImportance(propertyPath, importanceDict)));
                    continue;
                }

                if (shape.SpecifiedProperties.TryGetValue(property.Name, out PropertyInfo specifiedProp))
                {
                    bool specifiedA = (bool)specifiedProp.GetValue(objA, null);
                    bool specifiedB = (bool)specifiedProp.GetValue(objB, null);

                    // Specifiedがfalse(未入力)の場合、StbApplyConditionsListのset_defaultによる当て込みを試みる
                    if (!specifiedA && objA is IApplyConditionDefault applyDefaultSpecA && applyDefaultSpecA.TryGetApplyConditionDefault(property, stbA, out object defaultSpecA))
                    {
                        valueA = defaultSpecA;
                        specifiedA = true;
                        appliedDefaultComment = ApplyConditionDefaultComment;
                    }
                    if (!specifiedB && objB is IApplyConditionDefault applyDefaultSpecB && applyDefaultSpecB.TryGetApplyConditionDefault(property, stbB, out object defaultSpecB))
                    {
                        valueB = defaultSpecB;
                        specifiedB = true;
                        appliedDefaultComment = ApplyConditionDefaultComment;
                    }

                    if (specifiedA == false && specifiedB == false)
                    {
                        continue; // FieldSpecifiedがfalseの場合はスキップ
                    }
                    else if (specifiedA == false || specifiedB == false)
                    {
                        records.Add(new Record(propertyPath, key, propertyName, specifiedA ? "有" : "無", specifiedB ? "有" : "無", Consistency.Inconsistent, CheckImportance(propertyPath, importanceDict)));
                        continue;
                    }
                    else
                    {
                        if (objA is IProperty hasReference && hasReference.IsSpecial(property))
                        {
                            hasReference.CompareProperty(property, stbA, objB, stbB, propertyPath, key, records, importanceDict, toleranceSetting);
                        }
                        else
                        {
                            CompareProperty(property, valueA, stbA, valueB, stbB, propertyPath, key, records, importanceDict, toleranceSetting, appliedDefaultComment);
                        }
                        continue;
                    }
                }
                else if (objA is IProperty hasReference && hasReference.IsSpecial(property))
                {
                    hasReference.CompareProperty(property, stbA, objB, stbB, propertyPath, key, records, importanceDict, toleranceSetting);
                    continue;
                }
                else if (property.Name.EndsWith("Specified"))
                {
                    continue;
                }


                CompareProperty(property, valueA, stbA, valueB, stbB, propertyPath, key, records, importanceDict, toleranceSetting, appliedDefaultComment);
            }


            foreach (PropertyInfo property in shape.ElementProperties)
            {
                object valueA = property.GetValue(objA);
                object valueB = property.GetValue(objB);

                if (valueA == null && valueB == null)
                {
                    continue;
                }

                string propertyName = property.Name == "Item"
                    ? valueA.GetType()?.Name != null ? valueA.GetType().Name : valueB.GetType().Name
                    : property.Name;
                string propertyPath = $"{xmlPath}/{propertyName}";


                // 片方がnullの場合は、不一致として記録
                if (valueA == null || valueB == null)
                {
                    records.Add(new Record(propertyPath, key, propertyName, valueA != null ? "有" : "無", valueB != null ? "有" : "無", Consistency.Inconsistent, CheckImportance(propertyPath, importanceDict)));
                    continue;
                }

                if (objA is IProperty hasReference && hasReference.IsSpecial(property))
                {
                    hasReference.CompareProperty(property, stbA, objB, stbB, propertyPath, key, records, importanceDict, toleranceSetting);
                    continue;
                }
                else if (property.Name.EndsWith("Specified"))
                {
                    continue;
                }

                CompareProperty(property, valueA, stbA, valueB, stbB, propertyPath, key, records, importanceDict, toleranceSetting);
            }
        }

        /// <summary>
        /// プロパティの比較を行う関数
        /// </summary>
        internal static void CompareProperty(PropertyInfo property, object valueA, IST_BRIDGE stbA, object valueB, IST_BRIDGE stbB, string xmlPath, List<string> key, List<DiffCheckerLib.Record> records, Dictionary<string, Importance> importanceDict, IToleranceSetting toleranceSetting, string comment = null)
        {
            string propertyName = property.Name != "Item" ? property.Name : valueA.GetType().Name;

            if (property.PropertyType == typeof(double))
            {
                records.Add(new Record(
                    xmlPath,
                    key,
                    "@" + propertyName,
                    valueA?.ToString(),
                    valueB?.ToString(),
                     Math.Abs((double)valueA - (double)valueB) < 0.000001 ? Consistency.Consistent : Consistency.Inconsistent,
                    CheckImportance(xmlPath, importanceDict)
                )
                { Comment = comment });
            }
            else if (property.PropertyType.IsPrimitive)
            {
                records.Add(new Record(
                    xmlPath,
                    key,
                    "@" + propertyName,
                    valueA?.ToString(),
                    valueB?.ToString(),
                     valueA?.ToString() == valueB?.ToString() ? Consistency.Consistent : Consistency.Inconsistent,
                    CheckImportance(xmlPath, importanceDict)
                )
                { Comment = comment });
            }
            else if (property.PropertyType == typeof(string))
            {
                records.Add(new Record(
                    xmlPath,
                    key,
                    "@" + propertyName,
                    valueA?.ToString(),
                    valueB?.ToString(),
                     valueA?.ToString() == valueB?.ToString() ? Consistency.Consistent : Consistency.Inconsistent,
                    CheckImportance(xmlPath, importanceDict)
                )
                { Comment = comment });
            }
            else if (property.PropertyType.IsEnum)
            {
                records.Add(new Record(
                    xmlPath,
                    key,
                    "@" + propertyName,
                    valueA.ToString(),
                    valueB.ToString(),
                    valueA.Equals(valueB) ? Consistency.Consistent : Consistency.Inconsistent,
                    CheckImportance(xmlPath, importanceDict)
                )
                { Comment = comment });
            }
            else if (property.PropertyType.IsArray)
            {
                Array arrayA = (Array)valueA;
                Array arrayB = (Array)valueB;

                // Nullableによって場合分け
                if (property.GetCustomAttributes(typeof(XmlArrayItemAttribute), false).FirstOrDefault() is XmlArrayItemAttribute xmlArrayItemAttribute && xmlArrayItemAttribute.IsNullable == false)
                {
                    CompareArrays(arrayA, stbA, arrayB, stbB, xmlPath, records, importanceDict, key, toleranceSetting);
                }
                else
                {
                    string last = xmlPath.Split('/').Last();
                    Regex regex = new("/" + last + "$");
                    if (regex.Match(xmlPath).Success)
                    {
                        xmlPath = regex.Replace(xmlPath, "");
                    }
                    CompareArrays(arrayA, stbA, arrayB, stbB, xmlPath, records, importanceDict, key, toleranceSetting);
                }
            }
            else
            {
                // 両方ともnullではない場合、再帰的に比較
                CompareAndGroup(valueA, stbA, valueB, stbB, xmlPath, key, records, importanceDict, toleranceSetting);
            }
        }

        /// <summary>
        /// クラス名を指定して、そのクラスのインスタンスとXMLパスを取得する関数
        /// currentPathで
        /// </summary>
        public static (object, string) GetClassInstance(object obj, string className, string currentPath = "")
        {
            if (obj == null)
            {
                return (null, null);
            }

            Type objType = obj.GetType();
            PropertyInfo classProperty = objType.GetProperty(className);

            // プロパティが見つかれば、その値とXMLパスを返す
            if (classProperty != null)
            {
                string xmlPath = $"{currentPath}/{classProperty.Name}";
                return (classProperty.GetValue(obj), xmlPath);
            }

            // 直下のプロパティに見つからなかった場合、さらに再帰的に探索
            foreach (PropertyInfo property in GetShape(objType).AllProperties)
            {
                // プロパティの値を取得（nullの場合はスキップ）
                object propertyValue = property.GetValue(obj);
                if (propertyValue == null)
                {
                    continue;
                }

                string newPath = $"{currentPath}/{property.Name}";

                // 配列やIEnumerableの場合は各要素を探索
                if (propertyValue is System.Collections.IEnumerable enumerable)
                {
                    foreach (object item in enumerable)
                    {
                        // 再帰的に探索
                        (object result, string path) = GetClassInstance(item, className, newPath);
                        if (result != null)
                        {
                            return (result, path);
                        }
                    }
                }
                else
                {
                    // 再帰的に探索
                    (object result, string path) = GetClassInstance(propertyValue, className, newPath);
                    if (result != null)
                    {
                        return (result, path);
                    }
                }
            }

            // 見つからなければnullを返す
            return (null, null);
        }

        /// <summary>
        /// 配列の比較を行う関数
        /// 自動生成で配列要素として出てくる場合
        /// </summary>
        private static void CompareArrays(Array arrayA, IST_BRIDGE stbA, Array arrayB, IST_BRIDGE stbB, string parentName, List<DiffCheckerLib.Record> records, Dictionary<string, Importance> importanceDict, List<string> key, IToleranceSetting toleranceSetting)
        {
            List<object> unmatchedInA = [];
            if (arrayA != null)
            {
                foreach (object itemA in arrayA)
                {
                    unmatchedInA.Add(itemA);
                }
            }
            List<object> unmatchedInB = [];
            if (arrayB != null)
            {
                foreach (object itemB in arrayB)
                {
                    unmatchedInB.Add(itemB);
                }
            }

            // 型ごとのバケットに分けて探索量を減らす
            // (CompareToは型不一致で必ずfalseを返すため、同型のみの探索で結果は変わらない)
            Dictionary<Type, List<object>> bucketsB = [];
            foreach (object itemB in unmatchedInB)
            {
                if (!bucketsB.TryGetValue(itemB.GetType(), out List<object> bucket))
                {
                    bucket = [];
                    bucketsB[itemB.GetType()] = bucket;
                }
                bucket.Add(itemB);
            }

            // 比較済みを除外するためのリスト
            if (arrayA != null)
            {
                foreach (object itemA in arrayA)
                {
                    IEnumerable<string> key2 = [];
                    Consistency consistent = Consistency.Inconsistent;
                    object matchedItemB = null;

                    if (bucketsB.TryGetValue(itemA.GetType(), out List<object> candidates) && candidates.Count > 0)
                    {
                        if (itemA is ICompare compareA)
                        {
                            // GetKeyは要素Aにつき1回だけ計算する
                            key2 = compareA.GetKey(stbA);
                            foreach (object itemB in candidates)
                            {
                                if (itemB is ICompare compareB)
                                {
                                    if (compareA.CompareTo(compareB, stbA, stbB))
                                    {
                                        consistent = Consistency.Consistent;
                                        matchedItemB = itemB;
                                        break;
                                    }
                                    if (compareA.AlmostCompareTo(compareB, stbA, stbB, toleranceSetting))
                                    {
                                        consistent = Consistency.AlmostMatch;
                                        matchedItemB = itemB;
                                        break;
                                    }
                                }
                                else
                                {
                                    // 同型でICompareでない場合は型一致でマッチ(従来動作)
                                    consistent = Consistency.Consistent;
                                    matchedItemB = itemB;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // ICompareを持たない要素は型一致でマッチ(従来動作)
                            consistent = Consistency.Consistent;
                            matchedItemB = candidates[0];
                        }
                    }

                    if (matchedItemB != null)
                    {
                        _ = bucketsB[matchedItemB.GetType()].Remove(matchedItemB);
                        // 一致するものがあれば再帰的に比較
                        string elementOnlyKey = $"{itemA.GetType().Name}";

                        // 最後の要素が同じ場合は追加しない
                        string xmlPath = parentName.Split('/').Last() == itemA.GetType().Name ? parentName : $"{parentName}/{itemA.GetType().Name}";
                        List<string> newKey = new(key);
                        newKey.AddRange(key2);
                        records.Add(new Record(xmlPath, newKey, elementOnlyKey, itemA.GetType().Name, matchedItemB.GetType().Name, consistent, CheckImportance(xmlPath, importanceDict)));
                        CompareAndGroup(itemA, stbA, matchedItemB, stbB, xmlPath, newKey, records, importanceDict, toleranceSetting);
                        _ = unmatchedInB.Remove(matchedItemB); // 比較済みの要素は除外
                        _ = unmatchedInA.Remove(itemA); // 比較済みの要素は除外
                    }
                }
            }

            // unmatchedInAとunmatchedInBに残った要素は一致しないものとして記録
            foreach (object unmatchedA in unmatchedInA)
            {
                string elementOnlyKey = $"{unmatchedA.GetType().Name}";
                // 最後の要素が同じ場合は追加しない
                string xmlPath = parentName.Split('/').Last() == elementOnlyKey ? parentName : $"{parentName}/{elementOnlyKey}";
                if (unmatchedA is ICompare compareA)
                {
                    List<string> newKey = new(key);
                    newKey.AddRange(compareA.GetKey(stbA));
                    records.Add(new Record(xmlPath, newKey, elementOnlyKey, "有", "無", Consistency.ElementIncomparable, CheckImportance(xmlPath, importanceDict)));
                }
            }

            foreach (object unmatchedB in unmatchedInB)
            {
                string elementOnlyKey = $"{unmatchedB.GetType().Name}";
                // 最後の要素が同じ場合は追加しない
                string xmlPath = parentName.Split('/').Last() == elementOnlyKey ? parentName : $"{parentName}/{elementOnlyKey}";
                if (unmatchedB is ICompare compareB)
                {
                    List<string> newKey = new(key);
                    newKey.AddRange(compareB.GetKey(stbB));
                    records.Add(new Record(xmlPath, newKey, elementOnlyKey, "無", "有", Consistency.ElementIncomparable, CheckImportance(xmlPath, importanceDict)));
                }
            }
        }
    }
}

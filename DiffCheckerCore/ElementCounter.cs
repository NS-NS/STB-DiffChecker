using DiffCheckerLib.Interface;
using System;
using System.Linq;
using System.Reflection;

namespace DiffCheckerLib
{
    /// <summary>
    /// タブ名(要素名)に対応する要素数をリフレクションで取得するクラス
    /// 概要表示用。バージョン(スキーマ)に依存しない
    /// </summary>
    public static class ElementCounter
    {
        public static int Count(IST_BRIDGE? stb, string tabName)
        {
            if (stb == null)
            {
                return 0;
            }

            (object? obj, _) = ObjectComparer.GetClassInstance(stb, tabName, "/ST_BRIDGE");
            if (obj == null)
            {
                return 0;
            }

            if (obj is Array array)
            {
                return array.Length;
            }

            // StbJointsやStbConnectionsのような「配列だけを持つコンテナ」は配列要素数の合計を返す
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties.Length > 0 && properties.All(p => p.PropertyType.IsArray))
            {
                return properties.Sum(p => (p.GetValue(obj) as Array)?.Length ?? 0);
            }

            return 1;
        }
    }
}

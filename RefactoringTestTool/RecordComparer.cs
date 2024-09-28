using STBDiffChecker;

namespace RefactoringTestTool
{
    internal class RecordComparer
    {
        public static bool AreRecordsEqual(List<Record> list1, List<Record> list2)
        {
            bool isEqual = true;
            // Listの長さが異なる場合は即座に不一致と判断
            if (list1?.Count != list2?.Count)
            {
                Console.WriteLine($"List size mismatch: list1.Count = {list1?.Count ?? 0}, list2.Count = {list2?.Count ?? 0}");
                isEqual = false;
            }

            if (list1 == null || list2 == null)
            {
                return isEqual;
            }

            // list2に対応する要素がすべてあるかどうかを確認するためのリストコピーを作成
            List<Record> unmatchedRecords = new(list2);

            // list1の各Recordをlist2内の要素と比較
            foreach (Record record1 in list1)
            {
                // list2の中で一致する要素を探す
                Record matchingRecord = unmatchedRecords.FirstOrDefault(record2 => AreRecordsEqual(record1, record2));

                if (matchingRecord != null)
                {
                    // 一致するものが見つかった場合、list2の対象から削除（比較済みのため）
                    _ = unmatchedRecords.Remove(matchingRecord);
                }
                else
                {
                    // 一致するものが見つからない場合は不一致
                    Console.WriteLine($"Record not found in list2 for ParentElement: {record1.ParentElement}, Key: {record1.Key}, Item:{record1.Item}, A:{record1.A}, B:{record1.B}, Consistency:{record1.Consistency}, Importance:{record1.Importance}");
                    isEqual = false;
                }
            }

            foreach (Record record2 in unmatchedRecords)
            {
                Console.WriteLine($"Record not found in list1 for ParentElement: {record2.ParentElement}, Key: {record2.Key}, Item:{record2.Item}, A:{record2.A}, B:{record2.B}, Consistency:{record2.Consistency}, Importance:{record2.Importance}");
            }


            // すべて一致した場合はTrueを返す
            return isEqual;
        }

        private static bool AreRecordsEqual(Record record1, Record record2)
        {
            bool isEqual = true; // 全体の一致状況を示すフラグ

            // キーが一致するか
            if (record1.Key.Count() != record2.Key.Count())
            {
                int key1 = record1.Key.Count();
                int key2 = record2.Key.Count();
                _ = key1 == key2;
                isEqual = false;
            }
            else
            {
                for (int i = 0; i < record1.Key.Count(); i++)
                {
                    if (record1.Key[i] != record2.Key[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            // アイテムが一致するか
            if (record1.Item != record2.Item)
            {
                isEqual = false;
            }

            // 親要素が一致するか
            if (record1.ParentElement != record2.ParentElement)
            {
                isEqual = false;
            }

            if (isEqual == false)
            {
                return false;
            }

            // Aの値が一致するか
            if (record1.A != record2.A)
            {
                isEqual = false;
            }

            // Bの値が一致するか
            if (record1.B != record2.B)
            {
                isEqual = false;
            }

            // 一致度が一致するか
            if (record1.Consistency != record2.Consistency)
            {
                isEqual = false;
            }

            // 重要度が一致するか
            if (record1.Importance != record2.Importance)
            {
                isEqual = false;
            }

            // コメントが一致するか
            if (record1.Comment != record2.Comment)
            {
                isEqual = false;
            }
            if (isEqual == false)
            {
                Console.WriteLine($"Key: {record1.Key} vs {record2.Key}");
                Console.WriteLine($"Item: {record1.Item} vs {record2.Item}");
                Console.WriteLine($"ParentElement: {record1.ParentElement} vs {record2.ParentElement}");
                Console.WriteLine($"A: {record1.A} vs {record2.A}");
                Console.WriteLine($"B: {record1.B} vs {record2.B}");
                Console.WriteLine($"Consistency: {record1.Consistency} vs {record2.Consistency}");
                Console.WriteLine($"Importance: {record1.Importance} vs {record2.Importance}");
                Console.WriteLine($"Comment: {record1.Comment} vs {record2.Comment}");
            }

            return isEqual;
        }

    }
}

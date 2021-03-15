using System.Collections.Generic;

namespace STBDiffChecker
{
    internal class RecordTab
    {
        internal List<Record> records;
        internal readonly string DataGridName;
        internal readonly string HeaderName;

        internal RecordTab(string dataGridName, string headerName)
        {
            DataGridName = dataGridName;
            HeaderName = headerName;
        }
    }
}
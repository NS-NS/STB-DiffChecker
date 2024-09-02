using System.Collections.Generic;

namespace STBDiffChecker
{
    public class RecordTab
    {
        public List<Record> records;
        public readonly string DataGridName;
        public readonly string HeaderName;

        internal RecordTab(string dataGridName, string headerName)
        {
            DataGridName = dataGridName;
            HeaderName = headerName;
        }
    }
}
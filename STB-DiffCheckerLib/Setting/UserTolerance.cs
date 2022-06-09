namespace STBDiffChecker
{
    class UserTolerance
    {
        internal string Name;
        internal double Node;
        internal double Offset;

        internal UserTolerance(string name)
        {
            Name = name;
        }
    }
}
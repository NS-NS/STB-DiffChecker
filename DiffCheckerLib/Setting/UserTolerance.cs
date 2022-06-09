namespace STBDiffChecker
{
    public class UserTolerance
    {
        public string Name;
        public double Node;
        public double Offset;

        internal UserTolerance(string name)
        {
            Name = name;
        }
    }
}
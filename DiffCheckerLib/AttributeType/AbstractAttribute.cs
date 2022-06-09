using STBDiffChecker.Enum;

namespace STBDiffChecker.AttributeType
{
    public abstract class AbstractAttribute
    {
        public string StbName { get; }
        public Importance Importance { get; private set; }

        internal AbstractAttribute(string stbName)
        {
            this.StbName = stbName;
        }

        internal AbstractAttribute(string stbName, Importance importance) : this(stbName)
        {
            this.Importance = importance;
        }

        public void SetImportance(Importance importance)
        {
            this.Importance = importance;
        }

        public void SetImportance(string importance)
        {
            this.Importance = EnumExtension.TranslateJapanese(importance);
        }

        protected string ParentElement()
        {
            var split = this.StbName.Split('/');

            if (split.Length == 1)
                return string.Empty;
            else if (split[split.Length - 2] == string.Empty)
                return "/";
            else
                return split[split.Length - 2];
        }

        protected string Item()
        {
            var split = this.StbName.Split('/');
            return split[split.Length - 1];
        }
    }
}

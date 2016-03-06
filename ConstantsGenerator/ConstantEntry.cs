using Csv;

namespace ConstantGenerator
{
    public class ConstantEntry : Key
    {
        public ConstantEntry(ICsvLine csvLine)
        {
            ParseFullName(csvLine[NameHeader].Trim());
            Comment = csvLine[CommentHeader].Trim();
            LineIndex = csvLine.Index;
        }

        private const string NameHeader = "Name";
        private const string CommentHeader = "Comment";

        public string Comment { get; private set; }
        public int LineIndex { get; private set; }

        public override string ToString()
        {
            return "Line=" + LineIndex + ", " + FullName + ", " + Comment;
        }
    }
}
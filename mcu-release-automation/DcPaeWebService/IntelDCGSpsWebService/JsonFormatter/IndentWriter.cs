using System.Text;

namespace IntelDCGSpsWebService.Models
{
    public class IndentWriter
    {
        private readonly StringBuilder _result = new StringBuilder();
        private int _indentLevel;

        public void Indent()
        {
            _indentLevel++;
        }

        public void UnIndent()
        {
            if (_indentLevel > 0)
                _indentLevel--;
        }

        public void WriteLine(string line)
        {
            _result.AppendLine(CreateIndent() + line);
        }

        private string CreateIndent()
        {
            StringBuilder indent = new StringBuilder();
            for (int i = 0; i < _indentLevel; i++)
                indent.Append("    ");
            return indent.ToString();
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}

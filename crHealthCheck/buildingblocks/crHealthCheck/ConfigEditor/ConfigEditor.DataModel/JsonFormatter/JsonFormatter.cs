using System.Text;

namespace ConfigEditor.DataModel
{
    public class JsonFormatter
    {
        private readonly StringWalker _walker;
        private readonly IndentWriter _writer = new IndentWriter();
        private readonly StringBuilder _currentLine = new StringBuilder();
        private bool _quoted;

        public JsonFormatter(string json)
        {
            _walker = new StringWalker(json);
            ResetLine();
        }

        public void ResetLine()
        {
            _currentLine.Length = 0;
        }

        public string Format()
        {
            while (MoveNextChar())
            {
                if (this._quoted == false && this.IsOpenBracket())
                {
                    this.WriteCurrentLine();
                    this.AddCharToLine();
                    this.WriteCurrentLine();
                    _writer.Indent();
                }
                else if (this._quoted == false && this.IsCloseBracket())
                {
                    this.WriteCurrentLine();
                    _writer.UnIndent();
                    this.AddCharToLine();
                }
                else if (this._quoted == false && this.IsColon())
                {
                    this.AddCharToLine();
                    this.WriteCurrentLine();
                }
                else
                {
                    AddCharToLine();
                }
            }
            this.WriteCurrentLine();
            return _writer.ToString();
        }

        private bool MoveNextChar()
        {
            bool success = _walker.MoveNext();
            if (this.IsApostrophe())
            {
                this._quoted = !_quoted;
            }
            return success;
        }

        public bool IsApostrophe()
        {
            return this._walker.CurrentChar == '"' && this._walker.IsEscaped == false;
        }

        public bool IsOpenBracket()
        {
            return this._walker.CurrentChar == '{'
                || this._walker.CurrentChar == '[';
        }

        public bool IsCloseBracket()
        {
            return this._walker.CurrentChar == '}'
                || this._walker.CurrentChar == ']';
        }

        public bool IsColon()
        {
            return this._walker.CurrentChar == ',';
        }

        private void AddCharToLine()
        {
            this._currentLine.Append(_walker.CurrentChar);
        }

        private void WriteCurrentLine()
        {
            string line = this._currentLine.ToString().Trim();
            if (line.Length > 0)
            {
                _writer.WriteLine(line);
            }
            this.ResetLine();
        }
    }
}

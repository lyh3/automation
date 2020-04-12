namespace ConfigEditor.DataModel
{
    public class StringWalker
    {
        private readonly string _s;

        public int Index { get; private set; }
        public bool IsEscaped { get; private set; }
        public char CurrentChar { get; private set; }

        public StringWalker(string s)
        {
            _s = s;
            this.Index = -1;
        }

        public bool MoveNext()
        {
            if (this.Index == _s.Length - 1)
                return false;

            if (IsEscaped == false)
                IsEscaped = CurrentChar == '\\';
            else
                IsEscaped = false;
            this.Index++;
            CurrentChar = _s[Index];
            return true;
        }
    }
}

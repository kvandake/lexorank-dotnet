namespace LexoAlgorithm.NumeralSystems
{
    public class LexoNumeralSystem36 : ILexoNumeralSystem
    {
        private readonly char[] _digits = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

        public string Name => "Base36";

        public int GetBase()
        {
            return 36;
        }

        public char GetPositiveChar()
        {
            return '+';
        }

        public char GetNegativeChar()
        {
            return '-';
        }

        public char GetRadixPointChar()
        {
            return ':';
        }

        public int ToDigit(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return ch - 48;
            if (ch >= 'a' && ch <= 'z')
                return ch - 97 + 10;
            throw new LexoException("Not valid digit: " + ch);
        }

        public char ToChar(int digit)
        {
            return _digits[digit];
        }
    }
}
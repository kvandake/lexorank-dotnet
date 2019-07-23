namespace LexoAlgorithm.NumeralSystems
{
    public class LexoNumeralSystem64 : ILexoNumeralSystem
    {
        private readonly char[] _digits =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ^_abcdefghijklmnopqrstuvwxyz".ToCharArray();

        public string Name => "Base64";

        public int GetBase()
        {
            return 64;
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
            if (ch >= 'A' && ch <= 'Z')
                return ch - 65 + 10;
            if (ch == '^')
                return 36;
            if (ch == '_')
                return 37;
            if (ch >= 'a' && ch <= 'z')
                return ch - 97 + 38;
            throw new LexoException("Not valid digit: " + ch);
        }

        public char ToChar(int digit)
        {
            return _digits[digit];
        }
    }
}
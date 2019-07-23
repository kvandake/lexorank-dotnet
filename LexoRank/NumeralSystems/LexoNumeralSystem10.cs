namespace LexoAlgorithm.NumeralSystems
{
    public class LexoNumeralSystem10 : ILexoNumeralSystem
    {
        public string Name => "Base10";

        public int GetBase()
        {
            return 10;
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
            return '.';
        }

        public int ToDigit(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return ch - 48;
            throw new LexoException("Not valid digit: " + ch);
        }

        public char ToChar(int digit)
        {
            return (char) (digit + 48);
        }
    }
}
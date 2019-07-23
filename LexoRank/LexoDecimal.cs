using System;
using System.Text;
using LexoAlgorithm.NumeralSystems;

namespace LexoAlgorithm
{
    public class LexoDecimal : IComparable<LexoDecimal>, IComparable
    {
        private readonly LexoInteger _mag;
        private readonly int _sig;

        private LexoDecimal(LexoInteger mag, int sig)
        {
            _mag = mag;
            _sig = sig;
        }

        /// <summary>
        ///     Сравнение объектов.
        /// </summary>
        /// <param name="obj">Инстанс.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is LexoDecimal other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(LexoDecimal)}");
        }

        /// <summary>
        ///     Сравнение объектов.
        /// </summary>
        /// <param name="other">Инстанс.</param>
        /// <returns></returns>
        public int CompareTo(LexoDecimal other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var tMag = _mag;
            var oMag = other._mag;
            if (_sig > other._sig)
                oMag = oMag.ShiftLeft(_sig - other._sig);
            else if (_sig < other._sig) tMag = tMag.ShiftLeft(other._sig - _sig);

            return tMag.CompareTo(oMag);
        }

        public static LexoDecimal Half(ILexoNumeralSystem sys)
        {
            var mid = sys.GetBase() / 2;
            return Make(LexoInteger.Make(sys, 1, new[] {mid}), 1);
        }

        public static LexoDecimal Parse(string str, ILexoNumeralSystem system)
        {
            var partialIndex = str.IndexOf(system.GetRadixPointChar());
            if (str.LastIndexOf(system.GetRadixPointChar()) != partialIndex)
                throw new FormatException("More than one " + system.GetRadixPointChar());

            if (partialIndex < 0) return Make(LexoInteger.Parse(str, system), 0);

            var intStr = str.Substring(0, partialIndex) + str.Substring(partialIndex + 1);
            return Make(LexoInteger.Parse(intStr, system), str.Length - 1 - partialIndex);
        }

        public static LexoDecimal From(LexoInteger integer)
        {
            return Make(integer, 0);
        }

        public static LexoDecimal Make(LexoInteger integer, int sig)
        {
            if (integer.IsZero()) return new LexoDecimal(integer, 0);

            var zeroCount = 0;

            for (var i = 0; i < sig && integer.GetMag(i) == 0; ++i) ++zeroCount;

            var newInteger = integer.ShiftRight(zeroCount);
            var newSig = sig - zeroCount;
            return new LexoDecimal(newInteger, newSig);
        }

        public ILexoNumeralSystem GetSystem()
        {
            return _mag.GetSystem();
        }

        public LexoDecimal Add(LexoDecimal other)
        {
            var tMag = _mag;
            var tSig = _sig;
            var oMag = other._mag;

            int oSig;
            for (oSig = other._sig; tSig < oSig; ++tSig) tMag = tMag.ShiftLeft();

            while (tSig > oSig)
            {
                oMag = oMag.ShiftLeft();
                ++oSig;
            }

            return Make(tMag.Add(oMag), tSig);
        }

        public LexoDecimal Subtract(LexoDecimal other)
        {
            var thisMag = _mag;
            var thisSig = _sig;
            var otherMag = other._mag;

            int otherSig;
            for (otherSig = other._sig; thisSig < otherSig; ++thisSig) thisMag = thisMag.ShiftLeft();

            while (thisSig > otherSig)
            {
                otherMag = otherMag.ShiftLeft();
                ++otherSig;
            }

            return Make(thisMag.Subtract(otherMag), thisSig);
        }

        public LexoDecimal Multiply(LexoDecimal other)
        {
            return Make(_mag.Multiply(other._mag), _sig + other._sig);
        }

        public LexoInteger Floor()
        {
            return _mag.ShiftRight(_sig);
        }

        public LexoInteger Ceil()
        {
            if (IsExact()) return _mag;

            var floor = Floor();
            return floor.Add(LexoInteger.One(floor.GetSystem()));
        }

        public bool IsExact()
        {
            if (_sig == 0) return true;

            for (var i = 0; i < _sig; ++i)
                if (_mag.GetMag(i) != 0)
                    return false;

            return true;
        }

        public int GetScale()
        {
            return _sig;
        }

        public LexoDecimal SetScale(int nSig)
        {
            return SetScale(nSig, false);
        }

        public LexoDecimal SetScale(int nSig, bool ceiling)
        {
            if (nSig >= _sig) return this;

            if (nSig < 0) nSig = 0;

            var diff = _sig - nSig;
            var nmag = _mag.ShiftRight(diff);
            if (ceiling) nmag = nmag.Add(LexoInteger.One(nmag.GetSystem()));

            return Make(nmag, nSig);
        }

        /// <summary>
        ///     Конвертировать в строку.
        /// </summary>
        /// <returns></returns>
        public string Format()
        {
            var intStr = _mag.Format();
            if (_sig == 0) return intStr;

            var sb = new StringBuilder(intStr);
            var head = sb[0];
            var specialHead = head == _mag.GetSystem().GetPositiveChar() ||
                              head == _mag.GetSystem().GetNegativeChar();
            if (specialHead) sb.Remove(0, 1);

            while (sb.Length < _sig + 1) sb.Insert(0, _mag.GetSystem().ToChar(0));

            sb.Insert(sb.Length - _sig, _mag.GetSystem().GetRadixPointChar());
            if (sb.Length - _sig == 0) sb.Insert(0, _mag.GetSystem().ToChar(0));

            if (specialHead) sb.Insert(0, head);

            return sb.ToString();
        }

        private bool Equals(LexoDecimal other)
        {
            return Equals(_mag, other._mag) && _sig == other._sig;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LexoDecimal) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_mag != null ? _mag.GetHashCode() : 0) * 397) ^ _sig;
            }
        }

        public override string ToString()
        {
            return Format();
        }
    }
}
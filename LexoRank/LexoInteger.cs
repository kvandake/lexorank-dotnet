using System;
using System.Collections.Generic;
using System.Text;
using LexoAlgorithm.Extensions;
using LexoAlgorithm.NumeralSystems;

namespace LexoAlgorithm
{
    public class LexoInteger : IComparable<LexoInteger>, IComparable
    {
        private const int NegativeSign = -1;
        private const int ZeroSign = 0;
        private const int PositiveSign = 1;
        private static readonly int[] ZeroMag = {0};
        private static readonly int[] OneMag = {1};
        private readonly int[] _mag;
        private readonly int _sign;
        private readonly ILexoNumeralSystem _sys;

        private LexoInteger(ILexoNumeralSystem system, int sign, int[] mag)
        {
            _sys = system;
            _sign = sign;
            _mag = mag;
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is LexoInteger other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(LexoInteger)}");
        }

        public int CompareTo(LexoInteger other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            if (_sign == -1)
            {
                if (other._sign == -1)
                {
                    var cmp = Compare(_mag, other._mag);
                    if (cmp == -1)
                        return 1;
                    return cmp == 1 ? -1 : 0;
                }

                return -1;
            }

            if (_sign == 1) return other._sign == 1 ? Compare(_mag, other._mag) : 1;

            if (other._sign == -1) return 1;

            return other._sign == 1 ? -1 : 0;
        }

        public LexoInteger Add(LexoInteger other)
        {
            CheckSystem(other);
            if (IsZero()) return other;

            if (other.IsZero()) return this;

            if (_sign != other._sign)
            {
                LexoInteger pos;
                if (_sign == -1)
                {
                    pos = Negate();
                    var val = pos.Subtract(other);
                    return val.Negate();
                }

                pos = other.Negate();
                return Subtract(pos);
            }

            var result = Add(_sys, _mag, other._mag);
            return Make(_sys, _sign, result);
        }

        private static int[] Add(ILexoNumeralSystem sys, IReadOnlyList<int> l, IReadOnlyList<int> r)
        {
            var estimatedSize = Math.Max(l.Count, r.Count);
            var result = new int[estimatedSize];
            var carry = 0;

            for (var i = 0; i < estimatedSize; ++i)
            {
                var lNum = i < l.Count ? l[i] : 0;
                var rNum = i < r.Count ? r[i] : 0;
                var sum = lNum + rNum + carry;

                for (carry = 0; sum >= sys.GetBase(); sum -= sys.GetBase()) ++carry;

                result[i] = sum;
            }

            return ExtendWithCarry(result, carry);
        }

        private static int[] ExtendWithCarry(int[] mag, int carry)
        {
            var result = mag;
            if (carry > 0)
            {
                var extendedMag = new int[mag.Length + 1];
                Array.Copy(mag, 0, extendedMag, 0, mag.Length);
                extendedMag[extendedMag.Length - 1] = carry;
                result = extendedMag;
            }

            return result;
        }

        public LexoInteger Subtract(LexoInteger other)
        {
            CheckSystem(other);
            if (IsZero()) return other.Negate();

            if (other.IsZero()) return this;

            if (_sign != other._sign)
            {
                LexoInteger negate;
                if (_sign == -1)
                {
                    negate = Negate();
                    var sum = negate.Add(other);
                    return sum.Negate();
                }

                negate = other.Negate();
                return Add(negate);
            }

            var cmp = Compare(_mag, other._mag);
            if (cmp == 0) return Zero(_sys);

            return cmp < 0
                ? Make(_sys, _sign == -1 ? 1 : -1, Subtract(_sys, other._mag, _mag))
                : Make(_sys, _sign == -1 ? -1 : 1, Subtract(_sys, _mag, other._mag));
        }

        private static int[] Subtract(ILexoNumeralSystem sys, int[] l, int[] r)
        {
            var rComplement = Complement(sys, r, l.Length);
            var rSum = Add(sys, l, rComplement);
            rSum[rSum.Length - 1] = 0;
            return Add(sys, rSum, OneMag);
        }

        public LexoInteger Multiply(LexoInteger other)
        {
            CheckSystem(other);
            if (IsZero()) return this;

            if (other.IsZero()) return other;

            if (IsOneish()) return _sign == other._sign ? Make(_sys, 1, other._mag) : Make(_sys, -1, other._mag);

            if (other.IsOneish()) return _sign == other._sign ? Make(_sys, 1, _mag) : Make(_sys, -1, _mag);

            var newMag = Multiply(_sys, _mag, other._mag);
            return _sign == other._sign ? Make(_sys, 1, newMag) : Make(_sys, -1, newMag);
        }

        private static int[] Multiply(ILexoNumeralSystem sys, int[] l, int[] r)
        {
            var result = new int[l.Length + r.Length];

            for (var li = 0; li < l.Length; ++li)
            for (var ri = 0; ri < r.Length; ++ri)
            {
                var resultIndex = li + ri;

                for (result[resultIndex] += l[li] * r[ri];
                    result[resultIndex] >= sys.GetBase();
                    result[resultIndex] -= sys.GetBase())
                    ++result[resultIndex + 1];
            }

            return result;
        }

        public LexoInteger Negate()
        {
            return IsZero() ? this : Make(_sys, _sign == 1 ? -1 : 1, _mag);
        }

        public LexoInteger ShiftLeft()
        {
            return ShiftLeft(1);
        }

        public LexoInteger ShiftLeft(int times)
        {
            if (times == 0) return this;

            if (times < 0) return ShiftRight(Math.Abs(times));

            var nmag = new int[_mag.Length + times];
            Array.Copy(_mag, 0, nmag, times, _mag.Length);
            return Make(_sys, _sign, nmag);
        }

        public LexoInteger ShiftRight()
        {
            return ShiftRight(1);
        }

        public LexoInteger ShiftRight(int times)
        {
            if (_mag.Length - times <= 0) return Zero(_sys);

            var nmag = new int[_mag.Length - times];
            Array.Copy(_mag, times, nmag, 0, nmag.Length);
            return Make(_sys, _sign, nmag);
        }

        public LexoInteger Complement()
        {
            return Complement(_mag.Length);
        }

        private LexoInteger Complement(int digits)
        {
            return Make(_sys, _sign, Complement(_sys, _mag, digits));
        }

        private static int[] Complement(ILexoNumeralSystem sys, int[] mag, int digits)
        {
            if (digits <= 0) throw new LexoException("Expected at least 1 digit");

            var nmag = new int[digits];

            nmag.Fill(sys.GetBase() - 1);

            for (var i = 0; i < mag.Length; ++i) nmag[i] = sys.GetBase() - 1 - mag[i];

            return nmag;
        }

        public bool IsZero()
        {
            return _sign == 0 && _mag.Length == 1 && _mag[0] == 0;
        }

        private bool IsOneish()
        {
            return _mag.Length == 1 && _mag[0] == 1;
        }

        public bool IsOne()
        {
            return _sign == 1 && _mag.Length == 1 && _mag[0] == 1;
        }

        public int GetMag(int index)
        {
            return _mag[index];
        }


        private static int Compare(IReadOnlyList<int> l, IReadOnlyList<int> r)
        {
            if (l.Count < r.Count) return -1;

            if (l.Count > r.Count) return 1;

            for (var i = l.Count - 1; i >= 0; --i)
            {
                if (l[i] < r[i]) return -1;

                if (l[i] > r[i]) return 1;
            }

            return 0;
        }

        public ILexoNumeralSystem GetSystem()
        {
            return _sys;
        }

        private void CheckSystem(LexoInteger other)
        {
            if (!_sys.Name.Equals(other._sys.Name)) throw new LexoException("Expected numbers of same numeral sys");
        }

        public string Format()
        {
            if (IsZero()) return Convert.ToString(_sys.ToChar(0));

            var sb = new StringBuilder();
            var var2 = _mag;
            var var3 = var2.Length;

            for (var var4 = 0; var4 < var3; ++var4)
            {
                var digit = var2[var4];
                sb.Insert(0, _sys.ToChar(digit));
            }

            if (_sign == -1) sb.Insert(0, _sys.GetNegativeChar());

            return sb.ToString();
        }

        public static LexoInteger Parse(string strFull, ILexoNumeralSystem system)
        {
            var str = strFull;
            var sign = 1;
            if (strFull.IndexOf(system.GetPositiveChar()) == 0)
            {
                str = strFull.Substring(1);
            }
            else if (strFull.IndexOf(system.GetNegativeChar()) == 0)
            {
                str = strFull.Substring(1);
                sign = -1;
            }

            var mag = new int[str.Length];
            var strIndex = mag.Length - 1;

            for (var magIndex = 0; strIndex >= 0; ++magIndex)
            {
                mag[magIndex] = system.ToDigit(str[strIndex]);
                --strIndex;
            }

            return Make(system, sign, mag);
        }

        internal static LexoInteger Zero(ILexoNumeralSystem sys)
        {
            return new LexoInteger(sys, 0, ZeroMag);
        }

        internal static LexoInteger One(ILexoNumeralSystem sys)
        {
            return Make(sys, 1, OneMag);
        }

        public static LexoInteger Make(ILexoNumeralSystem sys, int sign, int[] mag)
        {
            int actualLength;
            for (actualLength = mag.Length; actualLength > 0 && mag[actualLength - 1] == 0; --actualLength)
            {
            }

            if (actualLength == 0) return Zero(sys);

            if (actualLength == mag.Length) return new LexoInteger(sys, sign, mag);

            var nmag = new int[actualLength];
            Array.Copy(mag, 0, nmag, 0, actualLength);
            return new LexoInteger(sys, sign, nmag);
        }

        private bool Equals(LexoInteger other)
        {
            return _sys == other._sys && CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LexoInteger) obj);
        }

        public static bool operator ==(LexoInteger left, LexoInteger right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LexoInteger left, LexoInteger right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _mag != null ? _mag.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ _sign;
                hashCode = (hashCode * 397) ^ (_sys != null ? _sys.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Format();
        }
    }
}
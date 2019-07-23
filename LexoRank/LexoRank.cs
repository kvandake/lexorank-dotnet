using System;
using System.Runtime.CompilerServices;
using System.Text;
using LexoAlgorithm.NumeralSystems;

[assembly: InternalsVisibleTo("LexoRank.Tests")]

namespace LexoAlgorithm
{
    public class LexoRank : IComparable<LexoRank>, IComparable
    {
        public static readonly ILexoNumeralSystem NumeralSystem = new LexoNumeralSystem36();
        private static readonly LexoDecimal ZeroDecimal = LexoDecimal.Parse("0", NumeralSystem);
        private static readonly LexoDecimal OneDecimal = LexoDecimal.Parse("1", NumeralSystem);
        private static readonly LexoDecimal EightDecimal = LexoDecimal.Parse("8", NumeralSystem);
        private static readonly LexoDecimal MinDecimal = ZeroDecimal;

        private static readonly LexoDecimal MaxDecimal =
            LexoDecimal.Parse("1000000", NumeralSystem).Subtract(OneDecimal);

        private static readonly LexoDecimal MidDecimal = Between(MinDecimal, MaxDecimal);
        private static readonly LexoDecimal InitialMinDecimal = LexoDecimal.Parse("100000", NumeralSystem);

        private static readonly LexoDecimal InitialMaxDecimal =
            LexoDecimal.Parse(Convert.ToString(NumeralSystem.ToChar(NumeralSystem.GetBase() - 2)) + "00000",
                NumeralSystem);

        private readonly string _value;

        private LexoRank(string value)
        {
            _value = value;
            var parts = _value.Split('|');
            Bucket = LexoRankBucket.From(parts[0]);
            Decimal = LexoDecimal.Parse(parts[1], NumeralSystem);
        }

        private LexoRank(LexoRankBucket bucket, LexoDecimal dec)
        {
            _value = bucket.Format() + "|" + FormatDecimal(dec);
            Bucket = bucket;
            Decimal = dec;
        }

        public LexoRankBucket Bucket { get; }

        public LexoDecimal Decimal { get; }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is LexoRank other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(LexoRank)}");
        }

        public int CompareTo(LexoRank other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(_value, other._value, StringComparison.Ordinal);
        }

        public static LexoRank Min()
        {
            return From(LexoRankBucket.Bucket0, MinDecimal);
        }

        public static LexoRank Max()
        {
            return Max(LexoRankBucket.Bucket0);
        }

        public static LexoRank Middle()
        {
            var minLexoRank = Min();
            return minLexoRank.Between(Max(minLexoRank.Bucket));
        }

        public static LexoRank Max(LexoRankBucket bucket)
        {
            return From(bucket, MaxDecimal);
        }

        public LexoRank GenPrev()
        {
            if (IsMax()) return new LexoRank(Bucket, InitialMaxDecimal);

            var floorInteger = Decimal.Floor();
            var floorDecimal = LexoDecimal.From(floorInteger);
            var nextDecimal = floorDecimal.Subtract(EightDecimal);
            if (nextDecimal.CompareTo(MinDecimal) <= 0) nextDecimal = Between(MinDecimal, Decimal);

            return new LexoRank(Bucket, nextDecimal);
        }

        public LexoRank InNextBucket()
        {
            return From(Bucket.Next(), Decimal);
        }

        public LexoRank InPrevBucket()
        {
            return From(Bucket.Prev(), Decimal);
        }

        public bool IsMin()
        {
            return Decimal.Equals(MinDecimal);
        }

        public bool IsMax()
        {
            return Decimal.Equals(MaxDecimal);
        }

        public string Format()
        {
            return _value;
        }

        public LexoRank GenNext()
        {
            if (IsMin()) return new LexoRank(Bucket, InitialMinDecimal);

            var ceilInteger = Decimal.Ceil();
            var ceilDecimal = LexoDecimal.From(ceilInteger);
            var nextDecimal = ceilDecimal.Add(EightDecimal);
            if (nextDecimal.CompareTo(MaxDecimal) >= 0) nextDecimal = Between(Decimal, MaxDecimal);

            return new LexoRank(Bucket, nextDecimal);
        }

        public LexoRank Between(LexoRank other)
        {
            if (!Bucket.Equals(other.Bucket)) throw new LexoException("Between works only within the same bucket");

            var cmp = Decimal.CompareTo(other.Decimal);
            if (cmp > 0)
                return new LexoRank(Bucket, Between(other.Decimal, Decimal));
            if (cmp == 0)
                throw new LexoException("Try to rank between issues with same rank this=" + this +
                                        " other=" + other + " this.decimal=" + Decimal +
                                        " other.decimal=" + other.Decimal);
            return new LexoRank(Bucket, Between(Decimal, other.Decimal));
        }

        private bool Equals(LexoRank other)
        {
            return string.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LexoRank) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _value != null ? _value.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Bucket != null ? Bucket.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Decimal != null ? Decimal.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return _value;
        }

        public static LexoRank Initial(LexoRankBucket bucket)
        {
            return bucket == LexoRankBucket.Bucket0
                ? From(bucket, InitialMinDecimal)
                : From(bucket, InitialMaxDecimal);
        }

        private static LexoDecimal Between(LexoDecimal oLeft, LexoDecimal oRight)
        {
            if (oLeft.GetSystem() != oRight.GetSystem()) throw new LexoException("Expected same system");

            var left = oLeft;
            var right = oRight;
            LexoDecimal nLeft;
            if (oLeft.GetScale() < oRight.GetScale())
            {
                nLeft = oRight.SetScale(oLeft.GetScale(), false);
                if (oLeft.CompareTo(nLeft) >= 0) return Middle(oLeft, oRight);

                right = nLeft;
            }

            if (oLeft.GetScale() > right.GetScale())
            {
                nLeft = oLeft.SetScale(right.GetScale(), true);
                if (nLeft.CompareTo(right) >= 0) return Middle(oLeft, oRight);

                left = nLeft;
            }

            LexoDecimal nRight;
            for (var scale = left.GetScale(); scale > 0; right = nRight)
            {
                var nScale1 = scale - 1;
                var nLeft1 = left.SetScale(nScale1, true);
                nRight = right.SetScale(nScale1, false);
                var cmp = nLeft1.CompareTo(nRight);
                if (cmp == 0) return CheckMid(oLeft, oRight, nLeft1);

                if (nLeft1.CompareTo(nRight) > 0) break;

                scale = nScale1;
                left = nLeft1;
            }

            var mid = Middle(oLeft, oRight, left, right);

            int nScale;
            for (var mScale = mid.GetScale(); mScale > 0; mScale = nScale)
            {
                nScale = mScale - 1;
                var nMid = mid.SetScale(nScale);
                if (oLeft.CompareTo(nMid) >= 0 || nMid.CompareTo(oRight) >= 0) break;

                mid = nMid;
            }

            return mid;
        }

        private static LexoDecimal Middle(LexoDecimal lBound, LexoDecimal rBound, LexoDecimal left, LexoDecimal right)
        {
            var mid = Middle(left, right);
            return CheckMid(lBound, rBound, mid);
        }

        private static LexoDecimal CheckMid(LexoDecimal lBound, LexoDecimal rBound, LexoDecimal mid)
        {
            if (lBound.CompareTo(mid) >= 0) return Middle(lBound, rBound);

            return mid.CompareTo(rBound) >= 0 ? Middle(lBound, rBound) : mid;
        }

        private static LexoDecimal Middle(LexoDecimal left, LexoDecimal right)
        {
            var sum = left.Add(right);
            var mid = sum.Multiply(LexoDecimal.Half(left.GetSystem()));
            var scale = left.GetScale() > right.GetScale() ? left.GetScale() : right.GetScale();
            if (mid.GetScale() > scale)
            {
                var roundDown = mid.SetScale(scale, false);
                if (roundDown.CompareTo(left) > 0) return roundDown;

                var roundUp = mid.SetScale(scale, true);
                if (roundUp.CompareTo(right) < 0) return roundUp;
            }

            return mid;
        }

        private static string FormatDecimal(LexoDecimal dec)
        {
            var formatVal = dec.Format();
            var val = new StringBuilder(formatVal);
            var partialIndex = formatVal.IndexOf(NumeralSystem.GetRadixPointChar());
            var zero = NumeralSystem.ToChar(0);
            if (partialIndex < 0)
            {
                partialIndex = formatVal.Length;
                val.Append(NumeralSystem.GetRadixPointChar());
            }

            while (partialIndex < 6)
            {
                val.Insert(0, zero);
                ++partialIndex;
            }

            while (val[val.Length - 1] == zero) val.Length = val.Length - 1;

            return val.ToString();
        }

        public static LexoRank Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentException(nameof(str));
            return new LexoRank(str);
        }

        public static LexoRank From(LexoRankBucket bucket, LexoDecimal dec)
        {
            if (!dec.GetSystem().Name.Equals(NumeralSystem.Name)) throw new LexoException("Expected different system");

            return new LexoRank(bucket, dec);
        }
    }
}
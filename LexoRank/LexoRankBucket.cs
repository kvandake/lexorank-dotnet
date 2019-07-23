namespace LexoAlgorithm
{
    public class LexoRankBucket
    {
        internal static readonly LexoRankBucket Bucket0 = new LexoRankBucket("0");
        internal static readonly LexoRankBucket Bucket1 = new LexoRankBucket("1");
        internal static readonly LexoRankBucket Bucket2 = new LexoRankBucket("2");

        private static readonly LexoRankBucket[] Values = {Bucket0, Bucket1, Bucket2};

        private readonly LexoInteger _value;

        private LexoRankBucket(string val)
        {
            _value = LexoInteger.Parse(val, LexoRank.NumeralSystem);
        }

        public static LexoRankBucket Resolve(int bucketId)
        {
            var var1 = Values;
            var var2 = var1.Length;

            for (var var3 = 0; var3 < var2; ++var3)
            {
                var bucket = var1[var3];
                if (bucket.Equals(From(bucketId.ToString()))) return bucket;
            }

            throw new LexoException("No bucket found with id " + bucketId);
        }

        public string Format()
        {
            return _value?.Format();
        }

        public LexoRankBucket Next()
        {
            if (this == Bucket0) return Bucket1;

            if (this == Bucket1) return Bucket2;

            return this == Bucket2 ? Bucket0 : Bucket2;
        }

        public bool Equals(LexoRankBucket other)
        {
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LexoRankBucket other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(LexoRankBucket left, LexoRankBucket right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LexoRankBucket left, LexoRankBucket right)
        {
            return !Equals(left, right);
        }

        public LexoRankBucket Prev()
        {
            if (this == Bucket0) return Bucket2;

            if (this == Bucket1) return Bucket0;

            return this == Bucket2 ? Bucket1 : Bucket0;
        }

        public static LexoRankBucket From(string str)
        {
            var val = LexoInteger.Parse(str, LexoRank.NumeralSystem);
            var var2 = Values;
            var var3 = var2.Length;

            for (var var4 = 0; var4 < var3; ++var4)
            {
                var bucket = var2[var4];
                if (bucket._value.Equals(val)) return bucket;
            }

            throw new LexoException("Unknown bucket: " + str);
        }

        public static LexoRankBucket Min()
        {
            return Values[0];
        }

        public static LexoRankBucket Max()
        {
            return Values[Values.Length - 1];
        }
    }
}
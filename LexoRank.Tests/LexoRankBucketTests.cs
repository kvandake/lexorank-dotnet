using Shouldly;
using Xunit;

namespace LexoAlgorithm.Tests
{
    public class LexoRankBucketTests
    {
        [Fact]
        public void Should_Equals_From_String()
        {
            var bucket0 = LexoRankBucket.Resolve(1);
            var bucket1 = LexoRankBucket.Resolve(1);
            bucket0.Equals(bucket1).ShouldBeTrue();
        }

        [Fact]
        public void Should_NonEquals_From_String()
        {
            var bucket0 = LexoRankBucket.Resolve(0);
            var bucket1 = LexoRankBucket.Resolve(1);
            bucket0.Equals(bucket1).ShouldBeFalse();
        }

        [Fact]
        public void Should_Resolve_Buckets()
        {
            var bucket0 = LexoRankBucket.Resolve(0);
            var bucket1 = LexoRankBucket.Resolve(1);
            var bucket2 = LexoRankBucket.Resolve(2);

            bucket0.Format().ShouldBe("0");
            bucket1.Format().ShouldBe("1");
            bucket2.Format().ShouldBe("2");
            Should.Throw<LexoException>(() => { LexoRankBucket.Resolve(-1); });
            Should.Throw<LexoException>(() => { LexoRankBucket.Resolve(3); });
        }
    }
}
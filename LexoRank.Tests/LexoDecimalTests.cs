using Shouldly;
using Xunit;

namespace LexoAlgorithm.Tests
{
    public class LexoDecimalTests
    {
        [Fact]
        public void Should_CompareTo_Equals()
        {
            var dec1 = LexoDecimal.From(LexoInteger.Parse("12", LexoRank.NumeralSystem));
            var dec2 = LexoDecimal.From(LexoInteger.Parse("12", LexoRank.NumeralSystem));
            dec1.CompareTo(dec2).ShouldBe(0);
        }

        [Fact]
        public void Should_CompareTo_Greater()
        {
            var dec1 = LexoDecimal.From(LexoInteger.Parse("0", LexoRank.NumeralSystem));
            var dec2 = LexoDecimal.From(LexoInteger.Parse("1", LexoRank.NumeralSystem));
            dec2.CompareTo(dec1).ShouldBe(1);
        }

        [Fact]
        public void Should_CompareTo_Less()
        {
            var dec1 = LexoDecimal.From(LexoInteger.Parse("0", LexoRank.NumeralSystem));
            var dec2 = LexoDecimal.From(LexoInteger.Parse("1", LexoRank.NumeralSystem));
            dec1.CompareTo(dec2).ShouldBe(-1);
        }

        [Fact]
        public void Should_Equals_Format_From_Parse()
        {
            var dec1 = LexoDecimal.From(LexoInteger.Parse("12", LexoRank.NumeralSystem));
            dec1.Format().ShouldBe("12");
        }

        [Fact]
        public void Should_Equals_From_String()
        {
            var dec1 = LexoDecimal.From(LexoInteger.Parse("12", LexoRank.NumeralSystem));
            var dec2 = LexoDecimal.From(LexoInteger.Parse("12", LexoRank.NumeralSystem));
            dec1.Equals(dec2).ShouldBeTrue();
        }

        [Fact]
        public void Should_NonEquals_From_String()
        {
            var dec1 = LexoDecimal.From(LexoInteger.Parse("12", LexoRank.NumeralSystem));
            var dec2 = LexoDecimal.From(LexoInteger.Parse("120", LexoRank.NumeralSystem));
            dec1.Equals(dec2).ShouldBeFalse();
        }
    }
}
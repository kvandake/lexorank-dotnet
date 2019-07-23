using Shouldly;
using Xunit;

namespace LexoAlgorithm.Tests
{
    public class LexoIntegerTests
    {
        [Fact]
        public void Should_CompareTo_Equals()
        {
            var int1 = LexoInteger.Parse("12", LexoRank.NumeralSystem);
            var int2 = LexoInteger.Parse("12", LexoRank.NumeralSystem);
            int1.CompareTo(int2).ShouldBe(0);
        }

        [Fact]
        public void Should_CompareTo_Greater()
        {
            var int1 = LexoInteger.Parse("0", LexoRank.NumeralSystem);
            var int2 = LexoInteger.Parse("1", LexoRank.NumeralSystem);
            int2.CompareTo(int1).ShouldBe(1);
        }

        [Fact]
        public void Should_CompareTo_Less()
        {
            var int1 = LexoInteger.Parse("0", LexoRank.NumeralSystem);
            var int2 = LexoInteger.Parse("1", LexoRank.NumeralSystem);
            int1.CompareTo(int2).ShouldBe(-1);
        }

        [Fact]
        public void Should_Equals_Format_From_Parse()
        {
            var int1 = LexoInteger.Parse("12", LexoRank.NumeralSystem);
            int1.Format().ShouldBe("12");
        }

        [Fact]
        public void Should_Equals_From_String()
        {
            var int1 = LexoInteger.Parse("12", LexoRank.NumeralSystem);
            var int2 = LexoInteger.Parse("12", LexoRank.NumeralSystem);
            int1.Equals(int2).ShouldBeTrue();
        }

        [Fact]
        public void Should_NonEquals_From_String()
        {
            var int1 = LexoInteger.Parse("12", LexoRank.NumeralSystem);
            var int2 = LexoInteger.Parse("120", LexoRank.NumeralSystem);
            int1.Equals(int2).ShouldBeFalse();
        }
    }
}
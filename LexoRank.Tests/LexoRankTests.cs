using System.Collections.Generic;
using System.Linq;
using LexoAlgorithm.NumeralSystems;
using LexoAlgorithm.Tests.Extensions;
using Shouldly;
using Xunit;

namespace LexoAlgorithm.Tests
{
    public class LexoRankTests
    {
        [Theory]
        [InlineData(0, 1, "0|0i0000:")]
        [InlineData(1, 0, "0|0i0000:")]
        [InlineData(3, 5, "0|10000o:")]
        [InlineData(5, 3, "0|10000o:")]
        [InlineData(15, 30, "0|10004s:")]
        [InlineData(31, 32, "0|10006s:")]
        [InlineData(100, 200, "0|1000x4:")]
        [InlineData(200, 100, "0|1000x4:")]
        public void Should_Between_MoveTo(int prevStep, int nextStep, string expected)
        {
            // Arrange
            var prevRank = LexoRank.Min();
            for (var i = 0; i < prevStep; i++) prevRank = prevRank.GenNext();

            var nextRank = LexoRank.Min();
            for (var i = 0; i < nextStep; i++) nextRank = nextRank.GenNext();

            // Act
            var between = prevRank.Between(nextRank);

            // Assert
            between.ToString().ShouldBe(expected);
        }


        [Fact]
        public void Should_Between()
        {
            var lexorank = LexoRank.Min();
            var lexorank1 = lexorank.GenNext();
            var between = lexorank.Between(lexorank1);
            lexorank.CompareTo(between).ShouldBeLessThan(0);
            lexorank1.CompareTo(between).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Between_MaxGenPrev()
        {
            var maxRank = LexoRank.Max();
            var prevRank = maxRank.GenPrev();
            var between = maxRank.Between(prevRank);
            between.ToString().ShouldBe("0|yzzzzz:");
            maxRank.CompareTo(between).ShouldBeGreaterThan(0);
            prevRank.CompareTo(between).ShouldBeLessThan(0);
        }

        [Fact]
        public void Should_Between_Middle()
        {
            // Arrange
            var minRank = LexoRank.Min();
            var maxRank = LexoRank.Max();
            // Act
            var middleRank = LexoRank.Middle();
            var prevMiddleRank = middleRank.GenPrev();
            var nextMiddleRank = middleRank.GenNext();
            // Assert
            middleRank.ToString().ShouldBe("0|hzzzzz:");
            minRank.CompareTo(middleRank).ShouldBeLessThan(0);
            maxRank.CompareTo(middleRank).ShouldBeGreaterThan(0);
            prevMiddleRank.CompareTo(middleRank).ShouldBeLessThan(0);
            nextMiddleRank.CompareTo(middleRank).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Between_MinGenNext()
        {
            var minRank = LexoRank.Min();
            var nextRank = minRank.GenNext();
            var between = minRank.Between(nextRank);
            between.ToString().ShouldBe("0|0i0000:");
            minRank.CompareTo(between).ShouldBeLessThan(0);
            nextRank.CompareTo(between).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Between_MinMax()
        {
            var minRank = LexoRank.Min();
            var maxRank = LexoRank.Max();
            var between = minRank.Between(maxRank);
            between.ToString().ShouldBe("0|hzzzzz:");
            minRank.CompareTo(between).ShouldBeLessThan(0);
            maxRank.CompareTo(between).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Between_Twice()
        {
            var lexorank = LexoRank.Min();
            var lexorank1 = lexorank.GenNext();
            var between = lexorank.Between(lexorank1);
            var between1 = between.Between(lexorank1);
            between1.CompareTo(between).ShouldBeGreaterThan(0);
            lexorank1.CompareTo(between1).ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Correct_OrderBy_Value()
        {
            var cursorLexoRank = LexoRank.Min();
            var items = new List<LexoRank> {cursorLexoRank};
            for (var i = 0; i < 100000; i++)
            {
                cursorLexoRank = cursorLexoRank.GenNext();
                items.Add(cursorLexoRank);
            }

            var originItems = items.Select(x => x.Format()).ToList();
            var sortItems = originItems.ToList();
            sortItems.Shuffle();
            sortItems = sortItems.OrderBy(x => x).ToList();

            originItems.SequenceEqual(sortItems).ShouldBeTrue();
        }

        [Fact]
        public void Should_Create_Million_Instances()
        {
            var cursorLexoRank = LexoRank.Min();
            var items = new List<LexoRank> {cursorLexoRank};
            for (var i = 0; i < 1000000; i++)
            {
                cursorLexoRank = cursorLexoRank.GenNext();
                items.Add(cursorLexoRank);
            }

            items.Count.ShouldBe(1000001);
        }

        [Fact]
        public void Should_Different_NumeralSystem_By_From()
        {
            Should.Throw<LexoException>(() =>
            {
                LexoRank.From(LexoRankBucket.Min(), LexoDecimal.Parse("1", new LexoNumeralSystem10()));
            });
        }

        [Fact]
        public void Should_Equals_Format_From_Parse()
        {
            var lexorank = LexoRank.Parse("1|12345");
            lexorank.Format().ShouldBe("1|12345");
        }

        [Fact]
        public void Should_GenNext()
        {
            var lexorank = LexoRank.Min();
            var nextLexorank = lexorank.GenNext();
            lexorank.CompareTo(nextLexorank).ShouldBe(-1);
        }

        [Fact]
        public void Should_GenPrev()
        {
            var lexorank = LexoRank.Max();
            var prevLexorank = lexorank.GenPrev();
            lexorank.CompareTo(prevLexorank).ShouldBe(1);
        }

        [Fact]
        public void Should_InNextBucket()
        {
            var lexorank = LexoRank.Min();
            var lexorank1 = lexorank.InNextBucket();
            lexorank.Format().Contains("0|").ShouldBeTrue();
            lexorank1.Format().Contains("1|").ShouldBeTrue();
        }

        [Fact]
        public void Should_InPrevBucket()
        {
            var lexorank = LexoRank.Min();
            var lexorank1 = lexorank.InPrevBucket();
            lexorank.Format().Contains("0|").ShouldBeTrue();
            lexorank1.Format().Contains("2|").ShouldBeTrue();
        }

        [Fact]
        public void Should_Instance_By_From()
        {
            var lexorank = LexoRank.From(LexoRankBucket.Min(), LexoDecimal.Parse("1", LexoRank.NumeralSystem));
            lexorank.Bucket.Format().ShouldBe("0");
            lexorank.Decimal.Format().ShouldBe("1");
        }

        [Fact]
        public void Should_IsMax()
        {
            var lexorank = LexoRank.Max();
            lexorank.IsMax().ShouldBeTrue();
        }

        [Fact]
        public void Should_IsMin()
        {
            var lexorank = LexoRank.Min();
            lexorank.IsMin().ShouldBeTrue();
        }
    }
}
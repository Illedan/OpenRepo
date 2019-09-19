using System;
using FluentAssertions;
using OpenRepo.Util;
using Xunit;

namespace OpenRepo.Tests.Util
{
    public class IndexTraverserTests
    {
        [Fact]
        public void MoveNext_Incremented()
        {
            var cut = new IndexTraverser(0, 2);

            cut.MoveNext();

            cut.Current.Should().Be(1, "Because it is increased");
        }

        [Fact]
        public void MoveNext_TooHigh_GoesAround()
        {
            var cut = new IndexTraverser(0, 2);

            cut.MoveNext();
            cut.MoveNext();

            cut.Current.Should().Be(0, "Because it reached max");
        }

        [Fact]
        public void MovePrevious_Decreased()
        {
            var cut = new IndexTraverser(1, 2);

            cut.MovePrevious();

            cut.Current.Should().Be(0, "Because it is decreased");
        }

        [Fact]
        public void MovePrevious_TooLow_GoesAround()
        {
            var cut = new IndexTraverser(1, 2);

            cut.MovePrevious();
            cut.MovePrevious();

            cut.Current.Should().Be(1, "Because it is decreased");
        }
    }
}

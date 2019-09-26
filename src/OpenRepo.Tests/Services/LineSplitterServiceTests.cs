using System;
using OpenRepo.Services;
using Xunit;

namespace OpenRepo.Tests.Services
{
    public class LineSplitterServiceTests
    {
        [Fact]
        public void OnlySplaces_NormalSplitIsFine()
        {
            var line = "abc dce ok";

            var splitted = line.SplitPath();

            Assert.Equal("abc", splitted[0]);
            Assert.Equal("dce", splitted[1]);
            Assert.Equal("ok", splitted[2]);
            Assert.Equal(3, splitted.Length);
        }

        [Fact]
        public void EmptyLine_EmptyOutput()
        {
            var line = string.Empty;

            var splitted = line.SplitPath();

            Assert.Empty(splitted);
        }

        [Fact]
        public void OneItem_NormalSplit()
        {
            var line = "abc";

            var splitted = line.SplitPath();

            Assert.Equal("abc", splitted[0]);
            Assert.Single(splitted);
        }

        [Fact]
        public void WithQuotes_SplitWithQuotes()
        {
            var line = "\"abc dce\" ok";

            var splitted = line.SplitPath();

            Assert.Equal("abc dce", splitted[0]);
            Assert.Equal("ok", splitted[1]);
            Assert.Equal(2, splitted.Length);
        }

        [Fact]
        public void WithInvalidNumberOfQuotes_Throws()
        {
            var line = "\"abc dce ok";

            Assert.Throws<Exception>(() => line.SplitPath());
        }

        [Fact]
        public void DoubleQuotes_SplitWithQuotes()
        {
            var line = "\"abc dce\" ok \"c:\\my path with spaces\\\"";

            var splitted = line.SplitPath();

            Assert.Equal("abc dce", splitted[0]);
            Assert.Equal("ok", splitted[1]);
            Assert.Equal("c:\\my path with spaces\\", splitted[2]);
            Assert.Equal(3, splitted.Length);
        }
    }
}

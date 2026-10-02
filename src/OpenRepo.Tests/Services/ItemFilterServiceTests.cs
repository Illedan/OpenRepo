using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OpenRepo.Contracts;
using OpenRepo.Services;
using Xunit;

namespace OpenRepo.Tests.Services
{
    public class ItemFilterServiceTests
    {
        private readonly List<SelectableItem> m_items = new[] { "OpenRepo", "OpenAI-api", "work/OpenRepo-fork", "BirthBoi" }
            .Select(t => new SelectableItem(t, () => new SelectableAction[0]))
            .ToList();

        [Fact]
        public void EmptyText_KeepsAll()
        {
            ItemFilterService.Filter(m_items, string.Empty).Should().HaveCount(4);
        }

        [Fact]
        public void IgnoresCase()
        {
            var titles = ItemFilterService.Filter(m_items, "birth").Select(i => i.Title);

            titles.Should().Equal("BirthBoi");
        }

        [Fact]
        public void SeveralWords_AllMustMatch()
        {
            var titles = ItemFilterService.Filter(m_items, "repo  work").Select(i => i.Title);

            titles.Should().Equal("work/OpenRepo-fork");
        }
    }
}

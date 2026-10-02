using System.Linq;
using FluentAssertions;
using OpenRepo.Contracts;
using OpenRepo.Services;
using Xunit;

namespace OpenRepo.Tests.Services
{
    public class ActionShortcutServiceTests
    {
        [Fact]
        public void TakenFirstLetter_UsesNextFreeLetter()
        {
            var actions = new[] { "Open", "Old", "Web" }.Select(t => new SelectableAction(t, () => { })).ToArray();

            var shortcuts = ActionShortcutService.GetShortcuts(actions);

            shortcuts.Keys.Should().Equal('o', 'l', 'w');
        }

        [Fact]
        public void NoFreeLetter_UsesPosition()
        {
            var actions = new[] { "ab", "ab", "ab" }.Select(t => new SelectableAction(t, () => { })).ToArray();

            var shortcuts = ActionShortcutService.GetShortcuts(actions);

            shortcuts.Keys.Should().Equal('a', 'b', '3');
        }
    }
}

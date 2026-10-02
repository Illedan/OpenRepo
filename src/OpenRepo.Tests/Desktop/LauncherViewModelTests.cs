using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using OpenRepo.Contracts;
using OpenRepo.Desktop;
using OpenRepo.Services;
using Xunit;

namespace OpenRepo.Tests.Desktop
{
    public class LauncherViewModelTests
    {
        private readonly List<string> m_ran = new List<string>();
        private readonly List<SelectableItem> m_items;
        private int m_hideRequests;

        public LauncherViewModelTests()
        {
            LogService.Clear();
            m_items = new List<SelectableItem>
            {
                new SelectableItem("BirthBoi", () => new[] { Action("BirthBoi Open") }),
                new SelectableItem("OpenAI-api", () => new[] { Action("Open"), Action("Web"), Action("sln") }),
                new SelectableItem("OpenRepo", () => new[] { Action("Open"), Action("Web") }),
            };
        }

        private SelectableAction Action(string title) => new SelectableAction(title, () => m_ran.Add(title));

        private async Task<LauncherViewModel> CreateLoaded()
        {
            var cut = new LauncherViewModel(() => Task.FromResult(m_items));
            cut.HideRequested += () => m_hideRequests++;
            await cut.Reload();
            return cut;
        }

        [Fact]
        public async Task Reload_ShowsItems()
        {
            var cut = await CreateLoaded();

            cut.Rows.Select(r => r.Title).Should().Equal("BirthBoi", "OpenAI-api", "OpenRepo");
            cut.SelectedIndex.Should().Be(0);
        }

        [Fact]
        public async Task SearchText_KeepsSelectedItemWhenStillShown()
        {
            var cut = await CreateLoaded();
            cut.SelectedIndex = 2;

            cut.SearchText = "open";

            cut.Rows.Select(r => r.Title).Should().Equal("OpenAI-api", "OpenRepo");
            cut.SelectedIndex.Should().Be(1, "OpenRepo was selected before filtering");
        }

        [Fact]
        public async Task MoveSelection_GoesAround()
        {
            var cut = await CreateLoaded();

            cut.MoveSelection(-1);

            cut.SelectedIndex.Should().Be(2);
        }

        [Fact]
        public async Task Activate_OneAction_RunsItAndHides()
        {
            var cut = await CreateLoaded();

            cut.Activate();

            m_ran.Should().Equal("BirthBoi Open");
            m_hideRequests.Should().Be(1);
        }

        [Fact]
        public async Task Activate_SeveralActions_ShowsThemWithShortcuts()
        {
            var cut = await CreateLoaded();
            cut.SelectedIndex = 1;

            cut.Activate();

            cut.IsChoosingAction.Should().BeTrue();
            cut.ActionTitle.Should().Be("OpenAI-api");
            cut.Rows.Select(r => r.Title + ":" + r.Shortcut).Should().Equal("Open:O", "Web:W", "sln:S");
            m_ran.Should().BeEmpty();
        }

        [Fact]
        public async Task Shortcut_RunsActionAndResets()
        {
            var cut = await CreateLoaded();
            cut.SearchText = "openai";
            cut.Activate();

            cut.TryRunShortcut('W').Should().BeTrue();

            m_ran.Should().Equal("Web");
            m_hideRequests.Should().Be(1);
            cut.IsChoosingAction.Should().BeFalse();
            cut.SearchText.Should().BeEmpty();
        }

        [Fact]
        public async Task Back_FromActions_SelectsTheOpenedItem()
        {
            var cut = await CreateLoaded();
            cut.SelectedIndex = 2;
            cut.Activate();

            cut.Back();

            cut.IsChoosingAction.Should().BeFalse();
            cut.Rows.Should().HaveCount(3);
            cut.SelectedIndex.Should().Be(2);
            m_hideRequests.Should().Be(0);
        }

        [Fact]
        public async Task Back_ClearsSearchBeforeHiding()
        {
            var cut = await CreateLoaded();
            cut.SearchText = "repo";

            cut.Back();
            cut.SearchText.Should().BeEmpty();
            m_hideRequests.Should().Be(0);

            cut.Back();
            m_hideRequests.Should().Be(1);
        }

        [Fact]
        public async Task FailingAction_StaysOpenWithMessage()
        {
            m_items.Add(new SelectableItem("Zebra", () => new[] { new SelectableAction("Open", () => LogService.Log("No type of sln found")) }));
            var cut = await CreateLoaded();
            cut.SearchText = "zeb";

            cut.Activate();

            cut.Message.Should().Be("No type of sln found");
            cut.SearchText.Should().BeEmpty();
            cut.SelectedIndex.Should().Be(3, "the failed item stays selected");
            m_hideRequests.Should().Be(0);
        }

        [Fact]
        public async Task ActionShowingMoreActions_StaysOnThem()
        {
            LauncherViewModel cut = null;
            var solutions = new SelectableItem("OpenRepo/*.sln", () => new[] { Action("A.sln"), Action("B.sln") });
            m_items.Add(new SelectableItem("Zebra", () => new[] { new SelectableAction("sln", () => cut.ShowActions(solutions)) }));
            cut = await CreateLoaded();
            cut.SelectedIndex = 3;

            cut.Activate();

            cut.ActionTitle.Should().Be("OpenRepo/*.sln");
            cut.Rows.Select(r => r.Title).Should().Equal("A.sln", "B.sln");
            m_hideRequests.Should().Be(0);
        }
    }
}

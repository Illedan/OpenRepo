
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.Providers;
using OpenRepo.Providers.Snake;
using OpenRepo.Services;
using OpenRepo.Util;
using OpenRepo.View;

namespace OpenRepo.ViewModels
{
    public class MainViewModel : IViewModel
    {
        private readonly TextHandler m_textHandler = new TextHandler(string.Empty, true);
        private readonly IndexTraverser m_traverser = new IndexTraverser(0, 1);
        private readonly string m_configuration;

        private List<SelectableItem> m_items;
        private List<SelectableItem> m_currentItems;

        public MainViewModel(string configuration)
        {
            m_configuration = configuration;
        }

        public async Task Initialize()
        {
            m_items = await ProviderContainer.GetItems(m_configuration, new SnakeProviderFactory());
            m_currentItems = m_items;
            UpdateCurrentItems();
        }

        public List<TextLine> GetOutput()
        {
            var output = new List<TextLine>
            {
                new TextLine(string.IsNullOrEmpty(m_textHandler.Text) ? ("Version: " + OpenRepoVersionService.GetCurrentVersion()) : m_textHandler.Text, ConsoleColor.White)
            };

            if (!string.IsNullOrEmpty(LogService.Message))
            {
                output.Add(new TextLine(string.Empty, ConsoleColor.Red));
                output.Add(new TextLine(LogService.Message, ConsoleColor.Red));
                output.Add(new TextLine(string.Empty, ConsoleColor.Red));
            }

            if(m_currentItems.Count > 0)
            {
                var tempTraverser = new IndexTraverser(m_traverser.Current, m_currentItems.Count);
                var visibleItems = new List<SelectableItem>();
                var max = m_currentItems.Count > 11 ? 11 : m_currentItems.Count;
                var selected = max / 3;
                for (var i = 0; i < selected; i++) tempTraverser.MovePrevious();
                for(var i = 0; i < max; i++)
                {
                    visibleItems.Add(m_currentItems[tempTraverser.Current]);
                    tempTraverser.MoveNext();
                }

                for (var i = 0; i < visibleItems.Count; i++)
                {
                    var item = visibleItems[i];
                    if (i == selected)
                    {
                        output.Add(new TextLine($"> {item.Title}", ConsoleColor.Gray));
                    }
                    else
                    {
                        output.Add(new TextLine($"  {item.Title}", ConsoleColor.Gray));
                    }
                }
            }


            return output;
        }

        public void HandleInput(ConsoleKeyInfo input)
        {
            if (m_traverser.Handles(input))
            {
                m_traverser.Handle(input);
            }
            else if (m_textHandler.Handles(input))
            {
                m_textHandler.Handle(input);
                UpdateCurrentItems();
            }
            else if(input.Key == ConsoleKey.Escape)
            {
                m_textHandler.Clear();
                UpdateCurrentItems();
                LogService.Clear();
            }
            else if (input.Key == ConsoleKey.Tab)
            {
                LogService.Clear();
                Program.Reset();
            }
            else if(input.Key == ConsoleKey.Enter && m_currentItems.Count > 0)
            {
                var item = m_currentItems[m_traverser.Current];
                var actions = m_currentItems[m_traverser.Current].ActionsFactory();
                if(actions.Length == 1)
                {
                    actions[0].Action();
                }
                else
                {
                    Viewer.Push(new ActionSelectionViewModel(item, actions));
                }

                m_textHandler.Clear();
                UpdateCurrentItems();
            }
        }

        private void UpdateCurrentItems()
        {
            var previousSelected = m_traverser.Current < m_currentItems.Count && m_traverser.Current >= 0 ? m_currentItems[m_traverser.Current] : null;
            m_currentItems = ItemFilterService.Filter(m_items, m_textHandler.Text);

            m_traverser.Reset(m_currentItems.IndexOf(previousSelected), m_currentItems.Count);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using OpenRepo.Contracts;
using OpenRepo.Util;
using OpenRepo.View;

namespace OpenRepo.ViewModels
{
    public class ActionSelectionViewModel : IViewModel
    {
        private readonly IndexTraverser m_traverser = new IndexTraverser(0, 1);
        private SelectableAction[] m_actions;
        private IDictionary<char, SelectableAction> m_actionMapping = new Dictionary<char, SelectableAction>();
        private string m_title;
        public ActionSelectionViewModel(SelectableItem item, SelectableAction[] actions = null)
        {
            m_actions = actions ?? item.ActionsFactory();
            m_title = item.Title;
            m_traverser.Reset(0, m_actions.Length);
            foreach(var action in m_actions)
            {
                var targetLetter = action.Title.ToLower().FirstOrDefault(letter => !m_actionMapping.ContainsKey(letter));
                if(targetLetter == 0)
                {
                    m_actionMapping.Add((char)('0' + m_actions.ToList().IndexOf(action) + 1), action);
                }
                else
                {
                    m_actionMapping.Add(targetLetter, action);
                }
            }
        }

        public List<TextLine> GetOutput()
        {
            var output = new List<TextLine> { new TextLine($"Choose action for {m_title}", ConsoleColor.White) };
            for(var i = 0; i < m_actions.Length; i++)
            {
                var action = m_actions[i];
                var letter = m_actionMapping.First(a => a.Value == action).Key;
                if(m_traverser.Current == i)
                {
                    output.Add(new TextLine($"> ({letter}) {action.Title}", ConsoleColor.Red)); 
                }
                else
                {
                    output.Add(new TextLine($"  ({letter}) {action.Title}", ConsoleColor.Gray));
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
            else if (input.Key == ConsoleKey.Enter)
            {
                SelectAction(m_traverser.Current);
            }
            else if(input.Key == ConsoleKey.Escape || input.Key == ConsoleKey.Backspace)
            {
                Viewer.Pop();
            }
            else if (char.IsLetterOrDigit(input.KeyChar))
            {
                var hasMatch = m_actionMapping.TryGetValue(input.KeyChar, out var action);
                if(hasMatch)
                {
                    SelectAction(action);
                }
            }
        }

        private void SelectAction(int index) => SelectAction(m_actions[index]);

        private void SelectAction(SelectableAction action)
        {
            Viewer.Pop();
            action.Action();
        }
    }
}

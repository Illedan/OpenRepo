using System;
using System.Collections.Generic;
using OpenRepo.Contracts;
using OpenRepo.Services;
using OpenRepo.Util;
using OpenRepo.View;

namespace OpenRepo.ViewModels
{
    public class ActionSelectionViewModel : IViewModel
    {
        private readonly IndexTraverser m_traverser = new IndexTraverser(0, 1);
        private SelectableAction[] m_actions;
        private string m_title;
        public ActionSelectionViewModel(SelectableItem item)
        {
            m_actions = item.ActionsFactory();
            m_title = item.Title;
            m_traverser.Reset(0, m_actions.Length);
        }

        public List<TextLine> GetOutput()
        {
            var output = new List<TextLine> { new TextLine($"Choose action for {m_title}", ConsoleColor.White) };
            for(var i = 0; i < m_actions.Length; i++)
            {
                var action = m_actions[i];
                if(m_traverser.Current == i)
                {
                    output.Add(new TextLine($"> {action.Title}", ConsoleColor.Red)); 
                }
                else
                {
                    output.Add(new TextLine($"  {action.Title}", ConsoleColor.Gray));
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
                Viewer.Pop();
                var action = m_actions[m_traverser.Current];
                action.Action();
                LogService.Log($"Ran action {action.Title} on {m_title}.");
            }
            else if(input.Key == ConsoleKey.Escape || input.Key == ConsoleKey.Backspace)
            {
                Viewer.Pop();
            }
        }
    }
}

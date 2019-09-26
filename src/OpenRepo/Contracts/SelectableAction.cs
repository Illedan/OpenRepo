using System;
using OpenRepo.Services;

namespace OpenRepo.Contracts
{
    public class SelectableAction
    {
        private Action m_action;
        public SelectableAction(string title, Action action)
        {
            m_action = action;
            Title = title;
            Action = ExecuteAction;
        }

        public Action Action { get; }
        public string Title { get; }

        private void ExecuteAction()
        {
            try
            {
                m_action();
            }
            catch(Exception e)
            {
                LogService.Log(e.Message);
            }
        }
    }
}

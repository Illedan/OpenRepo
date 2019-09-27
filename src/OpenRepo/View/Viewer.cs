using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenRepo.Contracts;

namespace OpenRepo.View
{
    public static class Viewer
    {
        private static readonly ConcurrentStack<IViewModel> m_viewModelStack = new ConcurrentStack<IViewModel>();

        public static void Push(IViewModel viewModel)
        {
            m_viewModelStack.Push(viewModel);
            Draw(viewModel.GetOutput());
        }

        public static void Reset(IViewModel viewModel)
        {
            m_viewModelStack.Clear();
            Push(viewModel);
        }

        public static void Pop()
        {
            m_viewModelStack.TryPop(out _);
            var hasRenderer = m_viewModelStack.TryPeek(out var viewModel);
            if (!hasRenderer)
            {
                Console.Clear();
                return;
            }

            Draw(viewModel.GetOutput());
        }

        public static void Start()
        {
            while (true)
            {
                var hasRenderer = m_viewModelStack.TryPeek(out var viewModel);
                if (hasRenderer)
                {
                    Draw(viewModel.GetOutput());
                }

                var key = Console.ReadKey();

                if (!hasRenderer)
                {
                    hasRenderer = m_viewModelStack.TryPeek(out viewModel);
                }

                if (hasRenderer)
                {
                    viewModel.HandleInput(key);
                }
            }
        }

        public static void Draw(List<TextLine> lines)
        {
            Console.Clear();
            foreach(var l in lines)
            {
                Console.ForegroundColor = l.Color;
                Console.WriteLine(l.Title);
            }
        }
    }
}

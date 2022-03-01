using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public static async void Start()
        {
            await Task.Run(() =>
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
            });
        }
        private static TextWriter _tw = Console.Out;
        public static void Draw(List<TextLine> lines)
        {
            Console.SetError(TextWriter.Null);
            Console.SetOut(_tw);
            Console.Clear();
            foreach(var l in lines)
            {
                Console.ForegroundColor = l.Color;
                Console.WriteLine(l.Title);
            }

            Console.SetOut(TextWriter.Null);
        }
    }
}

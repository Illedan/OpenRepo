using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.View;
using System.Linq;

namespace OpenRepo.Providers.Snake
{
    public class SnakeViewModel : IViewModel
    {
        private readonly int m_time;
        private SnakeGame m_game;
        private bool m_running;
        public SnakeViewModel(int time)
        {
            m_game = new SnakeGame((11 - time/100));
            m_running = true;
            Start(time);
            m_time = time;
        }

        public List<TextLine> GetOutput() => new List<TextLine> { new TextLine("Arrows to control, enter to reset, escape to quit", ConsoleColor.White) }.Concat(m_game.GetView()).ToList();

        public void HandleInput(ConsoleKeyInfo input)
        {
            if (input.Key == ConsoleKey.Escape)
            {
                m_running = false;
                m_game.IsDead = true;
                Viewer.Pop();
            }
            else if(input.Key == ConsoleKey.Enter)
            {
                m_game.IsDead = true;
                m_game = new SnakeGame((11 - m_time / 100));
                Viewer.Draw(GetOutput());
            }
            else if (input.Key == ConsoleKey.UpArrow)
                m_game.OnMoveChanged(0, -1);
            else if(input.Key == ConsoleKey.DownArrow)
                m_game.OnMoveChanged(0, 1);
            else if (input.Key == ConsoleKey.RightArrow)
                m_game.OnMoveChanged(1, 0);
            else if (input.Key == ConsoleKey.LeftArrow)
                m_game.OnMoveChanged(-1, 0);
        }

        private async void Start(int time)
        {
            while (m_running)
            {
                await Task.Delay(time);
                m_game.GameLoop();
                if(!m_game.IsDead){

                    Viewer.Draw(GetOutput());
                }
            }
        }
    }
}

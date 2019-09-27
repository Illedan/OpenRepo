using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRepo.Contracts;

namespace OpenRepo.Providers.Snake
{
    public class SnakeGame
    {
        private static readonly Random rnd = new Random();

        private const int m_width = 15;
        private const int m_height = 10;
        private readonly string m_border = string.Empty.PadRight(m_width + 2, '@');
        private readonly List<Point> m_snake = new List<Point>();
        private readonly List<Point> m_eatenFood = new List<Point>();
        private Point m_currentFood;
        private int m_dx, m_dy;
        private int m_prevDx, m_prevDy;
        private readonly char[,] m_board = new char[m_width, m_height];
        public bool IsDead { get; set; }
        private readonly int m_scoring;

        public SnakeGame(int scoring)
        {
            m_scoring = scoring;
            var startX = rnd.Next(5, 10);
            var startY = rnd.Next(4, 6);
            m_prevDx = m_dx = 1;
            m_prevDy = m_dy = 0;
            var head = new Point(startX, startY);
            m_snake.Add(head);
            for(var i = 1; i < 4; i++)
            {
                m_snake.Add(new Point(head.X - i, head.Y));
            }

            CreateFood();
        }

        public void GameLoop()
        {
            if (IsDead) return;
            m_prevDx = m_dx;
            m_prevDy = m_dy;
            var head = m_snake[0];
            var next = new Point(head.X + m_dx, head.Y + m_dy);
            if(next == m_currentFood)
            {
                m_eatenFood.Add(m_currentFood);
                if(m_snake.Count == m_width * m_height)
                {
                    IsDead = true; // Win!
                    return;
                }

                CreateFood();
            }

            if (m_snake.Any(s => s == next))
            {
                IsDead = true;
                return;
            }

            m_snake.Insert(0, next);
            if(!m_eatenFood.Any(e => e == m_snake[m_snake.Count - 1]))
            {
                m_snake.RemoveAt(m_snake.Count - 1);
            }
            else
            {
                m_eatenFood.RemoveAll(e => e == m_snake[m_snake.Count - 1]);
            }

            if (m_snake[0].X < 0 || m_snake[0].X >= m_width || m_snake[0].Y < 0 || m_snake[0].Y >= m_height)
            {
                IsDead = true;
            }
        }

        private void CreateFood()
        {
            m_currentFood = new Point(rnd.Next(0, m_width), rnd.Next(0, m_height));
            while(m_snake.Any(s => s == m_currentFood))
            {
                m_currentFood = new Point(rnd.Next(0, m_width), rnd.Next(0, m_height));
            }
        }

        private int GetScore()
        {
            return (m_snake.Count - 4) * m_scoring;
        }

        public List<TextLine> GetView()
        {
            if (IsDead)
            {
                //TODO: save highscores.
                return new List<TextLine>
                {
                    new TextLine(m_border, ConsoleColor.Gray),
                    new TextLine(string.Empty, ConsoleColor.Gray),
                    new TextLine("     SCORE IS: " + GetScore(), ConsoleColor.Gray),
                    new TextLine(string.Empty, ConsoleColor.Gray),
                    new TextLine(m_border, ConsoleColor.Gray)
                };
            }

            for (var x = 0; x < m_width; x++)
            {
                for(var y = 0; y < m_height; y++)
                {
                    if (m_eatenFood.Any(p => p.X == x && p.Y == y)) m_board[x, y] = 'O';
                    else if (m_snake.Any(p => p.X == x && p.Y == y)) m_board[x, y] = '#';
                    else if (m_currentFood.X == x && m_currentFood.Y == y) m_board[x, y] = 'x';
                    else m_board[x, y] = ' ';
                }
            }

            var view = new List<TextLine>();
            view.Add(new TextLine(m_border, ConsoleColor.Gray));
            for (var y = 0; y < m_height; y++)
            {
                var line = "@";
                for (var x = 0; x < m_width; x++)
                {
                    line += m_board[x, y];
                }

                line += "@";
                view.Add(new TextLine(line, ConsoleColor.Gray));
            }
            view.Add(new TextLine(m_border, ConsoleColor.Gray));

            view.Add(new TextLine(string.Empty, ConsoleColor.Gray));
            view.Add(new TextLine("SCORE: " + GetScore(), ConsoleColor.Gray));
            return view;
        }

        public void OnMoveChanged(int dx, int dy)
        {
            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1 || Math.Abs(dx) + Math.Abs(dy) != 1) throw new Exception("Only 1 in either direction");
            if (m_prevDx * -1 == dx && m_prevDy * -1 == dy) return; // Don't go straight back
            m_dx = dx;
            m_dy = dy;
        }
    }
}

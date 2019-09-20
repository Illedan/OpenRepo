using System;

namespace OpenRepo.Util
{
    public class IndexTraverser
    {
        private int m_max;
        public IndexTraverser(int index, int max) => Reset(index, max);

        public void Reset(int index, int max)
        {
            m_max = max;
            Current = index;
            if (Current >= max || Current < 0) Current = 0;
        }

        public int Current { get; private set; }

        public void MoveNext()
        {
            Current++;
            if (Current >= m_max) Current = 0;
        }

        public void MovePrevious()
        {
            Current--;
            if (Current < 0) Current = m_max - 1;
        }


        public bool Handles(ConsoleKeyInfo input)
        {
            return input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.DownArrow;
        }

        public void Handle(ConsoleKeyInfo input)
        {
            if(input.Key == ConsoleKey.UpArrow)
            {
                MovePrevious();
            }
            else
            {
                MoveNext();
            }
        }
    }
}

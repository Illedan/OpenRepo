using System;
namespace OpenRepo.Util
{
    public class TextHandler
    {
        private bool m_acceptsSpace;
        public TextHandler(string initial, bool acceptSpace)
        {
            m_acceptsSpace = acceptSpace;
            Text = initial ?? string.Empty;
        }

        public string Text { get; private set; }


        public bool Handles(ConsoleKeyInfo input)
        {
            return input.Key == ConsoleKey.Backspace
                || char.IsLetterOrDigit(input.KeyChar)
                || (m_acceptsSpace && input.Key == ConsoleKey.Spacebar);
        }

        public void Handle(ConsoleKeyInfo input)
        {
            if (char.IsLetterOrDigit(input.KeyChar) || input.Key == ConsoleKey.Spacebar)
            {
                Text += input.KeyChar;
            }
            else if (input.Key == ConsoleKey.Backspace)
            {
                Text = Text.Length > 0 ? Text.Substring(0, Text.Length - 1) : string.Empty;
            }
        }

        public void Clear() => Text = string.Empty;
    }
}

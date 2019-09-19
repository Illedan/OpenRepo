using System;
namespace OpenRepo.Contracts
{
    public class TextLine
    {
        public TextLine(string title, ConsoleColor color = ConsoleColor.White) 
        {
            Title = title;
            Color = color;
        }

        public string Title { get; }
        public ConsoleColor Color { get; }
    }
}

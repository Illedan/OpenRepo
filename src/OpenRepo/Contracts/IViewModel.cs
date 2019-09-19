using System;
using System.Collections.Generic;

namespace OpenRepo.Contracts
{
    public interface IViewModel
    {
        List<TextLine> GetOutput();

        void HandleInput(ConsoleKeyInfo input);
    }
}

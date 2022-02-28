using System;
using System.Collections.Generic;
using System.Text;
using TextCopy;

namespace Illedan.OpenRepo.Services
{
    public static class CopyToClipboardService
    {
        public static void AddTextToClipboard(string text)
        {
            ClipboardService.SetText(text);
        }
    }
}

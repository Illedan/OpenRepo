using System;
using System.IO;

namespace OpenRepo.Services
{
    public static class FileService
    {
        public static string GetFileName(string path) => Path.GetFileName(path);
    }
}

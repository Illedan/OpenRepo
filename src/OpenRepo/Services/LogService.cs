namespace OpenRepo.Services
{
    public static class LogService
    {
        public static void Log(string content)
        {
            if (string.IsNullOrEmpty(content.Trim())) return;
            Message += "\n" + content;
        }

        public static void Clear()
        {
            Message = string.Empty;
        }

        public static string Message { get; private set; } = string.Empty;
    }
}

namespace OpenRepo.Services
{
    public static class LogService
    {
        public static void Log(string content)
        {
            Message = content ?? string.Empty;
        }

        public static string Message { get; private set; } = string.Empty;
    }
}

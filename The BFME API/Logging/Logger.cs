namespace The_BFME_API.Logging
{
    public static class Logger
    {
        public static event EventHandler<string>? OnWarning;
        public static event EventHandler<string>? OnDiagnostic;

        internal static void LogWarning(string message, string subsystem)
        {
            OnWarning?.Invoke(null, $"[{subsystem}]: {message}");
        }

        internal static void LogDiagnostic(string message, string subsystem)
        {
            OnDiagnostic?.Invoke(null, $"[{subsystem}]: {message}");
        }
    }
}

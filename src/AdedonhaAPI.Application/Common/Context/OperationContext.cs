namespace AdedonhaAPI.Application.Common.Context
{
    public static class OperationContext
    {
        private static readonly AsyncLocal<string?> _current = new();

        public static string? Current => _current.Value;
        public static void Set(string operationId) => _current.Value = operationId;
        public static void Clear() => _current.Value = null;
    }
}

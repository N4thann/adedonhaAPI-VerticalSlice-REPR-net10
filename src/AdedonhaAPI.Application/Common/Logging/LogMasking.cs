namespace AdedonhaAPI.Application.Common.Logging
{
    public static class LogMasking
    {
        public static string MaskEmail(string email)
        {
            var atIndex = email.IndexOf('@');
            if (atIndex <= 0) return "***";
            return $"{email[0]}***{email[atIndex..]}";
        }
    }
}

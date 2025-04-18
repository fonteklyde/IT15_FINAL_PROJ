namespace IT15_Final_Proj.Services
{
    public static class LoginAttemptTracker
    {
        private static Dictionary<string, (int Count, DateTime LastAttempt)> attempts = new();

        private const int MaxAttempts = 3;
        private static readonly TimeSpan LockoutTime = TimeSpan.FromMinutes(10);

        public static bool IsLockedOut(string email)
        {
            if (!attempts.ContainsKey(email))
                return false;

            var (count, lastAttempt) = attempts[email];
            if (count >= MaxAttempts && (DateTime.UtcNow - lastAttempt) < LockoutTime)
                return true;

            if ((DateTime.UtcNow - lastAttempt) > LockoutTime)
            {
                attempts.Remove(email);
                return false;
            }

            return false;
        }

        public static void RecordFailedAttempt(string email)
        {
            if (!attempts.ContainsKey(email))
            {
                attempts[email] = (1, DateTime.UtcNow);
            }
            else
            {
                var (count, _) = attempts[email];
                attempts[email] = (count + 1, DateTime.UtcNow);
            }
        }

        public static void ResetAttempts(string email)
        {
            if (attempts.ContainsKey(email))
                attempts.Remove(email);
        }
    }
}

using System;

namespace Extensions
{
    public static class TimeExtensions
    {
        public static string GetFormatedSeconds(this float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            string result = $"{time.Hours}h:{time.Minutes}m:{time.Seconds}s";
            return result;
        }
    }
}
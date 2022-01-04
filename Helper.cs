using System;
using System.Globalization;

namespace AutoReplyUserBot
{
    public static class Helper
    {
        internal static string GetSystemLanguage()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }

        internal static string GetDeviceModel()
        {
            return $"AutoReply server";
        }

        internal static string GetOSVersion()
        {
            return Environment.OSVersion.VersionString + (Environment.Is64BitOperatingSystem ? " (64 bits)" : " (32 bits)");
        }

        public static string GetAppVersion()
        {
            //TODO: Getting Assembly version
            return "2.0.1";
        }
    }
}

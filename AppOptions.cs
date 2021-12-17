using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoReplyUserBot
{
    public static class AppOptions
    {
        // Required
        public static int API_ID = -1;
        public static string API_HASH = string.Empty;

        // TDLib Parameters
        public static bool UseTestDc = false;
        public static string OSVersion = Environment.OSVersion.VersionString + (Environment.Is64BitOperatingSystem ? " x64 (64 Bits)" : " x86 (32 Bits)");
        public static string SystemLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        public static string DeviceModel = "AutoReply Server";
        public static bool UseFilesDB = false;
        public static bool UseChatsDB = false;
        public static bool UseMessagesDB = false;
        public static bool UseSecretChats = false;

        // App output
        public static bool AllowOutput = true;
        public static bool AllowCommands = true;
        public static bool TDLibVersion = true;
        public static bool ConnectionStateUpdates = true;
        public static bool AuthenticatedMessage = true;

        // Userbot Data
        public static bool UseAutoReplySigniture = true;
        public static bool ReplyWhenOnline = false;
        public static bool OnlyInPrivateChats = true; // New feature
        public static string AutoReplySignitureText = "- An auto replied message";

        // Logs
        public static int LogLevel = 2;
        public static string LogsFilePath = "Auto-reply.tdlib.log";

        public const string Version = "0.4";

        public static void Refresh()
        {
            if (!AllowOutput)
            {
                TDLibVersion = false;
                AuthenticatedMessage = false;
                ConnectionStateUpdates = false;
            }
        }
    }
}

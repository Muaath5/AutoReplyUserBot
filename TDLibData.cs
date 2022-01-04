namespace AutoReplyUserBot
{
    public static class TDLibData
    {
        // Required
        public static int API_ID = -1;
        public static string API_HASH = string.Empty;

        // Optional
        public static bool UseTestDc = false;
        public static bool UseFilesDB = false;
        public static bool UseChatsDB = false;
        public static bool UseMessagesDB = false;
        public static bool UseSecretChats = false;
        public static bool EnableStorageOptimizer = true;

        /// <summary>
        /// Log level 4 is a secret, don't share it with anybody, it'll contain you API_ID & API_HASH
        /// Logging will be via TDLib
        /// </summary>
        public static int LogLevel = 4;
        public static string LogsFilePath = "AutoReplyUserBot.tdlib.log";
    }
}

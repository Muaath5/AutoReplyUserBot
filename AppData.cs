using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoReplyUserBot
{

    public static class AppData
    {
        // App output
        public static bool AllowOutput = true;
        public static bool AllowCommands = true;
        public static bool TDLibVersion = true;
        public static bool ConnectionStateUpdates = true;
        public static bool AuthenticatedMessage = true;

        // Userbot Data
        public static bool UseAutoReplySigniture = true;
        public static bool ReplyWhenOnline = false;
        public static bool OnlyInPrivateChats = true;
        public static string AutoReplySignitureText = "- An auto replied message";

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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Td;
using Telegram.Td.Api;

namespace AutoReplyUserBot
{
    public static class Program
    {
        public static readonly UpdatesHandler AppUpdatesHandler = new();
        public static Client App = null;

        public static User Me = null;
        public static string TDLibVersion = "unknown";
        public static DateTime AuthorizationDate;

        public static Dictionary<string, FormattedText> AutoReplies = new Dictionary<string, FormattedText>();
        public static string State = "none";

        // For avoiding the duplicate updates of userbot messages
        public static long LastMessageID = -1;

        // Only temporary using
        public static string currentCommand = null;

        public static bool AllowOutput = true;

        public static bool Exit = false;

        private static DateTime StartTime;

        private static Thread TDLibThread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            Client.Run();
        });

        public static TimeSpan RunTime
        {
            get
            {
                return DateTime.Now.Subtract(StartTime);
            }
        }

        static void Main(string[] args)
        {
            StartTime = DateTime.Now;
            Console.ResetColor();
            UseEnvironmentVariables();
            if (!ProcessConsoleInput(args))
            {
                Console.Error.WriteLine("App exited by an error!");
                return;
            }

            if (AppData.AllowOutput)
            {
                Console.WriteLine("Auto-reply userbot!");
            }

            Client.Execute(new SetLogStream(new LogStreamFile(TDLibData.LogsFilePath, long.MaxValue, false)));
            Client.Execute(new SetLogVerbosityLevel(TDLibData.LogLevel));
            if (AppData.AllowOutput)
            {
                Console.WriteLine($"Log level: {TDLibData.LogLevel}");
                Console.WriteLine($"Logs saved in {TDLibData.LogsFilePath}");
            }

            if (AppData.AllowCommands)
            {
                Console.CancelKeyPress += Console_CancelKeyPress;
            }

            // TDLib client & Multithreading managing
            CreateNewClient();
            TDLibThread.Start();

            // No stop
            while (!Exit) {}
        }

        private static void UseEnvironmentVariables()
        {
            // TELEGRAM_API_ID
            string api_id_env = Environment.GetEnvironmentVariable("TELEGRAM_API_ID");
            if (api_id_env != null)
            {
                if (int.TryParse(api_id_env, out int api_id))
                {
                    TDLibData.API_ID = api_id;
                }
                else
                {
                    Console.Error.WriteLine("TELEGRAM_API_ID must be an integer number");
                    Exit = true;
                }
            }

            // TELEGRAM_API_HASH
            string api_hash = Environment.GetEnvironmentVariable("TELEGRAM_API_HASH");
            if (api_hash != null)
            {
                TDLibData.API_HASH = api_hash;
            }
        }

        private static bool ProcessConsoleInput(string[] args)
        {
            string consoleInputOpt = "";
            string currentInputText = "";
            int consoleInputState = 0;
            for (int i = 0; i < args.Length; i++)
            {
                string currentOpt = args[i].StartsWith('-') || args[i].StartsWith('/') ? args[i].ToUpper().Substring(1) : args[i].ToUpper();
                switch (currentOpt)
                {
                    case "DISABLE_COMMANDS":
                        AppData.AllowCommands = false;
                        break;

                    case "DISABLE_OUTPUT":
                        AppData.AllowOutput = false;
                        break;

                    case "NO_LOGS":
                        Console.WriteLine("Note: Logs will contains ONLY Fatal errors!");
                        TDLibData.LogLevel = 0;
                        break;

                    case "DB_ALL":
                        TDLibData.UseChatsDB = true;
                        TDLibData.UseFilesDB = true;
                        TDLibData.UseMessagesDB = true;
                        break;

                    case "REPLY_WHEN_ONLINE":
                        AppData.ReplyWhenOnline = true;
                        break;

                    case "NO_AUTO_REPLY_SIGNITURE":
                        AppData.UseAutoReplySigniture = false;
                        break;

                    case "TEST_DC":
                        TDLibData.UseTestDc = true;
                        break;

                    case "API_ID":
                    case "API_HASH":
                    case "TELEGRAM_API_ID":
                    case "TELEGRAM_API_HASH":
                    case "LOG_LEVEL":
                    case "NEW_AUTO_REPLY_SIGNITURE":
                        consoleInputState = 1;
                        consoleInputOpt = currentOpt;
                        break;

                    default:
                        if (consoleInputState > 0)
                        {
                            if (consoleInputState == 1)
                            {
                                if (consoleInputOpt == "API_ID" || consoleInputOpt == "TELEGRAM_API_ID")
                                {
                                    if (!int.TryParse(currentOpt, out int api_id))
                                    {
                                        Console.Error.WriteLine($"{consoleInputOpt} isn\'t an integer number!");
                                        return false;
                                    }
                                    else
                                    {
                                        TDLibData.API_ID = api_id;
                                    }
                                }
                                else if (consoleInputOpt == "API_HASH" || consoleInputOpt == "TELEGRAM_API_HASH")
                                {
                                    TDLibData.API_HASH = currentOpt.ToLower();
                                }
                                else if (consoleInputOpt == "LOG_LEVEL")
                                {
                                    if (!int.TryParse(currentOpt, out int logLevel))
                                    {
                                        Console.Error.WriteLine($"{consoleInputOpt} isn\'t an integer number!");
                                        return false;
                                    }
                                    else
                                    {
                                        TDLibData.LogLevel = logLevel;
                                    }
                                }
                                else if (currentOpt.StartsWith('"'))
                                {
                                    // To remove
                                    currentInputText += currentOpt.Substring(1);
                                }
                            }

                            // In case of long text, Currently [NEW_AUTO_REPLY_SIGNITURE]
                            if (consoleInputState == 2)
                            {
                                if (currentOpt.EndsWith('"'))
                                {
                                    currentOpt.Remove(currentOpt.Length - 1);

                                    if (consoleInputOpt == "NEW_AUTO_REPLY_SIGNITURE")
                                    {
                                        AppData.AutoReplySignitureText = currentInputText;
                                    }

                                    currentInputText = "";
                                    consoleInputState = 0;
                                }
                                currentInputText += currentOpt;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine($"Invalid console argument ({args[i].ToUpper()})");
                            Console.ResetColor();
                        }
                        break;
                }
            }

            // To make all info compitible
            AppData.Refresh();

            return true;
        }

        private static void CreateNewClient()
        {
            App = Client.Create(AppUpdatesHandler);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
        EnterCmd:
            if (AllowOutput)
            {
                Console.Write("Enter command: ");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            string cmdline = Console.ReadLine();
            Console.ResetColor();
            if (string.IsNullOrWhiteSpace(cmdline)) Environment.Exit(0);
            string[] cmds = cmdline.Split(' ');
            string cmd = cmds[0];
            switch (cmd.ToLower())
            {
                case "logout":
                    App.Send(new LogOut(), null);
                    App.Send(new Close(), null);
                    Exit = true;
                    break;

                case "close":
                    App.Send(new Close(), null);
                    CreateNewClient();
                    return;

                case "info":
                    App.Send(new GetMe(), new UserUpdateHandler());
                    goto EnterCmd;

                case "config":
                    Console.WriteLine("Sending GetApplicationConfig");
                    App.Send(new GetApplicationConfig(), new OutputHandler());
                    Console.WriteLine("GetApplicationConfig was sent");
                    goto EnterCmd;

                case "getoption":
                    App.Send(new GetOption(cmds[1]), new OutputHandler());
                    Thread.Sleep(600);
                    goto EnterCmd;

                case "tdlib_version":
                case "tdlib":
                case "tdlib_ver":
                    Console.WriteLine($"TDLib version: {TDLibVersion}");
                    break;

                case "version":
                case "v":
                case "ver":
                    Console.WriteLine($"AutoReplyUserBot version: {Helper.GetAppVersion()}");
                    break;

                case "":
                case "\n":
                    Exit = true;
                    break;

                default:
                    Console.Error.WriteLine("Command not found, If you don't need anything press Enter");
                    goto EnterCmd;
            }
        }
    }
}
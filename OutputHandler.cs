using System;
using Telegram.Td;
using Telegram.Td.Api;

namespace AutoReplyUserBot
{
    public class OutputHandler : ClientResultHandler
    {
        public void OnResult(BaseObject @object)
        {
            if (@object is Error err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error[{err.Code}]: {err.Message}");
                Console.ResetColor();
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(@object);
            }
        }
    }
}
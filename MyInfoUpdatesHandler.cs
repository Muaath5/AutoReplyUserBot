using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td;
using Telegram.Td.Api;

namespace AutoReplyUserBot
{
    public class MyInfoUpdatesHandler : ClientResultHandler
    {
        public async void OnResult(BaseObject @object)
        {
            if (@object is User me)
            {
                var fullInfo = (await Program.App.SendAsync(new GetUserFullInfo(me.Id))) as UserFullInfo;

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("ID: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(me.Id);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("First name: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(me.FirstName);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Last name: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(me.LastName);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Username: ");
                Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine('@' + me.Username);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Phone number: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine('+' + me.PhoneNumber);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Bio: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(fullInfo.Bio);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Has profile photo? ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(me.ProfilePhoto != null ? $"Yes, Animated? {me.ProfilePhoto.HasAnimation}" : "No");

                Console.ResetColor();
            }
        }
    }
}

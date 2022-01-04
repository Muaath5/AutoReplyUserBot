using System;
using Telegram.Td;
using Telegram.Td.Api;

namespace AutoReplyUserBot
{
    public class UserUpdateHandler : ClientResultHandler
    {
        public async void OnResult(BaseObject @object)
        {
            if (@object is User user)
            {
                var fullInfo = (await Program.App.SendAsync(new GetUserFullInfo(user.Id))) as UserFullInfo;

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("ID: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(user.Id);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("First name: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(user.FirstName);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Last name: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(user.LastName);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Username: ");
                Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine('@' + user.Username);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Phone number: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine('+' + user.PhoneNumber);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Bio: ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(fullInfo.Bio);

                Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("Has profile photo? ");
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(user.ProfilePhoto != null ? $"Yes, Animated? {user.ProfilePhoto.HasAnimation}" : "No");

                Console.ResetColor();
            }
        }
    }
}

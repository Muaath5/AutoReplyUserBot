using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Td;
using Telegram.Td.Api;

namespace AutoReplyUserBot
{
    public class UpdatesHandler : ClientResultHandler
    {
        public async void OnResult(BaseObject @object)
        {
            if (@object is UpdateAuthorizationState updAuthState)
            {
                await OnAuthorizationState(updAuthState.AuthorizationState);
            }
            else if (@object is UpdateNewMessage updMessage)
            {
                await OnMessage(updMessage.Message);
            }
            else if (@object is UpdateConnectionState updConnState && AppOptions.ConnectionStateUpdates)
            {
                if (updConnState.State is ConnectionStateReady)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connected!");
                    Console.ResetColor();
                }
                else if (updConnState.State is ConnectionStateUpdating)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Updating...");
                    Console.ResetColor();
                }
                else if (updConnState.State is ConnectionStateWaitingForNetwork)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Waiting for network..!");
                    Console.ResetColor();
                }
                else if (updConnState.State is ConnectionStateConnecting)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Connecting...");
                    Console.ResetColor();
                }
                else if (updConnState.State is ConnectionStateConnectingToProxy)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Connecting to proxy...");
                    Console.ResetColor();
                }
            }
            else if (@object is UpdateOption updOpt)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (updOpt.Name == "version")
                {
                    Program.TDLibVersion = (updOpt.Value as OptionValueString).Value;
                    if (AppOptions.TDLibVersion)
                        Console.WriteLine($"TDLib Version: {Program.TDLibVersion}");
                }
                //else if (updOpt.Name == "version" && AppOptions.AllowOutput && false)
                //{
                //    Console.WriteLine($"TDLib Version: {(updOpt.Value as OptionValueString).Value}");
                //}
                Console.ResetColor();
            }
            else
            {
                // Console.WriteLine(@object);
            }
        }

        public async Task OnAuthorizationState(AuthorizationState authState)
        {
            try
            {
                if (authState is AuthorizationStateWaitTdlibParameters)
                {
                    await Program.App.SendAsync(new SetTdlibParameters(new TdlibParameters(
                        AppOptions.UseTestDc,
                        "AutoReplyUserBotDB",
                        "AutoReplyUserBotFiles",
                        AppOptions.UseFilesDB,
                        AppOptions.UseChatsDB,
                        AppOptions.UseMessagesDB,
                        AppOptions.UseSecretChats,
                        AppOptions.API_ID,
                        AppOptions.API_HASH,
                        AppOptions.SystemLanguage,
                        AppOptions.DeviceModel,
                        AppOptions.OSVersion,
                        AppOptions.Version,
                        true,
                        true
                    )));
                }
                else if (authState is AuthorizationStateWaitEncryptionKey)
                {
                    await Program.App.SendAsync(new SetDatabaseEncryptionKey());
                }
                else if (authState is AuthorizationStateWaitPhoneNumber)
                {
                    Console.Write("Enter phone number: ");
                    await Program.App.SendAsync(
                        new SetAuthenticationPhoneNumber(
                            Console.ReadLine(), // Phone number
                            new PhoneNumberAuthenticationSettings(false, true, false, false, new string[] { })
                        )
                    );
                }
                else if (authState is AuthorizationStateWaitCode code)
                {
                    string type = "unknown";
                    string length = "unknown";
                    if (code.CodeInfo.Type is AuthenticationCodeTypeTelegramMessage message)
                    {
                        type = "message";
                        length = message.Length.ToString();
                    }
                    else if (code.CodeInfo.Type is AuthenticationCodeTypeSms sms)
                    {
                        type = "SMS";
                        length = sms.Length.ToString();
                    }
                    else if (code.CodeInfo.Type is AuthenticationCodeTypeMissedCall missedCall)
                    {
                        type = "Missed call";
                        length = missedCall.Length.ToString();
                    }
                    else if (code.CodeInfo.Type is AuthenticationCodeTypeFlashCall flashCall)
                    {
                        type = "Flash call";
                        //length = flashCall.Length.ToString();
                    }
                    else if (code.CodeInfo.Type is AuthenticationCodeTypeCall call)
                    {
                        type = "Call";
                        length = call.Length.ToString();
                    }
                    Console.WriteLine($"An authentication code received by {type}");
                    Console.Write($"Enter code (Length: {length}): ");
                    await Program.App.SendAsync(new CheckAuthenticationCode(Console.ReadLine()));
                }
                else if (authState is AuthorizationStateWaitPassword password)
                {
                EnterCloudPassword:
                    Console.WriteLine("CLOUD PASSWORD (If empty means resetting your password)");
                    Console.Write($"Enter your cloud password ({password.PasswordHint}): ");
                    string passwordStr = Console.ReadLine();
                    if (passwordStr == "")
                    {
                        if (password.HasRecoveryEmailAddress)
                        {
                            await Program.App.SendAsync(new RequestAuthenticationPasswordRecovery());
                            Console.WriteLine($"Sent recovery code to {password.RecoveryEmailAddressPattern}");
                            Console.Write("Enter recovery code: ");
                            await Program.App.SendAsync(new CheckRecoveryEmailAddressCode(Console.ReadLine()));
                        }
                        else
                        {
                            Console.WriteLine("There's no recovery email, Use Some logged in account to reset it");
                            goto EnterCloudPassword;
                        }
                    }
                    else
                    {
                        await Program.App.SendAsync(new CheckAuthenticationPassword(passwordStr));
                    }
                }
                else if (authState is AuthorizationStateReady)
                {
                    var me = await Program.App.SendAsync(new GetMe()) as User;
                    if (AppOptions.AuthenticatedMessage)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Authenticated!");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Hi, {me.FirstName}. your ID is {me.Id} :)");
                        Program.Me = me;

                        // Converting unix-timestmap
                        var authDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        BaseObject authDateOpt = await Program.App.SendAsync(new GetOption("authorization_date"));
                        long unixAuthDate = (authDateOpt as OptionValueInteger).Value;
                        Program.AuthorizationDate = authDate.AddSeconds(unixAuthDate).ToLocalTime();
#if DEBUG
                        Console.WriteLine("Auth Date: " + unixAuthDate);
#endif

                        Console.ResetColor();
                    }
                    Thread.Sleep(1000);
                }
                else if (authState is AuthorizationStateLoggingOut)
                {
                    Console.WriteLine("Logging out..");
                }
                else if (authState is AuthorizationStateClosing)
                {
                    Console.WriteLine("Closing..");
                }
                else if (authState is AuthorizationStateClosed)
                {
                    Console.WriteLine("The App closed!");
                }
            }
            catch (TDLibException ex)
            {
                // Output in red color
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error[{ex.Code}]: {ex.Message}");
                Console.ResetColor();

                Thread.Sleep(1000);

                if (ex.Message == "Valid api_id must be provided. Can be obtained at https://my.telegram.org")
                {
                    Program.Exit = true;
                }
                else
                {
                    // Repeat authorization
                    await OnAuthorizationState(authState);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error[Unknown] in {ex.Source}: {ex.Message}");
                Console.ResetColor();
            }
        }

        public async Task OnMessage(Message message)
        {
            var me = (await Program.App.SendAsync(new GetMe()) as User);
            if (message.Content is MessageText text)
            {
                if (message.ChatId == me.Id && message.Id != Program.LastMessageID)
                {
                    string txt = text.Text.Text;
                    string botReply = "";

                    if (txt[0] == '/')
                    {
                        if (txt == "/cancel")
                        {
                            Program.State = "canceled";
                            botReply = "Ok, Last command was canceled";
                        }
                        else if (txt == "/add")
                        {
                            Program.State = "waiting_command";
                            botReply = "Good, Now send the command";
                        }
                        else if (txt == "/auto_replies")
                        {
                            Program.State = "auto_replies_sent";
                            botReply = "Auto-replies is:";
                            foreach (var autoReply in Program.AutoReplies)
                            {
                                botReply += $"\n{autoReply.Key} == {autoReply.Value.Text}";
                            }
                        }
                        else if (txt == "/state")
                        {
                            botReply = @$"<b>Auto-reply userbot info:</b>
User ID: <code>{Program.Me.Id}</code>
TDLib Version: <code>{Program.TDLibVersion}</code>
Auth date: <code>{Program.AuthorizationDate.ToString("yyyy/M/d t h:m:s")}</code>
Run time: <code>{Program.RunTime}</code>
Auto-replies count: {Program.AutoReplies.Count}
Auto-reply signature: <i>{(AppOptions.UseAutoReplySigniture ? AppOptions.AutoReplySignitureText : "")}</i>
Auto-reply state: <code>{Program.State}</code>";
                            Program.State = "state_sent";
                        }
                        else if (txt == "/signature")
                        {
                            Program.State = "waiting_signature";
                            botReply = "Send the new signature.\nIf you don't want a signature, send <i>\"disable\"</i>";
                        }
                        else if (txt == "/clear")
                        {
                            Program.AutoReplies.Clear();
                            Program.State = "auto_replies_cleared";
                            botReply = "Deleted all commands successfully";
                        }
                        else if (txt == "/allow_reply_when_online")
                        {
                            AppOptions.ReplyWhenOnline = true;
                            Program.State = "allowed_reply_when_online";
                            botReply = "Now the userbot <b>will reply</b> even if you are online";
                        }
                        else if (txt == "/disallow_reply_when_online")
                        {
                            AppOptions.ReplyWhenOnline = false;
                            Program.State = "disallowed_reply_when_online";
                            botReply = "Now the userbot <b>won't</b> reply even if you are online";
                        }
                        else
                        {
                            botReply = @"Please use a valid command, Commands is:
/add - Adds new auto-reply
/auto_replies - Shows all stored auto-replies
/clear - Clears all auto-replies
/state - Getting info about the userbot
/signature - Editing/Disabling signature
/allow_reply_when_online - Allowing userbot to reply when online
/disallow_reply_when_online - Disallowing userbot to reply when online
/cancel - Cancels last operation";
                        }
                    }
                    else if (Program.State == "waiting_command")
                    {
                        if (Program.AutoReplies.ContainsKey(text.Text.Text))
                        {
                            botReply = "This command is already in replies!";
                        }
                        else
                        {
                            Program.currentCommand = text.Text.Text;
                            Program.State = "waiting_reply";
                            botReply = "Command added, Now add the reply";
                        }
                    }
                    else if (Program.State == "waiting_reply")
                    {
                        Program.AutoReplies.Add(Program.currentCommand, text.Text);
                        Program.State = "auto_reply_added";
                        botReply = $"the Auto-reply added, Now you have {Program.AutoReplies.Count} Auto-replies..";
                    }
                    else if (Program.State == "waiting_siganture")
                    {
                        if (txt == "false" || txt == "no" || txt == "disable")
                        {
                            AppOptions.UseAutoReplySigniture = false;
                            botReply = "Auto-reply signature was disabled!";
                        }
                        else
                        {
                            AppOptions.AutoReplySignitureText = txt;
                            AppOptions.UseAutoReplySigniture = true;
                            botReply = "Auto-reply signature was enabled!";
                        }
                        Program.State = "signature_edited";
                    }


                    if (botReply != "")
                    {
                        Message lastMsg = await Program.App.SendAsync(
                            new SendMessage(me.Id,
                                0,
                                message.Id,
                                new MessageSendOptions(),
                                null,
                                new InputMessageText(Client.Execute(new ParseTextEntities(botReply, new TextParseModeHTML())) as FormattedText, true, false)
                            )
                        ) as Message;
                        Program.LastMessageID = lastMsg.Id;
                    }
                }
                else if (!(me.Status is UserStatusOnline) || !AppOptions.ReplyWhenOnline && message.ChatId != me.Id)
                {
                    Chat chat = await Program.App.SendAsync(new GetChat(message.ChatId)) as Chat;

                    // Replies only in private chats
                    if (chat.Type is ChatTypePrivate)
                    {
                        foreach (var reply in Program.AutoReplies)
                        {
                            if (text.Text.Text.Contains(reply.Key))
                            {
                                FormattedText editedReply = reply.Value;
                                editedReply.Text += (AppOptions.UseAutoReplySigniture ? "\n\n" + AppOptions.AutoReplySignitureText : "");
                                await Program.App.SendAsync(
                                    new SendMessage(
                                        message.ChatId,
                                        0,
                                        message.Id,
                                        new MessageSendOptions(),
                                        null,
                                        new InputMessageText(editedReply, false, false)
                                    )
                                );
                            }
                        }
                    }
                    await Program.App.SendAsync(new SetOption("online", new OptionValueBoolean(false)));
                }
            }
        }
    }
}

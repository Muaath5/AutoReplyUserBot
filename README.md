# Telegram Auto-reply userbot
This userbot will auto-reply messages.	

## How does the bot works?
The bot will reply only if the message contains some specific words that you select it.

## How can I add replies?
First, You should send `/add` in **_Saved Messages_**.
Then send the text that if the user sent a message contains similar text.
At last send the reply.

## Building
### Prerequist
1. Git
2. Visual Studio (2019 is suggested) (With C#)

### Steps
1. Open in Visual Studio and clone the repo
2. Running `git clone https://github.com/Muaath5/TDLibBinaries.git` in a folder next to this repo's folder
3. Run the application

## Running the app
At first, You should set `TELEGRAM_API_ID` and `TELEGRAM_API_HASH` environment variables, Unless you'll get an error.
Or use `API_ID` & `API_HASH` parameters in running

After that, There are some parameters you can use:
- `NO_LOGS`: To disable TDLib logs
- `DB_ALL`: To run all TDLib databases
- `TEST_DC`: To run TDLib in the test servers
- `REPLY_WHEN_ONLINE`: If you set this parameter, The userbot will reply even if you're online
- `NO_AUTO_REPLY_SIGNITURE`: There's a signiture of messages sent by the userbot, By this you can disable them

And you can use this parameters and after it enter a text
- `TELEGRAM_API_ID` or `API_ID`: Your API ID.
- `TELEGRAM_API_HASH` or `API_HASH`: Your API HASH.
- `LOG_LEVEL`: TDLib logging level, Between 0 and 1024
- `NEW_AUTO_REPLY_SIGNITURE`: To choose new signiture

You can use it like this:
```cmd
AutoReplyUserBot.exe /API_ID 123456 /API_HASH abcd1234 /TEST_DC /DB_ALL /NO_LOGS
```
Or like this:
```cmd
AutoReplyUserBot.exe -API_ID 123456 -API_HASH abcd1234 -DB_ALL -NO_LOGS -REPLY_WHEN_ONLINE
```
And even without `-` or `/`:
```cmd
AutoReplyUserBot.exe API_ID 123456 API_HASH abcd1234 NO_AUTO_REPLY_SIGNITURE TEST_DC
```

## Available commands
These commands can be used only in **Saved Messages**
- `/add`: To add new reply
- `/state`: To get info about userbot
- `/clear`: Delete all stored replies
- `/cancel`: To cancel current operation
- `/auto_replies`: Display all stored auto-replies
- `/signature`: Edit/Disable messages signature
- `/disallow_reply_when_online`: Disallow userbot to reply when user is online
- `/allow_reply_when_online`: Allow userbot to reply when user is online
- `/enable_groups`: Allows auto-reply in groups
- `/disable_groups`: Disables auto-reply in groups

## TODO List:
- [x] Creating a repository contains built result of TDLib, And remove it from here.
- [x] Adding ability to set API_ID & API_HASH in console without environment variable.
- [ ] Adding more options for auto-reply filtering

## Licence
GPL 3.0